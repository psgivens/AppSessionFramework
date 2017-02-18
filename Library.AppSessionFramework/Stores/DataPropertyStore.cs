using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework
{
    #region abstract class DataPropertyStore
    // TOOD: Make this class internal
    public class DataPropertyStore : AvailabilityStore
    {
        #region Fields
        private object value;
        #endregion

        #region Constructor and Factory Methods
        protected DataPropertyStore(string name)
        {
            Name = name;
            ValueNotifier = new ApplicationLayerNotifier(this);
        }
        #endregion

        #region Events
        public event EventHandler ValueChanged
        {
            add { ValueNotifier.ChangeNotificationSent += value; }
            remove { ValueNotifier.ChangeNotificationSent -= value; }
        }
        #endregion

        #region Properties
        public string Name { get; private set; }
        public DualLayerNotifier ValueNotifier { get; private set; }
        #endregion

        #region Properties
        // TODO: Evaluate the necessity of this flag. It may be handled for us in the ValueNotifier.
        internal bool IsDirty { get; private set; }
        public virtual object Value
        {
            get
            {
                return value;
            }
            internal set
            {
                if (ReferenceEquals(this.value, value))
                    return;

                this.value = value;

                IsDirty = true;

                ValueNotifier.Notify(NotifyLayer.Primary);
            }
        }
        #endregion
    }
    #endregion
}
