using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework {
    internal class SessionPropertyStore<TSession> : DataPropertyStore<TSession>,
        ISessionPropertyStore
        where TSession : SessionBase {
        #region Fields
        /// <summary>
        /// Collection of listeners that require this the value of this trait store to work.
        /// </summary>
        /// <remarks>
        /// All of these listeners hook/unhook in relation to the availability of traits that contain
        /// sessions. Some of the traits that contain sessions represent the session to be hooked.
        /// Some of the traits that contain sessions represent part of the relative path to the 
        /// session to be hooked. 
        /// </remarks>
        /// <see cref="PropogateListener"/>
        private List<SessionListener> _sessionListeners = new List<SessionListener>();
        private bool _isValueChanging;
        #endregion

        #region Constructors
        public SessionPropertyStore(string name)
            : base(name) {
            SetAvailabilityCondition(new DynamicBool(() => Value != null, ValueNotifier));
        }
        #endregion


        protected override void OnAvailabilityChanged() {
            if (Value != null && !_isValueChanging) {
                if (IsAvailable) {
                    foreach (var listener in _sessionListeners) {
                        if (!(listener is LinkToSession)) {
                            // TODO: Account for linking to property or to the whole session. 
                            listener.PathInfo.Push((ISessionPropertyStore)this);
                        }
                        PropogateListener(listener);
                    }
                }
                else {
                    foreach (var listener in _sessionListeners) {
                        ((ISessionPropertyStore)this).RescindListener(listener);
                        if (!(listener is LinkToSession)) {
                            // TODO: Account for linking to property or to the whole session. 
                            listener.PathInfo.Pop((ISessionPropertyStore)this);
                        }
                        
                    }
                }
            }
            base.OnAvailabilityChanged();
        }

        #region Value
        object ISessionPropertyStore.Value { get { return this.Value; } }
        
        public override object Value {
            get {
                return base.Value;
            }

            internal set {
                var oldValue = base.Value as SessionBase;
                if (oldValue != null && IsAvailable)
                    foreach (var listener in _sessionListeners)
                        ((ISessionPropertyStore)this).RescindListener(listener);

                try {
                    _isValueChanging = true;
                    base.Value = value;
                }
                finally {
                    _isValueChanging = false;
                }

                if (value != null && IsAvailable)
                    foreach (var listener in _sessionListeners)
                        PropogateListener(listener);
            }
        }
        #endregion

        #region Registering Hooks
        /// <summary>
        /// Registers a session listener as being dependant on this trait, either
        /// directly or indirectly.
        /// </summary>
        /// <param name="listener">session listener to be registered.</param>
        void ISessionPropertyStore.RegisterSessionListener(SessionListener listener) {
            _sessionListeners.Add(listener);

            if (IsAvailable) {
                listener.PathInfo.Push((ISessionPropertyStore)this);

                PropogateListener(listener);
            }
        }

        /// <summary>
        /// Unregisters a session listener as being dependant on this trait, either
        /// directly or indirectly.
        /// </summary>
        /// <param name="listener">session listener to be unregistered.</param>
        void ISessionPropertyStore.UnRegistersessionListener(SessionListener listener) {
            bool isAvailable = IsAvailable;
            if (isAvailable)
                ((ISessionPropertyStore)this).RescindListener(listener);

            _sessionListeners.Remove(listener);

            if (isAvailable)
                listener.PathInfo.Pop((ISessionPropertyStore)this);
        }
        #endregion

        #region Hook Processing
        /// <summary>
        /// Propogates the listener toward its final destination of being hooked.
        /// </summary>
        /// <param name="listener">Listener to be hooked</param>
        /// <remarks>
        /// In the case that the "Value" of this trait store is the session to be
        /// hooked, it will be hooked in this method. Otherwise, the listener will
        /// be registered with the value a link in the chain toward hooking the 
        /// appropriate session. 
        /// </remarks>
        private void PropogateListener(SessionListener listener) {
            var value = Value as SessionBase;
            if (listener.PathInfo.IsTargetTrait(this)) {
                // Hook the change handler
                listener.HookSession(value, false);
            }
            else {
                // Register change listener with next session
                value.RegisterSessionListenerOnTrait(listener);
            }
        }

        /// <summary>
        /// Rescinds the previously propogated listener.
        /// </summary>
        /// <param name="listener">Listener to be rescinded</param>
        /// <see cref="PropogateListener"/>
        void ISessionPropertyStore.RescindListener(SessionListener listener) {
            var value = this.Value as SessionBase;
            if (listener.PathInfo.IsTargetTrait(this)) {
                // Unhook the change handler
                listener.UnhookSession(value, false);
            }
            else {
                // Unregister change listener from the next session
                value.UnregisterSessionListenerOnTrait(listener);
            }
        }
        #endregion
    }
}
