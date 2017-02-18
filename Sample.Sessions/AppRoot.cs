using PhillipScottGivens.Library.AppSessionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Sessions {
    public class AppRoot {
        public static MainSession MainSession { get; private set; }
        static AppRoot() {
            MainSession = SessionManager.ResolveSession<MainSession>();
        }
    }
}
