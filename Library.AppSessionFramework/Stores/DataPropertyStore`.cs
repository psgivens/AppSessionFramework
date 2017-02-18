using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework
{
    #region class DataPropertyStore<TValue>
    public class DataPropertyStore<TValue> : DataPropertyStore, IPropertyStore<TValue>
    {
        #region Constructor and Factory Methods
        internal DataPropertyStore(string name)
            : base(name)
        {
            Value = default(TValue);
            SetAvailabilityCondition(new DynamicBool(() => Value != null, ValueNotifier));
        }
        #endregion
        
        TValue IPropertyStore<TValue>.Value {
            get { return (TValue)base.Value; }
        }
    }
    #endregion
}
