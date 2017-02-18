using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework
{
    internal interface ISessionPropertyStore
    {
        void RegisterSessionListener(SessionListener listener);
        string Name { get; }

        void UnRegistersessionListener(SessionListener listener);

        void RescindListener(SessionListener listener);

        object Value { get; }
    }
}
