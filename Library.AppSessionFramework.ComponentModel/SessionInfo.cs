using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework.ComponentModel {
    public class SessionInfo<TSession> : SessionInfo
        where TSession : SessionBase {
        private static readonly IEnumerable<MethodInfo> operationMethods;
        static SessionInfo() {
            var operations = new List<MethodInfo>();
            operationMethods = GetOperationMethodInfos(typeof(TSession));
        }

        public static IEnumerable<MethodInfo> GetOperationMethods() {
            return operationMethods;
        }

        public override IEnumerable<MethodInfo> OperationMethods {
            get { return operationMethods; }
        }
    }

    public abstract class SessionInfo {
        public static IEnumerable<MethodInfo> GetOperationMethods(Type sessionType) {
            var infoType = (typeof(SessionInfo<>)).MakeGenericType(sessionType);
            var info = (SessionInfo)Activator.CreateInstance(infoType);
            return info.OperationMethods;
        }

        public abstract IEnumerable<MethodInfo> OperationMethods { get; }

        public static IEnumerable<MethodInfo> GetOperationMethodInfos(Type type) {
            TypeInfo typeInfo;
            for (; type != typeof(SessionBase); type = typeInfo.BaseType) {
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
    }
}
