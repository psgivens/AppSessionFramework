using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Input;

namespace PhillipScottGivens.Library.AppSessionFramework.WPF
{
    public class SessionPresenterDescriptor<TValue> : PropertyDescriptor
    {
        private readonly PropertyDescriptor propertyDescriptor;
        public SessionPresenterDescriptor(PropertyDescriptor propertyDescriptor)
            : base(propertyDescriptor.Name, null)
        {
            this.propertyDescriptor = propertyDescriptor;
        }

        public override object GetValue(object component)
        {
            var value = (SessionBase)propertyDescriptor
                .GetValue(((IHasValue<TValue>)component).Value);
            return value == null
                ? null
                : Presenter.GetOrCreate(value);
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
            get { return typeof(Presenter); }
        }
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
        public override Type ComponentType
        {
            get
            {
                return propertyDescriptor.ComponentType;
            }
        }
    }
}