using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework
{
    #region class SessionPropertyDescriptor<TValue>
    public class SessionPropertyDescriptor<TValue> : DataPropertyDescriptor<TValue>
        where TValue : SessionBase
    {
        #region Constructors
        protected SessionPropertyDescriptor(string name, Type sessionType)
            : base(name, sessionType)
        {
        }
        #endregion

        #region Static methods
        public static SessionPropertyDescriptor<TValue> Register(string name, Type sessionType)
        {
            SessionPropertyDescriptor<TValue> descriptor;
            DataPropertyDescriptor.RegisterPropertyDescriptor(sessionType, descriptor = new SessionPropertyDescriptor<TValue>(name, sessionType));
            return descriptor;
        }
        #endregion

        internal override DataPropertyStore CreateDataStore(string name)
        {
            return new SessionPropertyStore<TValue>(name);
        }
    }
    #endregion
}
