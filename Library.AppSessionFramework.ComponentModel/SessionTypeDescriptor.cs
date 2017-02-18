using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework.ComponentModel
{
    public class SessionTypeDescriptor<TSession> : CustomTypeDescriptor
        where TSession : SessionBase
    {
        private static PropertyDescriptorCollection _properties;
        private static EventDescriptorCollection _events;

        static SessionTypeDescriptor()
        {
        }

        internal SessionTypeDescriptor(ICustomTypeDescriptor typeDescriptor)
            : base(typeDescriptor)
        {
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            if (_properties == null)
            {
                var properties = base.GetProperties();
                var descriptors = new List<PropertyDescriptor>(properties.OfType<PropertyDescriptor>());
                for (int index = 0; index < properties.Count; index++)
                {
                    PropertyDescriptor descriptor = properties[index];
                    if (descriptor.PropertyType.IsSubclassOf(typeof(SessionBase)))
                    {
                        descriptors.Add(descriptor);
                    }
                    else
                    {
                        descriptors.Add(descriptor);

                        // TODO: Add data property availabilities here.
                    }
                }
                foreach (MethodInfo method in SessionInfo<TSession>.GetOperationMethods())// typeof(TSession).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    descriptors.Add(new OperationPropertyDescriptor(method));

                    // TODO: Add operation availabilities here.
                }
                _properties = new PropertyDescriptorCollection(descriptors.ToArray());
            }

            return _properties;
        }

        //public override EventDescriptorCollection GetEvents()
        //{
        //    if (_events == null)
        //    {
        //        var events = base.GetEvents();
        //        var descriptors = new List<EventDescriptor>(events.OfType<EventDescriptor>());
        //        for (int index = 0; index < events.Count; index++)
        //        {
        //            var descriptor = events[index];

        //            descriptors.Add(descriptor);
        //        }

        //        //foreach (MethodInfo method in SessionInfo<TSession>.GetOperationMethods())// typeof(TSession).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        //        //{
        //        //    descriptors.Add(new OperationPropertyDescriptor(method));

        //        //    // TODO: Add operation availabilities here.
        //        //}
        //        //_properties = new PropertyDescriptorCollection(descriptors.ToArray());
        //    }
        //    return _events;

        //    //return base.GetEvents();
        //}
    }
}
