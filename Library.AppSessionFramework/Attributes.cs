using System;
using System.Collections.Generic;

namespace PhillipScottGivens.Library.AppSessionFramework
{
    #region attributes SessionListener
    /// <summary>
    /// sessionListenerAttribute is used to mark a method as
    /// a listener for a session change event.
    /// </summary>
    /// <remarks>
    /// Some of the methods on this class only pertain to listening
    /// to a session designated by a path. Other methods only pertain
    /// to listening to a session designated by type. In a pure world, 
    /// this class 
    /// </remarks>
    public class SessionListenerAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// private .ctor that takes a notification type.
        /// </summary>
        /// <param name="notificationType">Type of event to hook</param>
        private SessionListenerAttribute(SessionNotificationType notificationType)
        {
            NotificationType = notificationType;
        }

        /// <summary>
        /// .ctor that uses a broadcasting type.
        /// </summary>
        /// <param name="notificationType">Type of event to hook</param>
        /// <param name="broadcastingType">Type of session to hook when it becomes available.</param>
        /// <remarks>
        /// When this constructor is used, if the session of type broadcastingType and Matchsession returns true,
        /// the broadcasting session shall be hooked whenever either the broadcasting or listening session is created.
        /// </remarks>
        public SessionListenerAttribute(SessionNotificationType notificationType, Type broadcastingType)
            : this(notificationType)
        {
            BroadcastingType = broadcastingType;
        }

        /// <summary>
        /// .ctor that uses a path to determine which session to hook.
        /// </summary>
        /// <param name="notificationType">Type of event to hook</param>
        /// <param name="broadcastingPath">Path describing which session to hook.</param>
        /// <remarks>
        /// When this constructor is used, the path will be built up as the traits that contain sessions
        /// become available. When the full path has been built, the approriate event will be hooked. 
        /// </remarks>
        public SessionListenerAttribute(SessionNotificationType notificationType, string broadcastingPath)
            : this(notificationType)
        {
            BroadcastingPath = broadcastingPath;
            //this.PathInfo = new Path(this, broadcastingPath);
        }
        #endregion

        public readonly SessionNotificationType NotificationType;
        public readonly Type BroadcastingType;
        public readonly string BroadcastingPath;

        #region Broadcasting Type Properties
        /// <summary>
        /// Matchsession allows derived classes to use additional criteria 
        /// in determining which sessions to hook.
        /// </summary>
        /// <param name="session">session to check against.</param>
        /// <returns>true if this session shall be hooked.</returns>
        /// <remarks>
        /// This is only applicable when a broadcasting type is supplied as a 
        /// constructor argument. 
        /// </remarks>
        protected internal virtual bool Matchsession(SessionBase session)
        {
            return true;
        }
        #endregion
    }
    #endregion

    #region attribute TraitListener
    public class TraitListenerAttribute : SessionListenerAttribute
    {
        #region Constructors
        public TraitListenerAttribute(Type broadcastingType)
            : base(SessionNotificationType.TraitChanged, broadcastingType)
        {
        }

        public TraitListenerAttribute(string broadcastingPath)
            : base(SessionNotificationType.TraitChanged, broadcastingPath)
        {
        }
        #endregion
    }
    #endregion

    #region attribute AvailabilityListener
    public class AvailabilityListenerAttribute : SessionListenerAttribute
    {
        #region Constructors
        public AvailabilityListenerAttribute(Type broadcastingType)
            : base(SessionNotificationType.AvailabilityChanged, broadcastingType)
        {
        }

        public AvailabilityListenerAttribute(string broadcastingPath)
            : base(SessionNotificationType.AvailabilityChanged, broadcastingPath)
        {
        }
        #endregion
    }
    #endregion

    // TECHNOTE: Using Attributes
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AbstractBaseSessionAttribute : Attribute
    {
    }



    // TODO: Break this into an object representing listener for the session to become available, and ...
    // an object responsible for hooking the remote property stores. 
    internal class SessionListener<TValue> : SessionListener
    {
        #region Fields
        private DataPropertyDescriptor<TValue> dataPropertyDescriptor;
        private bool allowMultipleHooks;
        #endregion

        #region Properties
        public virtual TValue Value
        {
            get;
            protected set;
        }
        public virtual bool IsAvailable
        {
            get;
            protected set;
        }
        #endregion

        #region Constructors
        public SessionListener(SessionNotificationType notificationType, SessionBase rootsession, string broadcastingPath, DataPropertyDescriptor<TValue> descriptor)
            : base(notificationType, rootsession, broadcastingPath)
        {
            this.dataPropertyDescriptor = descriptor;

            // When specifying a path, there shall never be two sessions hooked. 
            this.allowMultipleHooks = false;

            // We initialize without a listener because we are going to hook our listeners explicitly by overriding Hooksession.
            Initialize(null);
        }

        public SessionListener(SessionNotificationType notificationType, Type broadcastingType, Predicate<SessionBase> matchsession, DataPropertyDescriptor<TValue> descriptor)
            : this(notificationType, false, broadcastingType, matchsession, descriptor)
        {
        }

        protected SessionListener(SessionNotificationType notificationType, bool allowMultipleHooks, Type broadcastingType, Predicate<SessionBase> matchsession, DataPropertyDescriptor<TValue> descriptor)
            : base(notificationType, broadcastingType, matchsession)
        {
            this.dataPropertyDescriptor = descriptor;

            // We initialize without a listener because we are going to hook our listeners explicitly by overriding Hooksession.
            Initialize(null);
        }
        #endregion

        #region Hook/Unhook sessions
        internal override void HookSession(SessionBase session, bool isHooked)
        {
            if (!isHooked)
            {
                if (!allowMultipleHooks && IsHooked)
                    throw new InvalidOperationException("Hooksessions was called multiple times for a listener that does not support it.");

                // TODO: Handle case where we hook multiple sessions. 
                var propertyStore = session.GetStore<TValue>(this.dataPropertyDescriptor);

                switch (NotificationType)
                {
                    case SessionNotificationType.AvailabilityChanged:
                        propertyStore.AvailabilityNotifier.ChangeNotificationSent += Property_AvailabilityChanged;
                        break;

                    case SessionNotificationType.TraitChanged:
                        propertyStore.ValueNotifier.ChangeNotificationSent += Property_ValueChanged;
                        propertyStore.AvailabilityNotifier.ChangeNotificationSent += Property_AvailabilityChanged;
                        break;
                }

                propertyStores.Add(propertyStore);
            }
            base.HookSession(session, true);

            if (!IsHooked)
                IsAvailable = false;
        }

        /// <summary>
        /// Unhooksession unhooks the appropriate event from the session.
        /// </summary>
        /// <param name="session">session to be unhooked</param>
        internal override void UnhookSession(SessionBase session, bool isUnhooked)
        {
            if (!isUnhooked)
            {
                var propertyStore = session.GetStore<TValue>(dataPropertyDescriptor);
                switch (NotificationType)
                {
                    case SessionNotificationType.AvailabilityChanged:
                        propertyStore.AvailabilityNotifier.ChangeNotificationSent -= Property_AvailabilityChanged;
                        break;

                    case SessionNotificationType.TraitChanged:
                        propertyStore.ValueNotifier.ChangeNotificationSent -= Property_ValueChanged;
                        propertyStore.AvailabilityNotifier.ChangeNotificationSent -= Property_AvailabilityChanged;
                        break;
                }

                propertyStores.Remove(propertyStore);
            }
            base.UnhookSession(session, true);

            if (!IsHooked)
                IsAvailable = false;
        }
        #endregion

        private List<DataPropertyStore<TValue>> propertyStores = new List<DataPropertyStore<TValue>>();

        protected virtual void Property_ValueChanged(object sender, EventArgs e)
        {
            if (allowMultipleHooks)
                throw new InvalidOperationException("Property_ValueChanged cannot handle multiple values of type TValue.");

            var propertyStore = (DynamicValue<TValue>)sender;
            Value = propertyStore.Value;
        }

        protected virtual void Property_AvailabilityChanged(object sender, EventArgs e)
        {
            bool isAvailable = false;
            foreach (var propertyStore in propertyStores)
                if (isAvailable |= propertyStore.IsAvailable)
                    break;

            IsAvailable = isAvailable;
        }
    }

    internal class sessionBooleanListener : SessionListener<bool>
    {
        #region Constructors
        public sessionBooleanListener(SessionNotificationType notificationType, SessionBase rootsession, string broadcastingPath, DataPropertyDescriptor<bool> descriptor)
            : base(notificationType, rootsession, broadcastingPath, descriptor)
        {
        }
        #endregion
    }
}
