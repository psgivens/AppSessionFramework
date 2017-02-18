using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework.ComponentModel
{
    public class OperationPropertyDescriptor : PropertyDescriptor
    {
        private readonly MethodInfo methodInfo;
        public OperationPropertyDescriptor(MethodInfo methodInfo)
            : base(methodInfo.Name, null)
        {
            this.methodInfo = methodInfo;
        }

        public override object GetValue(object component)
        {
            var session = component as SessionBase;
            return SessionManager.WrapOperation(
                session.Operations.First(item => item.Name == methodInfo.Name));
        }
        public override void SetValue(object component, object value)
        {
            throw new NotSupportedException("SetValue");
        }
        public override void ResetValue(object component)
        {
            throw new NotSupportedException("ResetValue");
        }
        public override bool IsReadOnly
        {
            get { return true; }
        }
        public override bool CanResetValue(object component)
        {
            return false;
        }
        public override Type PropertyType
        {
            get { return typeof(Operation); }
        }
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
        public override Type ComponentType
        {
            get
            {
                return methodInfo.DeclaringType;
            }
        }
    }
}
