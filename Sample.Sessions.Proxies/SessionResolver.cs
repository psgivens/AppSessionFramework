using PhillipScottGivens.Library.AppSessionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Sessions.Proxies {
    public class SessionResolver : ISessionResolver {
        public TSession ResolveSession<TSession>()
            where TSession : SessionBase {
            Type type = typeof(TSession);
            if (type == typeof(Sample.Sessions.MainSession))
                return (TSession)((object)new Sample.Sessions.Proxies.MainSession());
            if (type == typeof(Sample.Sessions.SubSession))
                return (TSession)((object)new Sample.Sessions.Proxies.SubSession());
            if (type == typeof(Sample.Sessions.LoginSession))
                return (TSession)((object)new Sample.Sessions.Proxies.LoginSession(13));
            return default(TSession);
        }

        public static void Initialize() {
            SessionManager.Initialize(new SessionResolver());
        }
    }
}
