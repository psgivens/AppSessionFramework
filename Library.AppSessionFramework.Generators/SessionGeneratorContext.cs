using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework.Generators
{
    public class SessionGeneratorContext
    {
        private readonly List<SessionMetaInfo> sessionTypes = new List<SessionMetaInfo>();

        public void AddSessionInfos(IEnumerable<Type> types)
        {
            sessionTypes.AddRange(types.Select(item => new SessionMetaInfo(item)));
        }

        public IEnumerable<SessionMetaInfo> SessionInfos
        {
            get
            {
                return sessionTypes;
            }
        }
    }
}
