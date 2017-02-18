using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework {
    public interface ISessionRegistrar {
        void RegisterSession<TSession, TSessionProxy>()
            where TSession : SessionBase
            where TSessionProxy : TSession;
    }
}
