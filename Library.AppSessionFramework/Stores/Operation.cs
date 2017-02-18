using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework
{
    #region class Operation
    public class Operation : AvailabilityStore
    {
        #region Fields
        private Action action;
        #endregion

        #region Properties
        public string Name { get; private set; }
        #endregion

        #region Constructor
        public Operation(string name, Action action)
        {
            this.action = action;
            Name = name;
        }
        #endregion

        #region Methods
        public void Execute()
        {
            action();
        }
        #endregion
    }
    #endregion
}
