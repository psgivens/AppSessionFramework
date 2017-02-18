using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PhillipScottGivens.Library.AppSessionFramework
{
    public static partial class SessionManager
    {
        public static void CloseAll()
        {
            var sessions = Sessions;
            while (sessions.Count > 0)
            {
                var session = sessions[0];
                session.CloseSession();
            }
        }

        #region Fields
        // We are using sessions from the other file.
        //private static List<SessionBase> sessions = new List<SessionBase>();

        /// <summary>
        /// A lookup of session Listeners by a ulong session identifier.
        /// </summary>
        private static Dictionary<ulong, List<SessionListener>> listenerTypeLookup = new Dictionary<ulong, List<SessionListener>>();
        /// <summary>
        /// A lookup of session Listeners by the object that handles the change notification.
        /// </summary>
        private static Dictionary<object, List<SessionListener>> listenerLookup = new Dictionary<object, List<SessionListener>>();
        /// <summary>
        /// Current counter representing the last unique session identifier dispensed.
        /// </summary>
        private static ulong nextIdentifierToBeIssued = 1;
        /// <summary>
        /// All worklfow identifieres that have been dispensed. 
        /// </summary>
        private static Dictionary<Type, SessionIdentifier> sessionIdentifiers = new Dictionary<Type, SessionIdentifier>();

        #region MyClass Equivalent from AutoHookingSample
        /// <summary>
        /// Message created when ChangeTraitValue is called.
        /// </summary>
        public static NotificationMessage CreatedMessage { get; set; }
        /// <summary>
        /// Message received when a SessionBase.WL is called.
        /// </summary>
        /// <remarks>
        /// This is obviously not the appropriate place to assign ReceivedMessage, but
        /// this is simple code meant to test the features. Instead of cleaning this 
        /// up, I am leaving this comment. 
        /// </remarks>
        public static NotificationMessage ReceivedMessage { get; set; }
        #endregion
        #endregion

        #region session Identifiers
        internal struct SessionIdentifier
        {
            /// <summary>
            /// A single bit representing unique to a session type.
            /// </summary>
            public ulong sessionOnly;

            /// <summary>
            /// A bit field containing a session type and all of its ancestors.
            /// </summary>
            public ulong Hierarchy;
        }

        /// <summary>
        /// Method for generating and looking up session Identifiers by type.
        /// </summary>
        /// <param name="sessionType">Type of session.</param>
        /// <returns>Identifier unique to the supplied type of session.</returns>
        internal static SessionIdentifier GetIdentifier(Type sessionType)
        {
            // If we previously did a check, pull value from cache.
            if (sessionIdentifiers.ContainsKey(sessionType))
                return sessionIdentifiers[sessionType];

            // get the base type
            var baseType = sessionType.GetTypeInfo().BaseType;

            // get the base type key
            var baseTypeKey = baseType == typeof(SessionBase)
                ? (ulong)0
                // Recursively get all base typse ensureing that our all base types
                // have a key and that our hierarchy is complete.
                : GetIdentifier(baseType).Hierarchy;

            // get the current session-only type key
            ulong sessionOnly = nextIdentifierToBeIssued;
            nextIdentifierToBeIssued <<= 1;

            // 'or' the base type key with the current key
            ulong identifier = baseTypeKey | sessionOnly;

            // create a session identifier
            var sessionIdentifier = new SessionIdentifier()
            {
                sessionOnly = sessionOnly,
                Hierarchy = identifier,
            };

            // cache the session identifier for future lookups.
            sessionIdentifiers.Add(sessionType, sessionIdentifier);

            // return the identifier
            return sessionIdentifier;
        }
        #endregion

        #region Notification Methods
        /// <summary>
        /// PostsessionCreated is called at the end of the session constructor.
        /// </summary>
        /// <param name="session">The session that was constructed.</param>
        /// <remarks>
        /// This shall be called by the proxy code via OnConstructed. The purpose of 
        /// this is to alert the system that a session was created. This allows 
        /// sessionTraitStores that are listening for a session by type to hook
        /// that which needs to be hooked. 
        /// </remarks>
        public static void PostSessionCreated(SessionBase session, Action<ListenerBuilder> buildListeners)
        {
            Sessions.Add(session);

            // hook handlers when types match
            foreach (var listener in FindMatchingListeners(session))
                listener.HookSession(session, false);

            RegisterListener(session, buildListeners);
        }

        /// <summary>
        /// PostsessionClosed is called at the end of the session closed sequence.
        /// </summary>
        /// <param name="session">The session that was closed.</param>
        /// <remarks>
        /// This shall be called by the proxy code via OnClosed. The purpose of 
        /// this is to alert the system that a session was closed. This allows 
        /// sessionTraitStores that are listening for a session by type to hook
        /// that which needs to be hooked. 
        /// </remarks>
        public static void PostSessionClosed(SessionBase session)
        {
            UnregisterListener(session);

            // unhook handlers when types match
            foreach (var listener in FindMatchingListeners(session))
                listener.UnhookSession(session, false);

            Sessions.Remove(session);
        }
        #endregion

        // TODO: Make these internal
        public static void RemovePathHandler(object listeningObject, string propertyName)
        {
            foreach (var listener in GetSessionPropertyListeners(listeningObject, propertyName))
            {
                UnregisterPathListener(listener);
                listener.PathInfo.Rootsession = null;
            }
        }

        // TODO: Make these internal
        public static void AddPathHandler(object listeningObject, string propertyName, SessionBase session)
        {
            foreach (var listener in GetSessionPropertyListeners(listeningObject, propertyName))
            {
                var pathInfo = listener.PathInfo;
                pathInfo.Rootsession = session;

                RegisterPathListener(session, listener);
            }
        }

        private static IEnumerable<SessionListener> GetSessionPropertyListeners(object listeningObject, string propertyName)
        {
            return from listener in listenerLookup[listeningObject]
                   where listener.UsePath && listener.PathInfo.PropertyName == propertyName
                   select listener;
        }

        internal static void RegisterPathListener(SessionBase session, SessionListener listener)
        {
            if (listener.PathInfo.IsDirect)
                listener.HookSession(session, false);
            else
                // Assign it to the appropriate TraitStore
                session.RegisterSessionListenerOnTrait(listener);
        }
        private static void UnregisterPathListener(SessionListener listener)
        {
            var pathInfo = listener.PathInfo;
            var session = pathInfo.Rootsession;
            if (pathInfo.IsDirect)
                listener.UnhookSession(session, false);
            else
                session.UnregisterSessionListenerOnTrait(listener);
        }

        internal static void RegisterTypeListener(SessionListener listener)
        {
            // Get a single bit number representing the session type.
            ulong key = GetIdentifier(listener.BroadcastingType).sessionOnly;

            // Look for existing sessions that match this type.
            foreach (var session in Sessions)
                if (listener.BroadcastingType.GetTypeInfo().IsAssignableFrom(session.GetType().GetTypeInfo())
                    && listener.Matchsession(session))
                {
                    listener.HookSession(session, false);
                }

            // Ensure that this type is in the listener table.
            if (!listenerTypeLookup.ContainsKey(key))
                listenerTypeLookup[key] = new List<SessionListener>();

            // Add the listener to the lookup table. 
            listenerTypeLookup[key].Add(listener);
        }

        internal static void UnregisterTypeListener(SessionListener listener)
        {
            // Get a single bit number representing the session type.
            ulong key = GetIdentifier(listener.BroadcastingType).sessionOnly;

            listener.Unhooksessions();

            var listeners = listenerTypeLookup[key];
            listeners.Remove(listener);
        }

        #region Registration Methods
        //public static void RegisterListener(object listeningObject)
        //{
        //    if (listenerLookup.ContainsKey(listeningObject))
        //        throw new ArgumentException("Supplied object has already been registered as a listener.", "listeningObject");

        //    var listeners = new List<SessionListener>();
        //    listenerLookup.Add(listeningObject, listeners);

        //    //
        //    // Find all methods decorated with TraitListenerAttributes
        //    //
        //    MethodInfo[] methodInfos = listeningObject.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        //    foreach (var method in methodInfos)
        //        if (method.IsDefined(typeof(SessionListenerAttribute), true))
        //        {
        //            var listenerAttribute = (SessionListenerAttribute)method.GetCustomAttributes(typeof(SessionListenerAttribute), true)[0];


        //            var listener = listenerAttribute.BroadcastingType == null
        //                ? new SessionListener(listenerAttribute.NotificationType, listeningObject, listenerAttribute.BroadcastingPath)
        //                : new SessionListener(listenerAttribute.NotificationType, listenerAttribute.BroadcastingType, listenerAttribute.Matchsession);
        //            listener.Initialize((EventHandler)Delegate.CreateDelegate(typeof(EventHandler),listeningObject, method));

        //            listeners.Add(listener);

        //            if (listener.UsePath)
        //            {
        //                var pathInfo = listener.PathInfo;
        //                if (!pathInfo.IsPathValid)
        //                    throw new ArgumentException("The path supplied to session listener was not valid.", pathInfo.PathTosession);

        //                var session = pathInfo.Rootsession;
        //                if (session != null)
        //                    RegisterPathListener(session, listener);
        //            }
        //            else
        //            {
        //                //
        //                // If we are here, we are listening for a session type to become available.
        //                //

        //                RegisterTypeListener(listener);
        //            }
        //        }
        //}


        public static void RegisterListener(object listeningObject, Action<ListenerBuilder> buildListeners) {
            if (listenerLookup.ContainsKey(listeningObject))
                throw new ArgumentException("Supplied object has already been registered as a listener.", "listeningObject");

            var listeners = new List<SessionListener>();
            listenerLookup.Add(listeningObject, listeners);


            var builder = new ListenerBuilder(listeningObject);
            buildListeners(builder);
            foreach (SessionListener listener in builder.Listeners) {
                listeners.Add(listener);

                if (listener.UsePath) {
                    var pathInfo = listener.PathInfo;
                    if (!pathInfo.IsPathValid)
                        throw new ArgumentException("The path supplied to session listener was not valid.", pathInfo.PathTosession);

                    var session = pathInfo.Rootsession;
                    if (session != null)
                        RegisterPathListener(session, listener);
                }
                else {
                    //
                    // If we are here, we are listening for a session type to become available.
                    //

                    RegisterTypeListener(listener);
                }
            }


            ////
            //// Find all methods decorated with TraitListenerAttributes
            ////
            //MethodInfo[] methodInfos = listeningObject.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //foreach (var method in methodInfos)
            //    if (method.IsDefined(typeof(SessionListenerAttribute), true)) {
            //        var listenerAttribute = (SessionListenerAttribute)method.GetCustomAttributes(typeof(SessionListenerAttribute), true)[0];


            //        var listener = listenerAttribute.BroadcastingType == null
            //            ? new SessionListener(listenerAttribute.NotificationType, listeningObject, listenerAttribute.BroadcastingPath)
            //            : new SessionListener(listenerAttribute.NotificationType, listenerAttribute.BroadcastingType, listenerAttribute.Matchsession);
            //        listener.Initialize((EventHandler)Delegate.CreateDelegate(typeof(EventHandler), listeningObject, method));

            //        listeners.Add(listener);

            //        if (listener.UsePath) {
            //            var pathInfo = listener.PathInfo;
            //            if (!pathInfo.IsPathValid)
            //                throw new ArgumentException("The path supplied to session listener was not valid.", pathInfo.PathTosession);

            //            var session = pathInfo.Rootsession;
            //            if (session != null)
            //                RegisterPathListener(session, listener);
            //        }
            //        else {
            //            //
            //            // If we are here, we are listening for a session type to become available.
            //            //

            //            RegisterTypeListener(listener);
            //        }
            //    }
        }


        public static void UnregisterListener(object listeningObject)
        {
            List<SessionListener> listeners;
            if (listenerLookup.TryGetValue(listeningObject, out listeners)) {
                foreach (var listener in listeners)
                    if (listener.UsePath) {
                        UnregisterPathListener(listener);
                    }
                    else {
                        UnregisterTypeListener(listener);
                    }
                listenerLookup.Remove(listeningObject);
            }
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Finds all matching session listeners.
        /// </summary>
        /// <param name="session">session to compare to</param>
        /// <returns>An enumerable of session listeners.</returns>
        /// <remarks>
        /// This method searches through all registered listeners looking for 
        /// those registered with this session's type as well as to this 
        /// session's ancestors' types.
        /// </remarks>
        private static IEnumerable<SessionListener> FindMatchingListeners(SessionBase session)
        {
            // 
            // listenerLookup is a Dictionary<ulong, List<sessionListener>>
            //

            // Algorithm that compares a type's ancestry key against the registered ancestor.
            Func<ulong, ulong, bool> isInAncestry
                = (ulong typeKey, ulong heirarchyKey) => ((typeKey & heirarchyKey) != 0);

            return from entry in listenerTypeLookup
                   // find all lists of listeners registered to any type in the 
                   // supplied sessions ancestry
                   where isInAncestry(entry.Key, session.TypeIdentifier.Hierarchy)
                   from listener in entry.Value
                   // find all listeners for which this session is a match.
                   where listener.Matchsession(session)
                   select listener;
        }
        #endregion
    }

    #region NotificationMessage
    // Used for testing purposes only. This is not production code. 
    public class NotificationMessage
    {
        public SessionBase session { get; private set; }
        public Guid ID { get; private set; }
        public string Message { get; private set; }
        public bool DrivingMessage { get; private set; }
        public NotificationMessage(SessionBase session, string message, bool drivingMessage)
        {
            this.session = session;
            this.ID = new Guid();
            this.Message = message;
            this.DrivingMessage = drivingMessage;
        }
    }
    #endregion
}
