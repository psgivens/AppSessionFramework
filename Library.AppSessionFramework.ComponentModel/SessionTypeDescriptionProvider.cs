using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework.ComponentModel {
    public class SessionTypeDescriptionProvider<TSession> : TypeDescriptionProvider
        where TSession : SessionBase {
        private readonly static SessionTypeDescriptionProvider<TSession> _instance = new SessionTypeDescriptionProvider<TSession>();
        static SessionTypeDescriptionProvider() {
            TypeDescriptor.AddProvider(_instance, typeof(TSession));
        }

        /// <summary>
        /// An initialization method which forces the static constructor to execute. 
        /// </summary>
        public static void Register() {
            // Forces the static constructor to execute the first time this is called. 
        }

        public SessionTypeDescriptionProvider()
            : base(TypeDescriptor.GetProvider(typeof(TSession))) { }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
            return new SessionTypeDescriptor<TSession>(base.GetTypeDescriptor(objectType, instance));
        }
    }
}
