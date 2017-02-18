using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace PhillipScottGivens.Library.AppSessionFramework
{
    #region class DataPropertyDescriptor<TValue>
    public class DataPropertyDescriptor<TValue> : DataPropertyDescriptor
    {
        #region Constructors
        protected DataPropertyDescriptor(string name, Type sessionType)
            : base(name, sessionType)
        {
        }
        #endregion

        #region Static methods
        public static DataPropertyDescriptor<TValue> Register(string name, Type sessionType)
        {
            System.Diagnostics.Debug.Assert(!typeof(SessionBase).GetTypeInfo().IsAssignableFrom(typeof(TValue).GetTypeInfo()));

            DataPropertyDescriptor<TValue> descriptor;
            DataPropertyDescriptor.RegisterPropertyDescriptor(sessionType, descriptor= new DataPropertyDescriptor<TValue>(name, sessionType));
            return descriptor;
        }
        #endregion

        internal override DataPropertyStore CreateDataStore(string name)
        {
            if (typeof(TValue) == typeof(bool))
                return new BooleanPropertyStore(name);
            return new DataPropertyStore<TValue>(name);
        }
    }
    #endregion
}
