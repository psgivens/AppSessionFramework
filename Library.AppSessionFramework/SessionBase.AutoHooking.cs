using System;
using System.Collections.Generic;

namespace PhillipScottGivens.Library.AppSessionFramework {
    public partial class SessionBase {
        #region Fields
        /// <summary>
        /// Data representing the identification of this session type.
        /// </summary>
        internal SessionManager.SessionIdentifier TypeIdentifier { get; private set; }
        ///// <summary>
        ///// Collection of session listeners corresponding to methods on this session.
        ///// </summary>
        //private List<sessionListenerAttribute> rootedListeners = new List<sessionListenerAttribute>();

        // This should already exist in the real SessionBase.
        //private Dictionary<string, SessionPropertyStore> sessionTraitStores = new Dictionary<string, SessionPropertyStore>();
        #endregion

        #region Events
        // This should already exist in the real SessionBase.
        public event EventHandler DataChanged;
        // This should already exist in the real SessionBase.
        public event EventHandler AvailabilityChanged;
        public event EventHandler Closed;
        #endregion

        #region Initialize and Tear Down
        private static int globalCount = 0;
        internal int debugNumber = globalCount++;
        //public SessionBase()
        //{
        //    TypeIdentifier = SessionManager.GetIdentifier(GetType());
        //    ulong key = TypeIdentifier.sessionOnly;

        //    //
        //    // Create sessionTraitStores
        //    // In real code, this will be done in a similar fashion as DataObjectTraitStores
        //    //
        //    PropertyInfo[] propertyInfos = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        //    foreach (var property in propertyInfos)
        //        if (typeof(SessionBase).IsAssignableFrom(property.PropertyType))
        //            sessionTraitStores.Add(property.Name, new sessionTraitStore(this, property.Name));

        //    SessionManager.RegisterListener(this);
        //}

        protected virtual void OnConstructed() {
            SessionManager.PostSessionCreated(this, OnBuildListeners);
        }

        protected virtual void OnBuildListeners(ListenerBuilder builder) { }

        protected internal void CloseSession() {
            OnClosed();
        }

        private void OnClosed() {
            SessionManager.PostSessionClosed(this);
            if (Closed != null)
                Closed(this, EventArgs.Empty);
        }
        #endregion

        #region Event Notifiers
        // TODO: Make this private
        protected void NotifyTraitChanged(string traitName, bool drivingMessge) {
            var message = new NotificationMessage(this, traitName, drivingMessge);
            if (drivingMessge)
                SessionManager.CreatedMessage = message;

            if (DataChanged != null)
                DataChanged(this, new SessionChangedChangedArgs(message));
        }
        #endregion

        #region Trait Hook Registration
        /// <summary>
        /// Registers a session listener with a particular trait on this session. 
        /// </summary>
        /// <param name="listener">session listener to be registered</param>
        internal void RegisterSessionListenerOnTrait(SessionListener listener) {
            var dataStore = _dataPropertyLookup[listener.PathInfo.UnavailableTrait];
            var sessionStore = dataStore as ISessionPropertyStore;
            if (sessionStore != null) {
                sessionStore.RegisterSessionListener(listener);
            }
            else {
                listener.PathInfo.Push(dataStore);
                dataStore.ValueChanged += (EventHandler)listener.EventHandler;
                ((EventHandler)listener.EventHandler)(null, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Unregisters a session listener from a particular trait on this session. 
        /// </summary>
        /// <param name="listener">session listener to be unregistered</param>
        internal void UnregisterSessionListenerOnTrait(SessionListener listener) {
            var pathInfo = listener.PathInfo;
            string availableProperty;
            if (pathInfo.HasAvailableTrait 
                && _dataPropertyLookup.ContainsKey(availableProperty = listener.PathInfo.AvailableTrait))
            {
                var dataStore = _dataPropertyLookup[availableProperty];
                var sessionStore = dataStore as ISessionPropertyStore;
                if (sessionStore != null) {
                    sessionStore.UnRegistersessionListener(listener);
                }
                else {
                    listener.PathInfo.Pop(dataStore);
                    dataStore.ValueChanged -= (EventHandler)listener.EventHandler;
                }
            }
            else if (listener.PathInfo.Rootsession == this) {
                var traitStore = (ISessionPropertyStore)_dataPropertyLookup[listener.PathInfo.RootTrait];
                if (traitStore.Value != null)
                    traitStore.RescindListener(listener);
            }
            else {
                throw new ArgumentException("Supplied listener is not registered with this session.", "listener");
            }
        }
        #endregion
    }

    #region event args sessionChangedChangedArgs
    public class SessionChangedChangedArgs : EventArgs {
        internal SessionChangedChangedArgs(NotificationMessage message) {
            this.Message = message;
        }

        public NotificationMessage Message { get; private set; }
    }
    #endregion
}
