using System;
using System.Collections.Generic;
using System.Linq;

namespace PhillipScottGivens.Library.AppSessionFramework {
    public abstract partial class SessionBase :// INotifyPropertyChanged, 
        IDisposable {
        #region Fields
        private Dictionary<string, Operation> _operations = new Dictionary<string, Operation>();
        private Queue<DualLayerNotifier> _dirtyNotifiers = new Queue<DualLayerNotifier>();
        private Dictionary<DataPropertyDescriptor, DataPropertyStore> _dataProperties = new Dictionary<DataPropertyDescriptor, DataPropertyStore>();
        private Dictionary<string, DataPropertyStore> _dataPropertyLookup = new Dictionary<string, DataPropertyStore>();
        private Dictionary<string, AvailabilityStore> _availabilities = new Dictionary<string, AvailabilityStore>();
        #endregion

        #region Constructors
        public SessionBase() {
            #region Data Properties
            foreach (var descriptor in DataPropertyDescriptor.GetDescriptors(GetType())) {
                DataPropertyStore dataProperty;
                _dataProperties.Add(descriptor, dataProperty = descriptor.CreateDataStore(descriptor.PropertyName));
                _dataPropertyLookup.Add(descriptor.PropertyName, dataProperty);
                dataProperty.AvailabilityNotifier.ChangeNotificationSent += AvailabilityConditionNotifier_AvailabilityConditionChanged;
                dataProperty.ValueNotifier.ChangeNotificationSent += AvailabilityConditionNotifier_AvailabilityConditionChanged;
                _availabilities.Add(descriptor.PropertyName, dataProperty);
            }
            #endregion

            InitializeSession();

            #region Code from AutoHookingSample
            TypeIdentifier = SessionManager.GetIdentifier(GetType());
            ulong key = TypeIdentifier.sessionOnly;

            ////
            //// Create sessionTraitStores
            //// In real code, this will be done in a similar fashion as DataObjectTraitStores
            ////
            //PropertyInfo[] propertyInfos = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //foreach (var property in propertyInfos)
            //    if (typeof(SessionBase).IsAssignableFrom(property.PropertyType))
            //        sessionTraitStores.Add(property.Name, new sessionTraitStore(this, property.Name));

            //SessionManager.RegisterListener(this);
            #endregion
        }

        public virtual void Dispose() {
        }

        public event EventHandler Disposed;

        protected abstract void InitializeSession();
        #endregion

        #region Data Property Utilities
        protected TValue GetValue<TValue>(DataPropertyDescriptor<TValue> descriptor) {
            return (TValue)_dataProperties[descriptor].Value;
        }

        protected void SetValue<TValue>(DataPropertyDescriptor<TValue> descriptor, TValue value) {
            Action setValue = () => {
                _dataProperties[descriptor].Value = value;
            };
            if (operationNames.Any()) {
                setValue();
            }
            else {
                SetValueAsOperation(descriptor.PropertyName, setValue);
            }
        }

        private void SetValueAsOperation(string propertyName, Action action) {

            // Cannot call OnOperationBegin(propertyName) because propertyName is not an operation.
            // Call the relevant code instead. 
            operationNames.Push(propertyName);
            SessionManager.PushOperation(this, propertyName);
            ApplicationSynchronizationContext.PushContext(this, propertyName);

            try {
                action();
                OnOperationSuccess(propertyName);
            }
            catch (Exception e) {
                OnOperationError(propertyName, e);
            }
        }

        protected BooleanPropertyStore GetBooleanStore(DataPropertyDescriptor<bool> descriptor) {
            return (BooleanPropertyStore)GetStore<bool>(descriptor);
        }

        internal protected DataPropertyStore<TValue> GetStore<TValue>(DataPropertyDescriptor<TValue> descriptor) {
            return (DataPropertyStore<TValue>)_dataProperties[descriptor];
        }

        protected Operation GetStore(OperationDescriptor descriptor) {
            return _operations[descriptor.Name];
        }
        #endregion

        #region Availability Condition Handlers
        private void AvailabilityConditionNotifier_AvailabilityConditionChanged(object sender, EventArgs e) {
            var eventArgs = e as ApplicationLayerEventArgs;
            SessionManager.MarkDirty(this);
            _dirtyNotifiers.Enqueue(eventArgs.Notifier);
        }
        #endregion

        #region Availability Condition Utilities
        protected void SetAvailabilityCalculator(OperationDescriptor operation, DynamicBool condition) {
            _availabilities[operation.Name].SetAvailabilityCondition(condition);
        }
        protected void SetAvailabilityCalculator(DataPropertyDescriptor property, DynamicBool condition) {
            _availabilities[property.PropertyName].SetAvailabilityCondition(condition);
        }
        public bool IsAvailable(OperationDescriptor operation) {
            return _availabilities[operation.Name].IsAvailable;
        }
        public DynamicBool CreateDynamicBoolean(
            Func<bool> valueCalculation,
            DualLayerNotifier firstAvailibilityConditionChanged,
            params DualLayerNotifier[] args) {
            return new DynamicBool(valueCalculation, firstAvailibilityConditionChanged, args);
        }

        public DynamicBool CreateDynamicBoolean(
            DynamicBool firstCondition,
            BooleanLogic logic,
            DynamicBool secondCondition) {
            return new DynamicBool(logic, firstCondition, secondCondition);
        }

        public DynamicBool CreateDynamicBoolean(
            BooleanLogic logic,
            DynamicBool firstCondition,
            DynamicBool secondCondition,
            params DynamicBool[] args) {
            return new DynamicBool(logic, firstCondition, secondCondition, args);
        }

        public DynamicBool CreateDynamicBoolean(DataPropertyDescriptor<Boolean> descriptor, bool valueWhenUnavailable) {
            var propertyStore = GetStore<bool>(descriptor);
            return propertyStore.ToDynamicBooleanValue(valueWhenUnavailable);
        }

        public DynamicBool CreateDynamicBoolean<TValue>(IPropertyStore<TValue> propertyStore, bool valueWhenUnavailable, Func<bool> valueWhenAvailable) {
            var gatedCondition = valueWhenUnavailable
                ? new Func<bool>(() => !propertyStore.IsAvailable || valueWhenAvailable())
                : new Func<bool>(() => propertyStore.IsAvailable && valueWhenAvailable());
            return new DynamicBool(gatedCondition, propertyStore.AvailabilityNotifier, propertyStore.ValueNotifier);
        }
        public DynamicBool CreateDynamicBoolean<TValue>(DataPropertyDescriptor<TValue> descriptor, bool valueWhenUnavailable, Predicate<TValue> valueWhenAvailable) {
            IPropertyStore<TValue> propertyStore = GetStore<TValue>(descriptor);
            var gatedCondition = valueWhenUnavailable
                ? new Func<bool>(() => !propertyStore.IsAvailable || valueWhenAvailable(propertyStore.Value))
                : new Func<bool>(() => propertyStore.IsAvailable && valueWhenAvailable(propertyStore.Value));
            return new DynamicBool(gatedCondition, propertyStore.AvailabilityNotifier, propertyStore.ValueNotifier);
        }
        public DynamicBool CreateDynamicBoolean<TValue1, TValue2>(DataPropertyDescriptor<TValue1> firstDescriptor, DataPropertyDescriptor<TValue2> secondDescriptor, bool valueWhenNotAvailable, Func<TValue1, TValue2, bool> condition) {
            IPropertyStore<TValue1> firstPropertyStore = GetStore<TValue1>(firstDescriptor);
            IPropertyStore<TValue2> secondPropertyStore = GetStore<TValue2>(secondDescriptor);
            var gatedCondition = valueWhenNotAvailable
                ? new Func<bool>(() => !firstPropertyStore.IsAvailable || !secondPropertyStore.IsAvailable || condition(firstPropertyStore.Value, secondPropertyStore.Value))
                : new Func<bool>(() => firstPropertyStore.IsAvailable && secondPropertyStore.IsAvailable && condition(firstPropertyStore.Value, secondPropertyStore.Value));
            return CreateDynamicBool<TValue1, TValue2>(gatedCondition, firstPropertyStore, secondPropertyStore);
        }
        public DynamicBool CreateDynamicBoolean<TValue1, TValue2>(DataPropertyDescriptor<TValue1> descriptor, IPropertyStore<TValue2> propertyStore, bool valueWhenUnavailable, Func<TValue1, bool> valueWhenAvailable) {
            IPropertyStore<TValue1> localPropertyStore = GetStore<TValue1>(descriptor);
            var gatedCondition = valueWhenUnavailable
                ? new Func<bool>(() => !propertyStore.IsAvailable || !localPropertyStore.IsAvailable || valueWhenAvailable(localPropertyStore.Value))
                : new Func<bool>(() => propertyStore.IsAvailable && localPropertyStore.IsAvailable && valueWhenAvailable(localPropertyStore.Value));
            return CreateDynamicBool<TValue1, TValue2>(gatedCondition, localPropertyStore, propertyStore);
        }
        public DynamicBool CreateDynamicBoolean<TValue1, TValue2>(IPropertyStore<TValue1> firstPropertyStore, IPropertyStore<TValue2> secondPropertyStore, bool valueWhenNotAvailable, Func<bool> condition) {
            var gatedCondition = valueWhenNotAvailable
                ? new Func<bool>(() => !firstPropertyStore.IsAvailable || !secondPropertyStore.IsAvailable || condition())
                : new Func<bool>(() => firstPropertyStore.IsAvailable && secondPropertyStore.IsAvailable && condition());
            return CreateDynamicBool<TValue1, TValue2>(gatedCondition, firstPropertyStore, secondPropertyStore);
        }
        private DynamicBool CreateDynamicBool<TValue1, TValue2>(Func<bool> gatedCondition, IPropertyStore<TValue1> firstPropertyStore, IPropertyStore<TValue2> secondPropertyStore) {
            return new DynamicBool(gatedCondition, firstPropertyStore.AvailabilityNotifier, firstPropertyStore.ValueNotifier, secondPropertyStore.AvailabilityNotifier, secondPropertyStore.ValueNotifier);
        }
        #endregion

        #region Operation Utilities
        public Operation this[string operationName] {
            get {
                return _operations[operationName];
            }
        }
        public IEnumerable<Operation> Operations {
            get {
                return _operations.Values;
            }
        }
        public IEnumerable<DataPropertyStore> DataProperties {
            get {
                return _dataProperties.Values;
            }
        }
        private Stack<string> operationNames = new Stack<string>();

        protected internal void ExecuteOperation(string operationName, Action operationAction, bool isCallback = false) {
            OnOperationBegin(operationName, isCallback);
            try {
                operationAction();
                OnOperationSuccess(operationName);
            }
            catch (Exception e) {
                OnOperationError(operationName, e);
            }
        }

        protected void OnOperationBegin(string operationName, bool isCallback = false) {
            if (!isCallback && !_operations[operationName].IsAvailable)
                throw new InvalidOperationException(string.Format("{0} is not available at this time.", operationName));
            if (!operationNames.Any()) {
                ApplicationSynchronizationContext.PushContext(this, operationName);
            }
            operationNames.Push(operationName);
            SessionManager.PushOperation(this, operationName);
        }
        protected void OnOperationSuccess(string operationName) {
            OnOpertionEnd(operationName);
        }
        protected void OnOperationError(string operationName, Exception e) {
            // TODO: Do something with the exception.
            OnOpertionEnd(operationName);
        }
        private void OnOpertionEnd(string operationName) {
            operationNames.Pop();
            SessionManager.PopOperation();
            if (!operationNames.Any()) {
                ApplicationSynchronizationContext.PopContext();
            }
        }
        internal void NotifyPresentationLayer() {
            // TODO: Standardize notification order. 
            // Currently dirty notifiers includes all three notifications in the order
            // in which they were made dirty. Availability, Value and Condition are in
            // this queue and the consumer won't know which order to expect them.
            while (_dirtyNotifiers.Count > 0) {
                var notifier = _dirtyNotifiers.Dequeue();
                notifier.Notify(NotifyLayer.Secondary);
            }
        }
        #endregion

        protected LinkToSession<Tsession> CreateLink<Tsession>(string path)
            where Tsession : SessionBase {
            var link = new LinkToSession<Tsession>(this, path);
            SessionManager.RegisterPathListener(this, link);
            return link;
        }

        private Dictionary<DataPropertyDescriptor, object> linkLookup = new Dictionary<DataPropertyDescriptor, object>();
        protected LinkToSession<Tsession> Link<Tsession>(DataPropertyDescriptor<Tsession> descriptor)
            where Tsession : SessionBase {
            if (linkLookup.ContainsKey(descriptor))
                return (LinkToSession<Tsession>)linkLookup[descriptor];

            // TODO: Turn this inside out and have CreateLink(string) call this method. 
            var sessionLink = CreateLink<Tsession>("./" + descriptor.PropertyName);
            linkLookup.Add(descriptor, sessionLink);
            return sessionLink;
        }

        protected LinkToSession<TResultsession> Link<TPropertysession, TResultsession>(DataPropertyDescriptor<TPropertysession> property, DataPropertyDescriptor<TResultsession> destinationsession)
            where TPropertysession : SessionBase
            where TResultsession : SessionBase {
            return Link<TPropertysession>(property).Link<TResultsession>(destinationsession);
        }

        protected LinkToSession<TResultsession> Link<TPropertysession, TIntermediatesession, TResultsession>(DataPropertyDescriptor<TPropertysession> propertysession, DataPropertyDescriptor<TIntermediatesession> intermediatsession, DataPropertyDescriptor<TResultsession> destinationsession)
            where TPropertysession : SessionBase
            where TIntermediatesession : SessionBase
            where TResultsession : SessionBase {
            return Link<TPropertysession>(propertysession).Link<TIntermediatesession>(intermediatsession).Link<TResultsession>(destinationsession);
        }


        protected LinkToSession<Tsession> CreateLink<Tsession>()
            where Tsession : SessionBase {
            return CreateLink<Tsession>(session => true);
        }

        protected LinkToSession<Tsession> CreateLink<Tsession>(Predicate<SessionBase> matchsession)
            where Tsession : SessionBase {
            var link = new LinkToSession<Tsession>(typeof(Tsession), matchsession);
            SessionManager.RegisterTypeListener(link);
            return link;
        }

        //public event PropertyChangedEventHandler PropertyChanged;
        //protected void RaisePropertyChangedEvent(string name)
        //{
        //    if (PropertyChanged != null)
        //        PropertyChanged(this, new PropertyChangedEventArgs(name));
        //}

        protected void Proxy_RegisterOperation(string operationName, Action action) {
            Operation operation;
            _operations.Add(operationName, operation = new Operation(operationName, action));
            operation.AvailabilityNotifier.ChangeNotificationSent += AvailabilityConditionNotifier_AvailabilityConditionChanged;
            _availabilities.Add(operationName, operation);
        }
    }
}
