using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhillipScottGivens.Library.AppSessionFramework
{
    #region class DynamicValue<TValue>
    public class DynamicValue<TValue>
    {
        #region Fields
        private TValue value;
        private Func<TValue> valueCalculation;
        #endregion

        #region Properties
        public DualLayerNotifier ValueNotifier { get; private set; }
        public virtual TValue Value
        {
            get
            {
                return value;
            }
            protected set
            {
                // ReferenceEquals does not work with value types. 
                if (ReferenceEquals(this.value, value) || Equals(this.value, value))
                    return;

                this.value = value;

                IsDirty = true;

                ValueNotifier.Notify(NotifyLayer.Primary);
            }
        }
        public bool IsDirty { get; private set; }
        #endregion

        #region Events
        // This event makes the Value property databindable. 
        // TODO: Verify the above statement. 
        public event EventHandler ValueChanged
        {
            add { ValueNotifier.ChangeNotificationSent += value; }
            remove { ValueNotifier.ChangeNotificationSent -= value; }
        }
        #endregion

        #region Constructors
        internal DynamicValue(Func<TValue> valueCalculation, DualLayerNotifier firstAvailibilityConditionChanged, params DualLayerNotifier[] args)
            : this()
        {
            Link(valueCalculation, firstAvailibilityConditionChanged, args);
        }
        internal DynamicValue()
        {
            // TODO: When would we ever use ApplicationLayerNotifier
            ValueNotifier = new NonSessionNotifier(this);
        }

        internal void Link(Func<TValue> valueCalculation, DualLayerNotifier firstAvailibilityConditionChanged, params DualLayerNotifier[] args)
        {
            this.valueCalculation = valueCalculation;
            firstAvailibilityConditionChanged.ChangeNotificationSent += DependencyChanged;
            foreach (var notifier in args)
                notifier.ChangeNotificationSent += DependencyChanged;
            DependencyChanged(null, EventArgs.Empty);
        }

        internal void Unlink(DualLayerNotifier firstAvailibilityConditionChanged, params DualLayerNotifier[] args)
        {
            this.valueCalculation = () => default(TValue);
            firstAvailibilityConditionChanged.ChangeNotificationSent -= DependencyChanged;
            foreach (var notifier in args)
                notifier.ChangeNotificationSent -= DependencyChanged;
            DependencyChanged(null, EventArgs.Empty);
        }
        #endregion

        #region Factory Methods
        public static DynamicBool CreateCondition(
            Func<bool> valueCalculation,
            DualLayerNotifier firstAvailibilityConditionChanged,
            params DualLayerNotifier[] args)
        {
            return new DynamicBool(valueCalculation, firstAvailibilityConditionChanged, args);
        }
        public static DynamicBool CreateCondition(
            DynamicBool firstCondition,
            BooleanLogic logic,
            DynamicBool secondCondition)
        {
            return new DynamicBool(logic, firstCondition, secondCondition);
        }
        public static DynamicBool CreateCondition(
            BooleanLogic logic,
            DynamicBool firstCondition,
            DynamicBool secondCondition,
            params DynamicBool[] args)
        {
            return new DynamicBool(logic, firstCondition, secondCondition, args);
        }
        #endregion

        #region Handler to change Availability value
        protected virtual void DependencyChanged(object sender, EventArgs e)
        {
            Value = valueCalculation();
        }
        #endregion

        public static implicit operator TValue(DynamicValue<TValue> value)
        {
            return value.value;
        }
    }
    #endregion

    #region class AvailabilityCondition
    public class DynamicBool : DynamicValue<bool>
    {
        #region Constructors
        internal DynamicBool(
            Func<bool> valueCalculation,
            DualLayerNotifier firstAvailibilityConditionChanged,
            params DualLayerNotifier[] args)
            : base(valueCalculation, firstAvailibilityConditionChanged, args)
        {
        }
        internal DynamicBool(
            BooleanLogic logic,
            DynamicBool firstCondition,
            DynamicBool secondCondition,
            params DynamicBool[] args)
            : base(
                logic == BooleanLogic.And
                    ? CreateAndArgument(firstCondition, secondCondition, args)
                    : CreateOrArgument(firstCondition, secondCondition, args),
                firstCondition.ValueNotifier,
                CreateNotifiersList(secondCondition, args))
        {
        }
        #endregion

        #region Constructor Utility Methods
        private static DualLayerNotifier[] CreateNotifiersList(DynamicBool first, params DynamicBool[] args)
        {
            var notifiers = new List<DualLayerNotifier>();
            notifiers.Add(first.ValueNotifier);
            for (int index = 0; index < args.Length; index++)
                notifiers.Add(args[index].ValueNotifier);

            return notifiers.ToArray();
        }

        private static Func<bool> CreateAndArgument(DynamicBool firstCondition, DynamicBool secondCondition, DynamicBool[] args)
        {
            if (args.Length == 0)
                return () => firstCondition.Value && secondCondition.Value;

            return () =>
                {
                    if (!firstCondition.Value || !secondCondition.Value)
                        return false;

                    for (int index = 0; index < args.Length; index++)
                        if (!args[index].Value)
                            return false;

                    return true;
                };
        }

        private static Func<bool> CreateOrArgument(DynamicBool firstCondition, DynamicBool secondCondition, DynamicBool[] args)
        {
            if (args.Length == 0)
                return () => firstCondition.Value && secondCondition.Value;

            return () =>
            {
                if (firstCondition.Value || secondCondition.Value)
                    return true;

                for (int index = 0; index < args.Length; index++)
                    if (args[index].Value)
                        return true;

                return false;
            };
        }
        #endregion

        #region Operator Overloads
        private DynamicBool notValue;
        public DynamicBool Not
        {
            get
            {
                return !this;
            }
        }
        public static DynamicBool operator !(DynamicBool value)
        {
            if (value.notValue == null)
                value.notValue = new DynamicNotValue(value);

            return value.notValue;
        }
        public static DynamicBool operator &(DynamicBool first, DynamicBool second)
        {
            return new DynamicBool(BooleanLogic.And, first, second);
        }
        public static DynamicBool operator |(DynamicBool first, DynamicBool second)
        {
            return new DynamicBool(BooleanLogic.Or, first, second);
        }
        #endregion
    }

    #region class DynamicNotValue
    internal class DynamicNotValue : DynamicBool
    {
        internal DynamicNotValue(
            DynamicBool booleanCondition)
            : base(() => !booleanCondition.Value, booleanCondition.ValueNotifier)
        {
        }
    }
    #endregion
    #endregion

    #region interface IBooleanCondition
    //// Boolean values are unique in three ways. 
    //// * They can be used in boolean operations. 
    //// * Checking a boolean value also is relatively low impact. (as with all primitives)
    //// * 
    //public interface DynamicBooleanValue
    //{
    //    bool Value { get; }
    //    DualLayerNotifier ValueNotifier { get; }
    //    DynamicBooleanValue Not { get; }
    //}
    #endregion

    #region enum BooleanLogic
    public enum BooleanLogic
    {
        // TODO: Build shortcutting into the expression tree mechanism.
        // In many cases, we rely on shortcutting to prevent checking 
        // unavailable values. 
        And,
        Or
    }
    #endregion
}
