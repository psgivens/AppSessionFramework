using System;
using System.Collections.Generic;
using System.Reflection;

namespace PhillipScottGivens.Library.AppSessionFramework {
    // TODO: Make this class internal
    public class SessionListener {
        #region Fields
        /// <summary>
        /// eventHandler is the delegate to hook to the event when available.
        /// </summary>
        protected internal Delegate EventHandler { get; private set; }

        /// <summary>
        /// notificationType specifies which event this listener shall hook.
        /// </summary>
        protected SessionNotificationType NotificationType { get; private set; }

        /// <summary>
        /// sessions that have been hooked.
        /// </summary>
        /// <remarks>
        /// This is used to ensure that all hooked sessions are unhooked. 
        /// </remarks>
        private List<SessionBase> _hookedSessions = new List<SessionBase>();

        internal readonly Predicate<SessionBase> Matchsession;
        #endregion

        protected bool IsHooked {
            get {
                return _hookedSessions.Count > 0;
            }
        }

        protected SessionBase[] HookedSessions { get; private set; }

        internal SessionListener(SessionNotificationType notificationType, object listeningObject, string broadcastingPath)
            : base() {
            this.NotificationType = notificationType;
            this.PathInfo = new Path(this, broadcastingPath);
            Matchsession = new Predicate<SessionBase>((session) => true);

            PathInfo.Initialize(listeningObject);
        }

        internal SessionListener(SessionNotificationType notificationType, Type broadcastingType, Predicate<SessionBase> matchsession) {
            this.NotificationType = notificationType;
            this.BroadcastingType = broadcastingType;
            Matchsession = matchsession ?? new Predicate<SessionBase>((session) => true);
        }

        internal void Initialize(Delegate eventHandler) {
            this.EventHandler = eventHandler;
        }

        #region Properties
        #region Broadcasting Type Properties
        internal Type BroadcastingType { get; private set; }
        #endregion

        #region Path Properties
        internal bool UsePath { get { return PathInfo != null; } }
        internal Path PathInfo { get; private set; }
        internal class Path {
            #region Fields
            /// <summary>
            /// Back reference to the parent listener
            /// </summary>
            private SessionListener _listener;

            /// <summary>
            /// Name of the target trait. This is used in determining if we should
            /// hook the trait changed or propagate the registration.
            /// </summary>
            /// <see cref="sessionTraitStore.PropogateListener"/>
            private string _targetName;

            // May only hold sessionPropertyStore's.
            private Stack<ISessionPropertyStore> _traitStores = new Stack<ISessionPropertyStore>();

            /// <summary>
            /// The path to the desired trait in the form of the stack.
            /// </summary>
            /// <remarks>
            /// If the path were "./ImplantLink/Hardware" the stack would look like this:
            /// Push("Hardware");Push("ImplantLink"); 
            /// This is used for validation while we push elements onto the traitStores stack
            /// that we have appropriate values. 
            /// </remarks>
            private Stack<string> _reversePath = new Stack<string>();
            #endregion

            #region Initialize and Teardown
            public Path(SessionListener listener, string pathToSession) {
                this._listener = listener;
                this.PathTosession = pathToSession;
            }
            public void Initialize(object listeningObject) {
                string[] items = PathTosession.Split('/');
                Rootsession = listeningObject as SessionBase;

                if (items.Length < 2)
                    return;
                IsPathValid = true;

                bool isListenerAsession = true;

                // If the listener is not a session, get the session to root all paths from.
                if (Rootsession == null) {
                    isListenerAsession = false;
                    //var sessionProperty = listeningObject.GetType().GetProperty(items[1], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var sessionProperty = listeningObject.GetType().GetTypeInfo().GetDeclaredProperty(items[1]);
                    Rootsession = (SessionBase)sessionProperty.GetValue(listeningObject, null);
                }

                // element 0 is '.'
                if (isListenerAsession) {
                    PropertyName = ".";
                    RootTrait = items[1];  // element 1 is the TraitName on the root session
                }
                else {
                    PropertyName = items[1]; // element 1 is the property containing a session, 
                    if (items.Length > 2)
                        RootTrait = items[2];
                    else
                        this.IsDirect = true;
                }
                //element 2 is the TraitName on the root session.

                // The last element is the session with the change handler that 
                // we would like to hook.
                this._targetName = items[items.Length - 1];

                // notice the expression 'index > 0'. This excludes the last item which is "."
                for (int index = items.Length - 1; index > 0; index--)
                    _reversePath.Push(items[index]);

                if (!isListenerAsession)
                    _reversePath.Pop();
            }
            #endregion

            #region Properties
            /// <summary>
            /// The root session. When the listening object is not a session, this 
            /// may be null until the property on the listening object is assigned. 
            /// </summary>
            internal SessionBase Rootsession { get; set; }

            /// <summary>
            /// The path supplied to the constructor of the session listener attribute.
            /// </summary>
            internal string PathTosession { get; private set; }

            /// <summary>
            /// Name of the root most trait in the path to the session.
            /// </summary>
            internal string RootTrait { get; private set; }

            /// <summary>
            /// Name of the property which holds the root session. This is only
            /// applicable if the listening object is not a session.
            /// </summary>
            internal string PropertyName { get; private set; }

            /// <summary>
            /// The trait unavailable trait that is preventing this listener from
            /// being hooked.
            /// </summary>
            internal string UnavailableTrait { get { return _reversePath.Peek(); } }


            private string _availableTrait;

            /// <summary>
            /// The last trait to become available in the path to session.
            /// </summary>
            internal string AvailableTrait { get { return _availableTrait ?? _traitStores.Peek().Name; } }

            /// <summary>
            /// true if the PathTosession variable was parsed successfully.
            /// </summary>
            internal bool IsPathValid { get; private set; }

            /// <summary>
            /// true if AvailableProperty will return a value. 
            /// </summary>
            internal bool HasAvailableTrait { get { return _traitStores.Count > 0; } }

            /// <summary>
            /// True if the path contains the session that we want to hook. This means
            /// that there are no trait availabilities to be considered.
            /// </summary>
            /// <remarks>
            /// This is only applicable when the listening object is not a session.
            /// </remarks>
            internal bool IsDirect { get; private set; }
            #endregion

            /// <summary>
            /// Uses to the trait store's name to determine if it is the
            /// target trait.
            /// </summary>
            /// <param name="trait">Trait to be considered.</param>
            /// <returns>true if trait contains the session to be hooked.</returns>
            public bool IsTargetTrait(ISessionPropertyStore trait) {
                return _targetName == trait.Name;
            }

            #region Path Utility Methods
            /// <summary>
            /// Pushes a trait store onto the path of available traits.
            /// </summary>
            /// <param name="traitStore">Trait store that has become available.</param>
            /// <remarks>
            /// This is used to build the connection between the listening session
            /// and the broadcasting session. This is only applicable when a broadcastingPath
            /// is supplied as a constructor argument.
            /// </remarks>
            internal void Push(ISessionPropertyStore traitStore) {
                string current = UnavailableTrait;
                if (traitStore.Name != current)
                    throw new ArgumentException(string.Format("Trait store '{0}' was expected. Trait store '{1}' was supplied.", current, traitStore.Name));

                _traitStores.Push(traitStore);
                _reversePath.Pop();
            }

            /// <summary>
            /// Pops a trait store from the available traits
            /// </summary>
            /// <param name="traitStore">Trait store that is no longer available.</param>
            public void Pop(ISessionPropertyStore traitStore) {
                var element = _traitStores.Pop();
                if (element != traitStore)
                    throw new ArgumentException("supplied trait store is not expected.", "traitStore");

                _reversePath.Push(element.Name);
            }

            internal void Push(DataPropertyStore traitStore) {
                System.Diagnostics.Debug.Assert(_reversePath.Count == 1, "SessionListenerPath.Expected to have target node, but does not.");
                string current = UnavailableTrait;
                if (traitStore.Name != current)
                    throw new ArgumentException(string.Format("Trait store '{0}' was expected. Trait store '{1}' was supplied.", current, traitStore.Name));

                _availableTrait = current;
                _reversePath.Pop();
            }

            internal void Pop(DataPropertyStore traitStore) {
                System.Diagnostics.Debug.Assert(_availableTrait != null);
                var element = _availableTrait;
                _availableTrait = null;
                _reversePath.Push(element);
            }
            #endregion
        }
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Hooksession hooks the approriate event on the session.
        /// </summary>
        /// <param name="session">session to be hooked</param>
        internal virtual void HookSession(SessionBase session, bool isHooked) {
            if (!isHooked) {
                switch (NotificationType) {
                    case SessionNotificationType.AvailabilityChanged:
                        session.AvailabilityChanged += (EventHandler)EventHandler;
                        break;

                    case SessionNotificationType.TraitChanged:
                        session.DataChanged += (EventHandler)EventHandler;
                        break;
                }
            }
            _hookedSessions.Add(session);
            HookedSessions = _hookedSessions.ToArray();
        }

        /// <summary>
        /// Unhooksession unhooks the appropriate event from the session.
        /// </summary>
        /// <param name="session">session to be unhooked</param>
        internal virtual void UnhookSession(SessionBase session, bool isUnhooked) {
            if (!isUnhooked) {
                switch (NotificationType) {
                    case SessionNotificationType.AvailabilityChanged:
                        session.AvailabilityChanged -= (EventHandler)EventHandler;
                        break;

                    case SessionNotificationType.TraitChanged:
                        session.DataChanged -= (EventHandler)EventHandler;
                        break;
                }
            }
            _hookedSessions.Remove(session);
            HookedSessions = _hookedSessions.ToArray();
        }

        /// <summary>
        /// Unhooks all previously hooked sessions.
        /// </summary>
        internal void Unhooksessions() {
            while (_hookedSessions.Count > 0)
                UnhookSession(_hookedSessions[0], false);
        }
        #endregion
    }

    public abstract class LinkToSession : SessionListener {
        protected LinkToSession(SessionNotificationType notificationType, object listeningObject, string broadcastingPath)
            : base(notificationType, listeningObject, broadcastingPath) { }

        protected LinkToSession(SessionNotificationType notificationType, Type broadcastingType, Predicate<SessionBase> matchsession)
            : base(notificationType, broadcastingType, matchsession) { }
    }

    // TODO: Aggregate in the sessionListener instead of deriving from it. 
    public class LinkToSession<TSession> : LinkToSession
         where TSession : SessionBase {
        #region Events
        public event EventHandler<LinkTosessionEventArgs<TSession>> SessionChanged;
        #endregion

        #region Properties
        private TSession _session;
        public TSession Session {
            get { return _session; }
            private set {
                if (_session == value)
                    return;

                if (_session != null && SessionChanged != null)
                    SessionChanged(this, new LinkTosessionEventArgs<TSession>(_session, false));

                _session = value;

                if (SessionChanged != null && value != null)
                    SessionChanged(this, new LinkTosessionEventArgs<TSession>(value, true));
            }
        }
        #endregion

        #region Constructors
        public LinkToSession(SessionBase rootsession, string broadcastingPath)
            : base(SessionNotificationType.Linked, rootsession, broadcastingPath) {
            // We initialize without a listener because we are going to hook our listeners explicitly by overriding Hooksession.
            Initialize(null);
        }

        public LinkToSession(Type broadcastingType, Predicate<SessionBase> matchsession)
            : base(SessionNotificationType.Linked, broadcastingType, matchsession) {
            // We initialize without a listener because we are going to hook our listeners explicitly by overriding Hooksession.
            Initialize(null);
        }
        #endregion

        #region Hook/Unhook sessions
        internal override void HookSession(SessionBase session, bool isHooked) {
            if (!isHooked) {
                if (IsHooked)
                    throw new InvalidOperationException("HookSessions was called multiple times for a listener that does not support it.");

                Session = (TSession)session;
            }
            base.HookSession(session, true);
        }

        /// <summary>
        /// Unhooksession unhooks the appropriate event from the session.
        /// </summary>
        /// <param name="session">session to be unhooked</param>
        internal override void UnhookSession(SessionBase session, bool isUnhooked) {
            if (!isUnhooked) {
                Session = null;
            }
            base.UnhookSession(session, true);
        }
        #endregion

        public DynamicBool CreateDynamicBoolean(DataPropertyDescriptor<bool> descriptor, bool valueWhenUnavailable) {
            return CreateStoreLink<bool>(descriptor).ToDynamicBooleanValue(valueWhenUnavailable);
        }

        public IPropertyStore<TValue> CreateStoreLink<TValue>(DataPropertyDescriptor<TValue> descriptor) {
            return new LinkedsessionStore<TSession, TValue>(this, descriptor);
        }

        public LinkToSession<TValue> Link<TValue>(DataPropertyDescriptor<TValue> descriptor)
            where TValue : SessionBase {
            var rootsession = this.PathInfo.Rootsession;
            var link = new LinkToSession<TValue>(rootsession,
                string.Format("{0}/{1}", this.PathInfo.PathTosession, descriptor.PropertyName));
            SessionManager.RegisterPathListener(rootsession, link);
            return link;
        }
    }

    #region Event Args
    public class LinkTosessionEventArgs<Tsession> : EventArgs
        where Tsession : SessionBase {
        public Tsession session { get; private set; }
        public bool IsHooking { get; private set; }

        public LinkTosessionEventArgs(Tsession session, bool isHooking) {
            this.session = session;
            this.IsHooking = isHooking;
        }
    }
    #endregion

    public abstract class LinkedSessionValue<TSession, TValue> : DynamicValue<TValue>
        where TSession : SessionBase {
        #region Properties
        protected LinkToSession<TSession> LinkToSession { get; private set; }
        protected DataPropertyDescriptor<TValue> PropertyDescriptor { get; private set; }
        #endregion

        public LinkedSessionValue(LinkToSession<TSession> linkToSession, DataPropertyDescriptor<TValue> descriptor) {
            this.LinkToSession = linkToSession;
            this.PropertyDescriptor = descriptor;

            this.LinkToSession.SessionChanged += SessionChanged;
        }

        protected abstract void SessionChanged(object sender, LinkTosessionEventArgs<TSession> e);
    }

    public class LinkedsessionStore<Tsession, TValue> : LinkedSessionValue<Tsession, TValue>, IPropertyStore<TValue>
        where Tsession : SessionBase {
        private IPropertyStore<TValue> propertyStore;

        public LinkedsessionStore(LinkToSession<Tsession> linkTosession, DataPropertyDescriptor<TValue> descriptor)
            : base(linkTosession, descriptor) {
            AvailabilityNotifier = new NonSessionNotifier(this);
        }

        public override TValue Value {
            get {
                return base.Value;
            }
            protected set {
                base.Value = value;
            }
        }

        public bool IsAvailable { get; private set; }

        public DualLayerNotifier AvailabilityNotifier { get; private set; }

        protected override void SessionChanged(object sender, LinkTosessionEventArgs<Tsession> e) {
            if (e.IsHooking) {
                propertyStore = e.session.GetStore<TValue>(PropertyDescriptor);
                Link(() => propertyStore.IsAvailable ? propertyStore.Value : default(TValue), propertyStore.ValueNotifier, propertyStore.AvailabilityNotifier);
            }
            else {
                var tempStore = propertyStore;
                propertyStore = null;
                Unlink(tempStore.ValueNotifier, tempStore.AvailabilityNotifier);
                Value = default(TValue);
                IsAvailable = false;
            }
        }

        protected override void DependencyChanged(object sender, EventArgs e) {
            // The null check should not be necessary. 
            IsAvailable = propertyStore != null && propertyStore.IsAvailable;
            base.DependencyChanged(sender, e);
        }
    }

    public class LinkTosessionBool<Tsession> : LinkedSessionValue<Tsession, bool>
        where Tsession : SessionBase {
        #region Fields
        private bool valueOnUnlinked;
        #endregion

        #region Constructors
        public LinkTosessionBool(LinkToSession<Tsession> linkTosession, DataPropertyDescriptor<bool> descriptor, bool valueOnUnlinked)
            : base(linkTosession, descriptor) {
            this.valueOnUnlinked = valueOnUnlinked;
        }
        #endregion

        #region session Changed Handler
        protected override void SessionChanged(object sender, LinkTosessionEventArgs<Tsession> e) {
            var propertyStore = e.session.GetStore<bool>(PropertyDescriptor);
            if (e.IsHooking) {
                Link(() => propertyStore.IsAvailable ? ((IPropertyStore<bool>)propertyStore).Value : valueOnUnlinked, propertyStore.ValueNotifier, propertyStore.AvailabilityNotifier);
            }
            else {
                Unlink(propertyStore.ValueNotifier, propertyStore.AvailabilityNotifier);
                Value = valueOnUnlinked;
            }
        }
        #endregion
    }

    #region enum sessionNotificationType
    public enum SessionNotificationType {
        AvailabilityChanged,
        TraitChanged,
        Linked,
    }
    #endregion
}
