using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework
{
    #region class BooleanPropertyStore
    public class BooleanPropertyStore : DataPropertyStore<bool>
    {
        #region Properties
        public new bool Value
        {
            get
            {
                return (bool)base.Value;
            }
            internal set
            {
                base.Value = value;
            }
        }
        #endregion

        #region Constructors
        internal BooleanPropertyStore(string name)
            : base(name)
        {
        }
        #endregion
    }
    #endregion
}
