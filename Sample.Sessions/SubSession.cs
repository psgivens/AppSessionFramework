using PhillipScottGivens.Library.AppSessionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Sessions {
    public partial class SubSession : SessionBase {

        public SubSession() {
            MyFlag = true;

            bool val = IsMyFlagAvailable;
        }

        public virtual void DoSubAction() {
            this.MyFlag = !this.MyFlag;
        }
        public virtual void DoOtherSubAction() {
            this.MyFlag = !this.MyFlag;
        }

        internal void Close() {
            CloseSession();
        }
    }
}
