 

using PhillipScottGivens.Library.AppSessionFramework;
using System.Collections.Generic;
using System;


namespace Sample.Sessions {
    public partial class SubSession : SessionBase {

        #region DataPropertyDescriptor
        public static readonly DataPropertyDescriptor<bool> MyFlagProperty;
        #endregion

        #region OperationDescriptor
        public static readonly OperationDescriptor DoSubActionOperation; 
        public static readonly OperationDescriptor DoOtherSubActionOperation; 
        #endregion

        #region Setup and Teardown
        private static readonly IEnumerable<string> OperationNames = new string[] {
        	"DoSubAction",
        	"DoOtherSubAction",
        };
        static SubSession(){
            MyFlagProperty = DataPropertyDescriptor<bool>.Register("MyFlag", typeof(SubSession));
            DoSubActionOperation = OperationDescriptor.Register("DoSubAction", typeof(SubSession));
            DoOtherSubActionOperation = OperationDescriptor.Register("DoOtherSubAction", typeof(SubSession));
            OnStaticConstruction();
        }
        static partial void OnStaticConstruction();
        protected override void InitializeSession() {
            _myFlagStore = GetStore(MyFlagProperty);
            _myFlagStore.ValueChanged += MyFlag_ValueChanged;
            _myFlagStore.IsAvailableChanged += MyFlag_IsAvailableChanged;
            Proxy_RegisterOperation("DoSubAction", () => DoSubAction());
            _doSubActionStore = GetStore(DoSubActionOperation);
            _doSubActionStore.IsAvailableChanged += DoSubAction_IsAvailableChanged;
            Proxy_RegisterOperation("DoOtherSubAction", () => DoOtherSubAction());
            _doOtherSubActionStore = GetStore(DoOtherSubActionOperation);
            _doOtherSubActionStore.IsAvailableChanged += DoOtherSubAction_IsAvailableChanged;
        }
        #endregion
        
        #region MyFlag
        private DataPropertyStore<bool> _myFlagStore;
        private EventHandler _myFlagChanged;
        private EventHandler _myFlagAvailabilityChanged;
        
        public bool MyFlag {
        	get { return this.GetValue(MyFlagProperty); }
        	set { this.SetValue(MyFlagProperty, value); }
        }
        
        [NotifyConsumer]
        private void MyFlag_ValueChanged(object sender, EventArgs e)
        {
            if (_myFlagChanged != null)
                _myFlagChanged(this, e);
        }
        [NotifyConsumer]
        private void MyFlag_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_myFlagAvailabilityChanged != null)
                _myFlagAvailabilityChanged(this, e);
        }
        public virtual bool IsMyFlagAvailable
        {
            get { return _myFlagStore.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsMyFlagAvailable has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsMyFlagAvailableChanged
        {
            add { _myFlagAvailabilityChanged += value; }
            remove { _myFlagAvailabilityChanged -= value; }
        }
        /// <summary>
        /// Event meant to notify the client that MyFlag has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler MyFlagChanged
        {
            add { _myFlagChanged += value; }
            remove { _myFlagChanged -= value; }
        }
        #endregion
        
        #region DoSubAction
        private Operation _doSubActionStore;
        private EventHandler _doSubActionAvailabilityChanged;
        
        [NotifyConsumer]
        private void DoSubAction_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_doSubActionAvailabilityChanged != null)
                _doSubActionAvailabilityChanged(this, e);
        }
        public virtual bool IsDoSubActionAvailable
        {
            get { return _doSubActionStore.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsDoSubActionAvailable has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsDoSubActionAvailableChanged
        {
            add { _doSubActionAvailabilityChanged += value; }
            remove { _doSubActionAvailabilityChanged -= value; }
        }
        #endregion
        
        #region DoOtherSubAction
        private Operation _doOtherSubActionStore;
        private EventHandler _doOtherSubActionAvailabilityChanged;
        
        [NotifyConsumer]
        private void DoOtherSubAction_IsAvailableChanged(object sender, EventArgs e)
        {
            if (_doOtherSubActionAvailabilityChanged != null)
                _doOtherSubActionAvailabilityChanged(this, e);
        }
        public virtual bool IsDoOtherSubActionAvailable
        {
            get { return _doOtherSubActionStore.IsAvailable; }
        }
        /// <summary>
        /// Event meant to notify the client that IsDoOtherSubActionAvailable has changed. 
        /// </summary>
        /// <remarks>
        /// This event will not be raised until the stack is unwinding from the application layer. 
        /// </remarks>
        public event EventHandler IsDoOtherSubActionAvailableChanged
        {
            add { _doOtherSubActionAvailabilityChanged += value; }
            remove { _doOtherSubActionAvailabilityChanged -= value; }
        }
        #endregion
    }
}
