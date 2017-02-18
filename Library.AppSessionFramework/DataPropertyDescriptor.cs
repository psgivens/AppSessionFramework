using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace PhillipScottGivens.Library.AppSessionFramework
{
    #region abstract class DataPropertyDescriptor
    public abstract class DataPropertyDescriptor
    {
        #region Properties
        public string PropertyName { get; private set; }
        public Type sessionType { get; private set; }
        #endregion

        #region Constructors
        protected DataPropertyDescriptor(string name, Type sessionType)
        {
            this.PropertyName = name;
            this.sessionType = sessionType;
        }
        #endregion

        private static Dictionary<Type, List<DataPropertyDescriptor>> propertyDescriptors = new Dictionary<Type, List<DataPropertyDescriptor>>();
        internal static void RegisterPropertyDescriptor(Type sessionType, DataPropertyDescriptor descriptor)
        {
            List<DataPropertyDescriptor> descriptors;
            if (!propertyDescriptors.TryGetValue(sessionType, out descriptors))
            {
                descriptors = new List<DataPropertyDescriptor>();
                propertyDescriptors.Add(sessionType, descriptors);
            }
            descriptors.Add(descriptor);
        }
        internal static IEnumerable<DataPropertyDescriptor> GetDescriptors(Type sessionType)
        {
            for (Type current = sessionType; current != typeof(SessionBase); current = current.GetTypeInfo().BaseType)
            {
                if (!propertyDescriptors.ContainsKey(current))
                    continue;

                var descriptors = propertyDescriptors[current];
                for (int index = 0; index < descriptors.Count; index++)
                    yield return descriptors[index];
            }
        }
        internal virtual DataPropertyStore CreateDataStore(string name)
        {
            throw new NotSupportedException("CreateDataStore must be overriden.");
        }
    }
    #endregion
}
