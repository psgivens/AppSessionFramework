using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework.ComponentModel
{
    public class AvailabilityPropertyDescriptor : PropertyDescriptor
    {
        private readonly MemberInfo memberInfo;
        public AvailabilityPropertyDescriptor(MemberInfo memberInfo)
            : base(string.Format("Is{0}Available", memberInfo.Name), null)
        {
            this.memberInfo = memberInfo;
        }

        public override object GetValue(object component)
        {
            var session = component as SessionBase;
            return SessionManager.WrapAsAvailability(
                memberInfo is MethodInfo
                    ? (AvailabilityStore)session.Operations
                        .First(item => item.Name == memberInfo.Name)
                    : (AvailabilityStore)session.DataProperties
                        .First(item => item.Name == memberInfo.Name));
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
            get { return typeof(bool); }
        }
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
        public override Type ComponentType
        {
            get
            {
                return memberInfo.DeclaringType;
            }
        }

    }
}
