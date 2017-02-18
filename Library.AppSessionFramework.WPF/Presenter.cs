using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace PhillipScottGivens.Library.AppSessionFramework.WPF
{
    public class Presenter : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Factory
        private static readonly Dictionary<SessionBase, Presenter> presenterMap
            = new Dictionary<SessionBase, Presenter>();
        private static readonly Type presenterType = typeof(Presenter<>);
        public static Presenter GetOrCreate(SessionBase session)
        {
            Presenter presenter;
            if (presenterMap.TryGetValue(session, out presenter))
                return presenter;

            var thisPresenterType = presenterType.MakeGenericType(session.GetType());
            presenter = (Presenter)Activator.CreateInstance(thisPresenterType, session);
            presenterMap.Add(session, presenter);
            session.Disposed += (p, e) => presenterMap.Remove((SessionBase)p);
            return presenter;
        }
        #endregion
    }

    public class Presenter<TValue> : Presenter, ICustomTypeDescriptor,
        IHasValue<TValue>, IHasNotifyableValue<TValue>
        where TValue : SessionBase
    {
        #region Fields
        private PropertyDescriptorCollection _properties;
        #endregion

        #region Initialize and Teardown
        public Presenter(TValue value)
        {
            this.Value = value;
            //value.PropertyChanged += (s, e) => RaisePropertyChanged(e.PropertyName);
        }
        #endregion

        #region IHasTValue
        public TValue Value { get; private set; }
        #endregion

        #region GetProperties
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            if (_properties == null)
            {
                var properties = TypeDescriptor.GetProperties(Value);
                var descriptors = new List<PropertyDescriptor>();
                for (int index = 0; index < properties.Count; index++)
                {
                    PropertyDescriptor descriptor = properties[index];
                    if (descriptor.PropertyType.IsSubclassOf(typeof(SessionBase)))
                    {
                        descriptors.Add(new SessionPresenterDescriptor<TValue>(descriptor));
                    }
                    else
                    {
                        descriptors.Add(new PassThroughPropertyDescriptor<TValue>(descriptor));

                        // TODO: Add data property availabilities here.
                    }
                }
                foreach (MethodInfo method in typeof(TValue).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                //    descriptors.Add(new MethodCommandDescriptor<TValue>(method));

                    // TODO: Add operation availabilities here.
                }
                _properties = new PropertyDescriptorCollection(descriptors.ToArray());
            }
            return _properties;
        }
        #endregion

        #region other ICustomTypeDescriptor members
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return  TypeDescriptor.GetAttributes(this);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(this, attributes);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion

        public new void RaisePropertyChanged(string name)
        {
            base.RaisePropertyChanged(name);
        }
    }
}
