using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.SessionFramework.Generators
{
    public class SessionInfo
    {
        private static readonly Type AbstractAttributeType = typeof(AbstractBaseSessionAttribute);
        private static readonly Type OperationAttributeType = typeof(OperationAttribute);

        private readonly MethodInfo[] operations;
        public SessionInfo(Type type)
        {
            this.Type = type;
            IsAbstractBase = type.IsGenericTypeDefinition
                || type.IsDefined(AbstractAttributeType, false);

            operations = (from method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                          where method.IsVirtual && !method.IsFinal
                             && method.IsDefined(OperationAttributeType, true)
                          select method).ToArray();
        }

        public Type Type { get; private set; }
        public bool IsAbstractBase { get; private set; }
        public IEnumerable<MethodInfo> Operations { get { return operations; } }
    }
}
