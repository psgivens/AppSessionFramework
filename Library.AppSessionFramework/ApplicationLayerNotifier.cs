using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PhillipScottGivens.Library.AppSessionFramework
{
    #region class AvailabiltyConditionNotifier
    public abstract class DualLayerNotifier
    {
        #region Fields
        private object sender;
        private ApplicationLayerEventArgs eventArgs;
        #endregion

        #region Constructor
        public DualLayerNotifier(object sender)
        {
            this.sender = sender;
            eventArgs = new ApplicationLayerEventArgs(this);
        }
        #endregion

        #region Methods
        internal virtual void Notify(NotifyLayer layer)
        {
            switch (layer)
            {
                case NotifyLayer.Primary:
                    if (changeNotificationSentToApplication != null)
                        changeNotificationSentToApplication(sender, eventArgs);
                    break;

                case NotifyLayer.Secondary:
                    if (changeNotificationSentToConsumers != null)
                        changeNotificationSentToConsumers(sender, eventArgs);
                    break;
            }
        }
        #endregion

        #region Events
        private EventHandler changeNotificationSentToApplication;
        private EventHandler changeNotificationSentToConsumers;
        // TODO: Rename this event, it's value has grown beyond the original scope.
        public event EventHandler ChangeNotificationSent
        {
            add
            {
                if (IsPrimaryNotification(value))
                    changeNotificationSentToApplication += value;
                else
                    changeNotificationSentToConsumers += value;
            }
            remove
            {
                if (IsPrimaryNotification(value))
                    changeNotificationSentToApplication -= value;
                else
                    changeNotificationSentToConsumers -= value;
            }
        }
        #endregion

        public abstract bool IsPrimaryNotification(EventHandler value);
    }
    #endregion

    public class ApplicationLayerNotifier : DualLayerNotifier
    {
        public ApplicationLayerNotifier(object sender)
            : base(sender)
        {
        }

        public override bool IsPrimaryNotification(EventHandler value)
        {
            return (value.Target == null
                || value.Target is SessionBase
                || value.Target.GetType().GetTypeInfo().Assembly == this.GetType().GetTypeInfo().Assembly)
                && !value.GetMethodInfo().IsDefined(typeof(NotifyConsumerAttribute), false);
        }
    }

    #region class DirtyAvailabilityConditionNotifier
    /// <summary>
    /// DirtyAvailabilityConditionNotifier is just like AvailabilityConditionNotifier except it is 
    /// not to have been created in the application layer. As a consequence, it is not registered
    /// with a session to notify the presentation layer. To get around this limitation, this 
    /// class registeres itself with the session manager when a change occurs. It will then 
    /// notify the consuming component when the we've unwound from the applicaiton layer. 
    /// </summary>
    public class NonSessionNotifier : ApplicationLayerNotifier
    {
        #region Fields
        private bool isDirty;
        #endregion

        #region Constructor
        public NonSessionNotifier(object sender)
            : base(sender) { }
        #endregion

        #region Overrides
        internal override void Notify(NotifyLayer layer)
        {
            switch (layer)
            {
                case NotifyLayer.Primary:
                    isDirty = true;
                    SessionManager.EnqueueNotifier(this);
                    base.Notify(layer);
                    break;

                case NotifyLayer.Secondary:
                    if (isDirty)
                        base.Notify(layer);
                    break;
            }
        }
        #endregion
    }
    #endregion

    #region class ApplicationLayerEventArgs
    public class ApplicationLayerEventArgs : EventArgs
    {
        #region Properties
        public DualLayerNotifier Notifier { get; private set; }
        #endregion

        #region Constructor
        public ApplicationLayerEventArgs(DualLayerNotifier notifier)
        {
            Notifier = notifier;
        }
        #endregion
    }
    #endregion

    #region enum NotifyLayers
    internal enum NotifyLayer
    {
        Primary,
        Secondary,
    }
    #endregion
}
