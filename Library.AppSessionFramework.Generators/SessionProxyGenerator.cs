using PhillipScottGivens.Library.PsgCore.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework.Generators {
    public class SessionProxyGenerator : SessionProxyGeneratorBase {
        public SessionProxyGenerator(string @namespace) : base(@namespace) { }
        protected override void GenerateProxiesCore(string proxyLocation, string @namespace, SessionGeneratorContext context) {
            SessionProxyEmitter emitter = new SessionProxyEmitter();
            AddSessionBasesToContext();

            TemplateFileManager fileManager = new TemplateFileManager(emitter.GenerationEnvironment);
            fileManager.StartNewFile("SessionProxies.cs");
            emitter.EmitUsingStatements(context);
            using (emitter.BeginNamespace(@namespace)) {
                using (emitter.BeginEmitSessionContainer())
                using (emitter.BeginEmitSessionContainerInitialize()) {
                    foreach (var info in from i in context.SessionInfos
                                         where !i.IsAbstractBase
                                         select i) {
                        emitter.EmitSessionContainerRegistration(info.Type);
                    }
                }
            }

            foreach (var info in from i in context.SessionInfos
                                 where !i.IsAbstractBase
                                 select i) {
                using (emitter.BeginNamespace(info.Type.Namespace + ".Proxies")) {
                    using (emitter.BeginEmitSessionClass(info.Type)) {
                        emitter.EmitConstructor(info.Type);
                        foreach (var method in info.Operations) {
                            emitter.EmitOperation(method);
                        }
                        foreach (var property in info.DataProperties) {
                            emitter.EmitDataProperty(property);
                        }
                    }
                }
            }
            fileManager.Process(proxyLocation, true);
        }

    }
}
