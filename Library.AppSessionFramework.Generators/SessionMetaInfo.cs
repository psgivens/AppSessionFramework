using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PhillipScottGivens.Library.AppSessionFramework.Generators
{
    public class SessionMetaInfo
    {
        public SessionMetaInfo(System.Type type)
        {
            Type = type;
            IsAbstractBase = type.IsGenericTypeDefinition
                || type.IsDefined(SessionProxyGeneratorBase.AbstractBaseSessionAttribute, false);

            IsBaseMostSession = type.BaseType == SessionProxyGeneratorBase.SessionBaseType;

            Operations = GetOperationMethodInfos(type).ToList();
            //Operations = (from method in type.GetMethods(
            //              BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            //              where method.IsDefined(SessionProxyGeneratorBase.OperationAttributeType)
            //              select method).AsEnumerable();

            if (!Operations.All(item => item.IsVirtual))
                throw new InvalidOperationException("Non-virtual operation found.");

            DataProperties = GetProperties(type, false);
            //AllDataProperties = GetProperties(type, true);
        }

        private static IEnumerable<MethodInfo> GetOperationMethodInfos(Type type) {
            TypeInfo typeInfo;
            for (; type != SessionProxyGeneratorBase.SessionBaseType; type = typeInfo.BaseType) {
                typeInfo = type.GetTypeInfo();
                var field = typeInfo.GetDeclaredField("OperationNames");
                if (field == null) continue;
                var operationNames = (IEnumerable<string>)field.GetValue(null);
                foreach (var operationName in operationNames) {
                    var method = typeInfo.GetDeclaredMethod(operationName);
                    yield return method;
                }
            }
        }

        // TECHNOTE: Recursing through an IEnumerable
        private IEnumerable<PropertyInfo> GetProperties(Type type, bool recurse)
        {
            foreach (var propertyInfo
                in (from property in type.GetProperties(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    where property.IsDefined(SessionProxyGeneratorBase.ObservableAttribute)
                    where type == property.DeclaringType
                    select property))
                yield return propertyInfo;

            var baseType = type.BaseType;
            if (recurse && baseType != SessionProxyGeneratorBase.SessionBaseType)
                foreach (var propertyInfo in GetProperties(baseType, recurse))
                    yield return propertyInfo;
        }

        public bool IsAbstractBase { get; private set; }

        public bool IsBaseMostSession { get; private set; }

        public Type Type { get; private set; }

        public IEnumerable<MethodInfo> Operations { get; private set; }

        public IEnumerable<PropertyInfo> DataProperties { get; private set; }

        //public IEnumerable<PropertyInfo> AllDataProperties { get; private set; }
    }
}
