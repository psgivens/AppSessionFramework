 

using PhillipScottGivens.Library.AppSessionFramework;
using System.Collections.Generic;
using System;


namespace Sample.Sessions {
    public partial class MainSession : SessionBase {

        #region DataPropertyDescriptor
        public static readonly DataPropertyDescriptor<bool> SampleFlagProperty;
        public static readonly DataPropertyDescriptor<bool> IsLoggedInProperty;
        public static readonly DataPropertyDescriptor<SubSession> SubSessionProperty;
        public static readonly DataPropertyDescriptor<LoginSession> LoginSessionProperty;
        #endregion

        #region OperationDescriptor
        public static readonly OperationDescriptor DoActionOperation; 
        public static readonly OperationDescriptor DoOtherActionOperation; 
        public static readonly OperationDescriptor LogOutOperation; 
        #endregion

        #region Setup and Teardown
        private static readonly IEnumerable<string> OperationNames = new string[] {
        	"DoAction",
        	"DoOtherAction",
        	"LogOut",
        };
        static MainSession(){
            SampleFlagProperty = DataPropertyDescriptor<bool>.Register("SampleFlag", typeof(MainSession));
            IsLoggedInProperty = DataPropertyDescriptor<bool>.Register("IsLoggedIn", typeof(MainSession));
            SubSessionProperty = SessionPropertyDescriptor<SubSession>.Register("SubSession", typeof(MainSession));
            LoginSessionProperty = SessionPropertyDescriptor<LoginSession>.Register("LoginSession", typeof(MainSession));
            DoActionOperation = OperationDescriptor.Register("DoAction", typeof(MainSession));
            DoOtherActionOperation = OperationDescriptor.Register("DoOtherAction", typeof(MainSession));
            LogOutOperation = OperationDescriptor.Register("LogOut", typeof(MainSession));
            OnStaticConstruction();
        }
        static partial void OnStaticConstruction();
        protected override void InitializeSession() {
            _sampleFlagStore = GetStore(SampleFlagProperty);
            _sampleFlagStore.ValueChanged += SampleFlag_ValueChanged;
            _sampleFlagStore.IsAvailableChanged += SampleFlag_IsAvailableChanged;
            _isLoggedInStore = GetStore(IsLoggedInProperty);
            _isLoggedInStore.ValueChanged += IsLoggedIn_ValueChanged;
            _isLoggedInStore.IsAvailableChanged += IsLoggedIn_IsAvailableChanged;
            _subSessionStore = GetStore(SubSessionProperty);
            _subSessionStore.ValueChanged += SubSession_ValueChanged;
            _subSessionStore.IsAvailableChanged += SubSession_IsAvailableChanged;
            _loginSessionStore = GetStore(LoginSessionProperty);
            _loginSessionStore.ValueChanged += LoginSession_ValueChanged;
            _loginSessionStore.IsAvailableChanged += LoginSession_IsAvailableChanged;
            Proxy_RegisterOperation("DoAction", () => DoAction());
            _doActionStore = GetStore(DoActionOperation);
            _doActionStore.IsAvailableChanged += DoAction_IsAvailableChanged;
            Proxy_RegisterOperation("DoOtherAction", () => DoOtherAction());
            _doOtherActionStore = GetStore(DoOtherActionOperation);
            _doOtherActionStore.IsAvailableChanged += DoOtherAction_IsAvailableChanged;
            Proxy_RegisterOperation("LogOut", () => LogOut());
            _logOutStore = GetStore(LogOutOperation);
            _logOutStore.IsAvailableChanged += LogOut_IsAvailableChanged;
        }
        #endregion
        
        #region SampleFlag
        private DataPropertyStore<bool> _sampleFlagStore;
        private EventHandler _sampleFlagChanged;
        private EventHandler _sampleFlagAvailabilityChanged;
        
        public bool SampleFlag {
        	get { return this.GetValue(SampleFlagProperty); }
        	set { this.SetValue(SampleFlagProperty, value); }
        }
        
        [NotifyConsumer]
        private void SampleFlag_ValueChanged(object sender, EventArgs e)
        {
            if (_sampleFlagChanged != null)
                _sampleFlagChanged(this, e);
        }
        [NotifyConsumer]
        private void SampleFlag_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_sampleFlagAvailabilityChanged != null)
                _sampleFlagAvailabilityChanged(this, e);
        }
        public virtual bool IsSampleFlagAvailable
        {
            get { return _sampleFlagStore.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsSampleFlagAvailable has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsSampleFlagAvailableChanged
        {
            add { _sampleFlagAvailabilityChanged += value; }
            remove { _sampleFlagAvailabilityChanged -= value; }
        }
        /// <summary>
        /// Event meant to notify the client that SampleFlag has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler SampleFlagChanged
        {
            add { _sampleFlagChanged += value; }
            remove { _sampleFlagChanged -= value; }
        }
        #endregion
        
        #region IsLoggedIn
        private DataPropertyStore<bool> _isLoggedInStore;
        private EventHandler _isLoggedInChanged;
        private EventHandler _isLoggedInAvailabilityChanged;
        
        public bool IsLoggedIn {
        	get { return this.GetValue(IsLoggedInProperty); }
        	private set { this.SetValue(IsLoggedInProperty, value); }
        }
        
        [NotifyConsumer]
        private void IsLoggedIn_ValueChanged(object sender, EventArgs e)
        {
            if (_isLoggedInChanged != null)
                _isLoggedInChanged(this, e);
        }
        [NotifyConsumer]
        private void IsLoggedIn_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_isLoggedInAvailabilityChanged != null)
                _isLoggedInAvailabilityChanged(this, e);
        }
        public virtual bool IsIsLoggedInAvailable
        {
            get { return _isLoggedInStore.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsIsLoggedInAvailable has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsIsLoggedInAvailableChanged
        {
            add { _isLoggedInAvailabilityChanged += value; }
            remove { _isLoggedInAvailabilityChanged -= value; }
        }
        /// <summary>
        /// Event meant to notify the client that IsLoggedIn has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsLoggedInChanged
        {
            add { _isLoggedInChanged += value; }
            remove { _isLoggedInChanged -= value; }
        }
        #endregion
        
        #region SubSession
        private DataPropertyStore<SubSession> _subSessionStore;
        private EventHandler _subSessionChanged;
        private EventHandler _subSessionAvailabilityChanged;
        
        // Stub:public SubSession SubSession {
        // Stub:	get { return this.GetValue(SubSessionProperty); }
        // Stub:	set { this.SetValue(SubSessionProperty, value); }
        // Stub:}
        
        [NotifyConsumer]
        private void SubSession_ValueChanged(object sender, EventArgs e)
        {
            if (_subSessionChanged != null)
                _subSessionChanged(this, e);
        }
        [NotifyConsumer]
        private void SubSession_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_subSessionAvailabilityChanged != null)
                _subSessionAvailabilityChanged(this, e);
        }
        public virtual bool IsSubSessionAvailable
        {
            get { return _subSessionStore.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsSubSessionAvailable has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsSubSessionAvailableChanged
        {
            add { _subSessionAvailabilityChanged += value; }
            remove { _subSessionAvailabilityChanged -= value; }
        }
        /// <summary>
        /// Event meant to notify the client that SubSession has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler SubSessionChanged
        {
            add { _subSessionChanged += value; }
            remove { _subSessionChanged -= value; }
        }
        #endregion
        
        #region LoginSession
        private DataPropertyStore<LoginSession> _loginSessionStore;
        private EventHandler _loginSessionChanged;
        private EventHandler _loginSessionAvailabilityChanged;
        
        // Stub:public LoginSession LoginSession {
        // Stub:	get { return this.GetValue(LoginSessionProperty); }
        // Stub:	set { this.SetValue(LoginSessionProperty, value); }
        // Stub:}
        
        [NotifyConsumer]
        private void LoginSession_ValueChanged(object sender, EventArgs e)
        {
            if (_loginSessionChanged != null)
                _loginSessionChanged(this, e);
        }
        [NotifyConsumer]
        private void LoginSession_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_loginSessionAvailabilityChanged != null)
                _loginSessionAvailabilityChanged(this, e);
        }
        public virtual bool IsLoginSessionAvailable
        {
            get { return _loginSessionStore.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsLoginSessionAvailable has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsLoginSessionAvailableChanged
        {
            add { _loginSessionAvailabilityChanged += value; }
            remove { _loginSessionAvailabilityChanged -= value; }
        }
        /// <summary>
        /// Event meant to notify the client that LoginSession has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler LoginSessionChanged
        {
            add { _loginSessionChanged += value; }
            remove { _loginSessionChanged -= value; }
        }
        #endregion
        
        #region DoAction
        private Operation _doActionStore;
        private EventHandler _doActionAvailabilityChanged;
        
        [NotifyConsumer]
        private void DoAction_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_doActionAvailabilityChanged != null)
                _doActionAvailabilityChanged(this, e);
        }
        public virtual bool IsDoActionAvailable
        {
            get { return _doActionStore.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsDoActionAvailable has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsDoActionAvailableChanged
        {
            add { _doActionAvailabilityChanged += value; }
            remove { _doActionAvailabilityChanged -= value; }
        }
        #endregion
        
        #region DoOtherAction
        private Operation _doOtherActionStore;
        private EventHandler _doOtherActionAvailabilityChanged;
        
        [NotifyConsumer]
        private void DoOtherAction_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_doOtherActionAvailabilityChanged != null)
                _doOtherActionAvailabilityChanged(this, e);
        }
        public virtual bool IsDoOtherActionAvailable
        {
            get { return _doOtherActionStore.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsDoOtherActionAvailable has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsDoOtherActionAvailableChanged
        {
            add { _doOtherActionAvailabilityChanged += value; }
            remove { _doOtherActionAvailabilityChanged -= value; }
        }
        #endregion
        
        #region LogOut
        private Operation _logOutStore;
        private EventHandler _logOutAvailabilityChanged;
        
        [NotifyConsumer]
        private void LogOut_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_logOutAvailabilityChanged != null)
                _logOutAvailabilityChanged(this, e);
        }
        public virtual bool IsLogOutAvailable
        {
            get { return _logOutStore.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsLogOutAvailable has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsLogOutAvailableChanged
        {
            add { _logOutAvailabilityChanged += value; }
            remove { _logOutAvailabilityChanged -= value; }
        }
        #endregion
    }
}
