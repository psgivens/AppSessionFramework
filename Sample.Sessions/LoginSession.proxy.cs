 

using PhillipScottGivens.Library.AppSessionFramework;
using System.Collections.Generic;
using System;


namespace Sample.Sessions {
    public partial class LoginSession : SessionBase {

        #region DataPropertyDescriptor
        public static readonly DataPropertyDescriptor<string> UserNameProperty;
        public static readonly DataPropertyDescriptor<string> PasswordProperty;
        public static readonly DataPropertyDescriptor<string> NewUserNameProperty;
        public static readonly DataPropertyDescriptor<string> NewPassword1Property;
        public static readonly DataPropertyDescriptor<string> NewPassword2Property;
        #endregion

        #region OperationDescriptor
        public static readonly OperationDescriptor LoginOperation;
        public static readonly OperationDescriptor RegisterOperation;
        #endregion

        #region Setup and Teardown
        private static readonly IEnumerable<string> OperationNames = new string[] {
        	"Login",
        	"Register",
        };
        static LoginSession(){
            UserNameProperty = DataPropertyDescriptor<string>.Register("UserName", typeof(LoginSession));
            PasswordProperty = DataPropertyDescriptor<string>.Register("Password", typeof(LoginSession));
            NewUserNameProperty = DataPropertyDescriptor<string>.Register("NewUserName", typeof(LoginSession));
            NewPassword1Property = DataPropertyDescriptor<string>.Register("NewPassword1", typeof(LoginSession));
            NewPassword2Property = DataPropertyDescriptor<string>.Register("NewPassword2", typeof(LoginSession));
            LoginOperation = OperationDescriptor.Register("Login", typeof(LoginSession));
            RegisterOperation = OperationDescriptor.Register("Register", typeof(LoginSession));
            OnStaticConstruction();
        }
        static partial void OnStaticConstruction();
        protected override void InitializeSession() {
            _userNameStore = GetStore(UserNameProperty);
            _userNameStore.ValueChanged += UserName_ValueChanged;
            _userNameStore.IsAvailableChanged += UserName_IsAvailableChanged;
            _passwordStore = GetStore(PasswordProperty);
            _passwordStore.ValueChanged += Password_ValueChanged;
            _passwordStore.IsAvailableChanged += Password_IsAvailableChanged;
            _newUserNameStore = GetStore(NewUserNameProperty);
            _newUserNameStore.ValueChanged += NewUserName_ValueChanged;
            _newUserNameStore.IsAvailableChanged += NewUserName_IsAvailableChanged;
            _newPassword1Store = GetStore(NewPassword1Property);
            _newPassword1Store.ValueChanged += NewPassword1_ValueChanged;
            _newPassword1Store.IsAvailableChanged += NewPassword1_IsAvailableChanged;
            _newPassword2Store = GetStore(NewPassword2Property);
            _newPassword2Store.ValueChanged += NewPassword2_ValueChanged;
            _newPassword2Store.IsAvailableChanged += NewPassword2_IsAvailableChanged;
            Proxy_RegisterOperation("Login", () => Login());
            _loginStore = GetStore(LoginOperation);
            _loginStore.IsAvailableChanged += Login_IsAvailableChanged;
            Proxy_RegisterOperation("Register", () => Register());
            _registerStore = GetStore(RegisterOperation);
            _registerStore.IsAvailableChanged += Register_IsAvailableChanged;
        }
        #endregion
        
        #region UserName
        private DataPropertyStore<string> _userNameStore;
        private EventHandler _userNameChanged;
        private EventHandler _userNameAvailabilityChanged;
        
        public string UserName {
        	get { return this.GetValue(UserNameProperty); }
        	set { this.SetValue(UserNameProperty, value); }
        }
        
        [NotifyConsumer]
        private void UserName_ValueChanged(object sender, EventArgs e)
        {
            if (_userNameChanged != null)
                _userNameChanged(this, e);
        }
        [NotifyConsumer]
        private void UserName_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_userNameAvailabilityChanged != null)
                _userNameAvailabilityChanged(this, e);
        }
        public virtual bool IsUserNameAvailable
        {
            get { return _userNameStore.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsUserNameAvailable has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsUserNameAvailableChanged
        {
            add { _userNameAvailabilityChanged += value; }
            remove { _userNameAvailabilityChanged -= value; }
        }
        /// <summary>
        /// Event meant to notify the client that UserName has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler UserNameChanged
        {
            add { _userNameChanged += value; }
            remove { _userNameChanged -= value; }
        }
        #endregion
        
        #region Password
        private DataPropertyStore<string> _passwordStore;
        private EventHandler _passwordChanged;
        private EventHandler _passwordAvailabilityChanged;
        
        public string Password {
        	get { return this.GetValue(PasswordProperty); }
        	set { this.SetValue(PasswordProperty, value); }
        }
        
        [NotifyConsumer]
        private void Password_ValueChanged(object sender, EventArgs e)
        {
            if (_passwordChanged != null)
                _passwordChanged(this, e);
        }
        [NotifyConsumer]
        private void Password_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_passwordAvailabilityChanged != null)
                _passwordAvailabilityChanged(this, e);
        }
        public virtual bool IsPasswordAvailable
        {
            get { return _passwordStore.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsPasswordAvailable has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsPasswordAvailableChanged
        {
            add { _passwordAvailabilityChanged += value; }
            remove { _passwordAvailabilityChanged -= value; }
        }
        /// <summary>
        /// Event meant to notify the client that Password has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler PasswordChanged
        {
            add { _passwordChanged += value; }
            remove { _passwordChanged -= value; }
        }
        #endregion
        
        #region NewUserName
        private DataPropertyStore<string> _newUserNameStore;
        private EventHandler _newUserNameChanged;
        private EventHandler _newUserNameAvailabilityChanged;
        
        public string NewUserName {
        	get { return this.GetValue(NewUserNameProperty); }
        	set { this.SetValue(NewUserNameProperty, value); }
        }
        
        [NotifyConsumer]
        private void NewUserName_ValueChanged(object sender, EventArgs e)
        {
            if (_newUserNameChanged != null)
                _newUserNameChanged(this, e);
        }
        [NotifyConsumer]
        private void NewUserName_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_newUserNameAvailabilityChanged != null)
                _newUserNameAvailabilityChanged(this, e);
        }
        public virtual bool IsNewUserNameAvailable
        {
            get { return _newUserNameStore.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsNewUserNameAvailable has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsNewUserNameAvailableChanged
        {
            add { _newUserNameAvailabilityChanged += value; }
            remove { _newUserNameAvailabilityChanged -= value; }
        }
        /// <summary>
        /// Event meant to notify the client that NewUserName has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler NewUserNameChanged
        {
            add { _newUserNameChanged += value; }
            remove { _newUserNameChanged -= value; }
        }
        #endregion
        
        #region NewPassword1
        private DataPropertyStore<string> _newPassword1Store;
        private EventHandler _newPassword1Changed;
        private EventHandler _newPassword1AvailabilityChanged;
        
        public string NewPassword1 {
        	get { return this.GetValue(NewPassword1Property); }
        	set { this.SetValue(NewPassword1Property, value); }
        }
        
        [NotifyConsumer]
        private void NewPassword1_ValueChanged(object sender, EventArgs e)
        {
            if (_newPassword1Changed != null)
                _newPassword1Changed(this, e);
        }
        [NotifyConsumer]
        private void NewPassword1_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_newPassword1AvailabilityChanged != null)
                _newPassword1AvailabilityChanged(this, e);
        }
        public virtual bool IsNewPassword1Available
        {
            get { return _newPassword1Store.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsNewPassword1Available has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsNewPassword1AvailableChanged
        {
            add { _newPassword1AvailabilityChanged += value; }
            remove { _newPassword1AvailabilityChanged -= value; }
        }
        /// <summary>
        /// Event meant to notify the client that NewPassword1 has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler NewPassword1Changed
        {
            add { _newPassword1Changed += value; }
            remove { _newPassword1Changed -= value; }
        }
        #endregion
        
        #region NewPassword2
        private DataPropertyStore<string> _newPassword2Store;
        private EventHandler _newPassword2Changed;
        private EventHandler _newPassword2AvailabilityChanged;
        
        public string NewPassword2 {
        	get { return this.GetValue(NewPassword2Property); }
        	set { this.SetValue(NewPassword2Property, value); }
        }
        
        [NotifyConsumer]
        private void NewPassword2_ValueChanged(object sender, EventArgs e)
        {
            if (_newPassword2Changed != null)
                _newPassword2Changed(this, e);
        }
        [NotifyConsumer]
        private void NewPassword2_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_newPassword2AvailabilityChanged != null)
                _newPassword2AvailabilityChanged(this, e);
        }
        public virtual bool IsNewPassword2Available
        {
            get { return _newPassword2Store.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsNewPassword2Available has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsNewPassword2AvailableChanged
        {
            add { _newPassword2AvailabilityChanged += value; }
            remove { _newPassword2AvailabilityChanged -= value; }
        }
        /// <summary>
        /// Event meant to notify the client that NewPassword2 has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler NewPassword2Changed
        {
            add { _newPassword2Changed += value; }
            remove { _newPassword2Changed -= value; }
        }
        #endregion
        
        #region Login
        private Operation _loginStore;
        private EventHandler _loginAvailabilityChanged;
        
        [NotifyConsumer]
        private void Login_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_loginAvailabilityChanged != null)
                _loginAvailabilityChanged(this, e);
        }
        public virtual bool IsLoginAvailable
        {
            get { return _loginStore.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsLoginAvailable has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsLoginAvailableChanged
        {
            add { _loginAvailabilityChanged += value; }
            remove { _loginAvailabilityChanged -= value; }
        }
        #endregion
        
        #region Register
        private Operation _registerStore;
        private EventHandler _registerAvailabilityChanged;
        
        [NotifyConsumer]
        private void Register_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_registerAvailabilityChanged != null)
                _registerAvailabilityChanged(this, e);
        }
        public virtual bool IsRegisterAvailable
        {
            get { return _registerStore.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsRegisterAvailable has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsRegisterAvailableChanged
        {
            add { _registerAvailabilityChanged += value; }
            remove { _registerAvailabilityChanged -= value; }
        }
        #endregion
    }
}
