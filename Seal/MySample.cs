 

using PhillipScottGivens.Library.AppSessionFramework;
using System;

// This is the output code from your template
// you only get syntax-highlighting here - not intellisense
namespace PhillipScottGivens.Seal{
    public partial class MyGeneratedClass: SessionBase {

        #region DataPropertyDescriptor
        public static readonly DataPropertyDescriptor<bool> SampleFlagProperty; 
        #endregion

        #region Setup and Teardown
        static MyGeneratedClass(){
            SampleFlagProperty = DataPropertyDescriptor<bool>.Register("SampleFlag", typeof(MyGeneratedClass));
            OnStaticConstruction();
        }
        static partial void OnStaticConstruction();
        protected virtual void InitializeSession(){
            _sampleFlagStore = GetStore<bool>(SampleFlagProperty);
            _sampleFlagStore.ValueChanged += SampleFlag_ValueChanged;
            _sampleFlagStore.IsAvailableChanged += SampleFlag_IsAvailableChanged;
        }
        #endregion
        
        #region SampleFlag
        private DataPropertyStore _sampleFlagStore;
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
    }

	public partial class MyApplicationSession : SessionBase {

        #region DataPropertyDescriptor
        public static readonly DataPropertyDescriptor<bool> SampleFlagProperty; 
        #endregion

        #region Setup and Teardown
        static MyApplicationSession(){
            SampleFlagProperty = DataPropertyDescriptor<bool>.Register("SampleFlag", typeof(MyApplicationSession));
            OnStaticConstruction();
        }
        static partial void OnStaticConstruction();
        protected virtual void InitializeSession(){
            _sampleFlagStore = GetStore<bool>(SampleFlagProperty);
            _sampleFlagStore.ValueChanged += SampleFlag_ValueChanged;
            _sampleFlagStore.IsAvailableChanged += SampleFlag_IsAvailableChanged;
        }
        #endregion
        
        #region SampleFlag
        private DataPropertyStore _sampleFlagStore;
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
	}
}

