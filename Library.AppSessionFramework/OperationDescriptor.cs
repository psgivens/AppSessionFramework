using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework {
    public class OperationDescriptor {
        public string Name { get; private set; }
        private OperationDescriptor(string name) {
            Name = name;
        }

        public static OperationDescriptor Register(string name, Type type) {
            return new OperationDescriptor(name);
        }
    }
}
