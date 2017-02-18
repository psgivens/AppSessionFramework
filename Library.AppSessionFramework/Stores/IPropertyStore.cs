using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework
{
    public interface IPropertyStore<TValue>
    {
        TValue Value { get; }
        bool IsAvailable { get; }
        DualLayerNotifier ValueNotifier { get; }
        DualLayerNotifier AvailabilityNotifier { get; }
    }
}
