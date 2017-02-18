using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhillipScottGivens.Library.AppSessionFramework.WPF
{
    public interface IHasNotifyableValue<TValue> : IHasValue<TValue>
    {
        void RaisePropertyChanged(string name);
    }
}
