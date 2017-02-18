using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework
{
    public static class PropertyStoreExtensions
    {
        internal static DynamicBool ToDynamicBooleanValue(this IPropertyStore<bool> propertyStore, bool valueWhenNotAvailable)
        {
            var gatedCondition = valueWhenNotAvailable
                ? new Func<bool>(() => !propertyStore.IsAvailable || propertyStore.Value)
                : new Func<bool>(() => propertyStore.IsAvailable && propertyStore.Value);
            return new DynamicBool(gatedCondition, propertyStore.AvailabilityNotifier, propertyStore.ValueNotifier);
        }
    }
}
