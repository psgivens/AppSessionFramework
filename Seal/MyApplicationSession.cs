using PhillipScottGivens.Library.AppSessionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Seal {
    public partial class MyApplicationSession : SessionBase{

        [Operation]
        public virtual void DoSomethingAwesome() {
            SampleFlag = !SampleFlag;
        }
    }
}
