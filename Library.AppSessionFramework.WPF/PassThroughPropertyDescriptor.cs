using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhillipScottGivens.Library.AppSessionFramework.WPF
{
    public class PassThroughPropertyDescriptor<TValue> : PropertyDescriptor
        where TValue : SessionBase
    {

        private readonly PropertyDescriptor propertyDescriptor;
        public PassThroughPropertyDescriptor(PropertyDescriptor propertyDescriptor)
            : base(propertyDescriptor.Name, null)
        {
            this.propertyDescriptor = propertyDescriptor;
        }

        public override object GetValue(object component)
        {
            return propertyDescriptor.GetValue(((IHasValue<TValue>)component).Value);
        }
        public override void SetValue(object component, object value)
        {
            propertyDescriptor.SetValue(((IHasValue<TValue>)component).Value, value);
        }
        public override void ResetValue(object component)
        {
            propertyDescriptor.ResetValue(((IHasValue<TValue>)component).Value);
        }
        public override bool IsReadOnly
        {
            get { return propertyDescriptor.IsReadOnly; }
        }
        public override bool CanResetValue(object component)
        {
            return propertyDescriptor.CanResetValue(((IHasValue<TValue>)component).Value);
        }
        public override Type PropertyType
        {
            get { return propertyDescriptor.PropertyType; }
        }
        public override bool ShouldSerializeValue(object component)
        {
            return propertyDescriptor.ShouldSerializeValue(((IHasValue<TValue>)component).Value);
        }
        public override Type ComponentType
        {
            get
            {
                return typeof(Presenter<TValue>);
            }
        }
    }
}
