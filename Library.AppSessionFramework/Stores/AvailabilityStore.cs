using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework
{
    #region class AvailabilityStore
    // TODO: Implement mechanism for automatically hooking via path similar to the...
    // current session component's AutoHooking events. 
    public class AvailabilityStore
    {
        #region Fields
        private DynamicBool availabilityCalculation;
        #endregion

        #region Properties
        public DualLayerNotifier AvailabilityNotifier { get; private set; }

        public bool IsAvailable
        {
            get
            {
                return availabilityCalculation == null || availabilityCalculation.Value;
            }
        }
        #endregion

        #region Events
        // This event makes the Value property databindable. 
        // TODO: Verify the above statement. 
        public event EventHandler IsAvailableChanged
        {
            add { AvailabilityNotifier.ChangeNotificationSent += value; }
            remove { AvailabilityNotifier.ChangeNotificationSent -= value; }
        }
        #endregion

        #region Constructor
        public AvailabilityStore()
        {
            AvailabilityNotifier = new ApplicationLayerNotifier(this);
        }
        #endregion

        #region Methods
        internal void SetAvailabilityCondition(DynamicBool condition)
        {
            availabilityCalculation = condition;
            availabilityCalculation.ValueNotifier.ChangeNotificationSent += ValueNotifier_AvailabilityConditionChanged;
        }

        void ValueNotifier_AvailabilityConditionChanged(object sender, EventArgs e)
        {
            AvailabilityNotifier.Notify(NotifyLayer.Primary);
            OnAvailabilityChanged();
        }

        protected virtual void OnAvailabilityChanged() { }
        #endregion
    }
    #endregion
}
