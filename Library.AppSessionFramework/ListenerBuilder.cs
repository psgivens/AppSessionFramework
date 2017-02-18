using PhillipScottGivens.Library.AppSessionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhillipScottGivens.Library.AppSessionFramework {
    public class ListenerBuilder {
        private readonly object _listeningObject;
        private readonly List<SessionListener> _listeners = new List<SessionListener>();
        
        public ListenerBuilder(object listeningObject) {
            this._listeningObject = listeningObject;
        }

        public ListenerBuilder ListenForTraitChange<T>(EventHandler handler)
            where T : SessionBase {
            return ListenForTraitChange<T>(handler, null);
        }
        public ListenerBuilder ListenForTraitChange<T>(EventHandler handler, Predicate<SessionBase> matchSession)
            where T : SessionBase {
            var listener = new SessionListener(SessionNotificationType.TraitChanged, typeof(T), matchSession ?? (Predicate<SessionBase>)(s => true));
            listener.Initialize(handler);
            _listeners.Add(listener);
            return this;
        }
        public ListenerBuilder ListenForTraitChange(string path, EventHandler handler) {
            var listener = new SessionListener(SessionNotificationType.TraitChanged, _listeningObject, path);
            listener.Initialize(handler);
            _listeners.Add(listener);
            return this;
        }


        public ListenerBuilder ListenForAvailabilityChange<T>(EventHandler handler)
            where T : SessionBase {
            return ListenForAvailabilityChange<T>(handler, null);
        }
        public ListenerBuilder ListenForAvailabilityChange<T>(EventHandler handler, Predicate<SessionBase> matchSession)
            where T : SessionBase {
            var listener = new SessionListener(SessionNotificationType.AvailabilityChanged, typeof(T), matchSession ?? (Predicate<SessionBase>)(s => true));
            listener.Initialize(handler);
            _listeners.Add(listener);
            return this;
        }
        public ListenerBuilder ListenForAvailabilityChange(string path, EventHandler handler) {
            var listener = new SessionListener(SessionNotificationType.AvailabilityChanged, _listeningObject, path);
            listener.Initialize(handler);
            _listeners.Add(listener);
            return this;
        }

        internal IEnumerable<PhillipScottGivens.Library.AppSessionFramework.SessionListener> Listeners { get { return _listeners; } }
    }
}
