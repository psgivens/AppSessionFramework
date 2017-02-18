using PhillipScottGivens.Library.AppSessionFramework.Generators;
using PhillipScottGivens.Library.PsgCore.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework.Generators {
    public abstract class SessionProxyGeneratorBase {
        private readonly string @namespace;
        private readonly SessionGeneratorContext context = new SessionGeneratorContext();
        private readonly List<Assembly> _assemblies = new List<Assembly>();

        internal static Type SessionBaseType;
        internal static Type AbstractBaseSessionAttribute;
        internal static Type ObservableAttribute;

        public SessionProxyGeneratorBase(string @namespace) {
            this.@namespace = @namespace;
        }

        public static void SetFrameworkAssembly(Assembly assembly) {
            var name = assembly.GetName().Name;
            if (name == "Library.AppSessionFramework" || name == "Portable.Library.AppSessionFramework") {
                SessionBaseType = assembly.GetType("PhillipScottGivens.Library.AppSessionFramework.SessionBase");
                AbstractBaseSessionAttribute = assembly.GetType("PhillipScottGivens.Library.AppSessionFramework.AbstractBaseSessionAttribute");
                ObservableAttribute = assembly.GetType("PhillipScottGivens.Library.AppSessionFramework.ObservableAttribute");
            }
            else {
                throw new ArgumentException("Expected Libary.AppSessionFramework, but got " + assembly.GetName().Name);
            }
        }

        public void AddAssembly(Assembly assembly) {

            _assemblies.Add(assembly);
        }

        protected void AddSessionBasesToContext() {
            foreach (var assembly in _assemblies) {
                context.AddSessionInfos(from type in assembly.GetTypes()
                                        where SessionBaseType.IsAssignableFrom(type)
                                        select type);
            }
        }

        public void GenerateProxies(string proxyLocation) {
            GenerateProxiesCore(proxyLocation, @namespace, context);
        }

        protected abstract void GenerateProxiesCore(string proxyLocation, string @namespace, SessionGeneratorContext context);

        public void GenerateWpfCommands(WpfCommandsEmitter emitter) {
            AddSessionBasesToContext();

            TemplateFileManager fileManager = new TemplateFileManager(emitter.GenerationEnvironment);
            fileManager.StartNewFile("WpfSessionCommands.cs");
            emitter.EmitUsingStatements(context);
            using (emitter.BeginNamespace(@namespace)) {
                //using (emitter.BeginEmitSessionContainer())
                //using (emitter.BeginEmitSessionContainerInitialize())
                //{
                //    foreach (var info in from i in context.SessionInfos
                //                         where !i.IsAbstractBase
                //                         select i)
                //    {
                //        emitter.EmitSessionContainerRegistration(info.Type);
                //    }
                //}

                foreach (var info in from i in context.SessionInfos
                                     where !i.IsAbstractBase
                                     select i) {
                    using (emitter.BeginEmitCommandsClass(info.Type)) {
                        foreach (var method in info.Operations) {
                            emitter.EmitRoutedCommand(info, method);
                        }

                        using (emitter.BeginEmitBindCommandsMethod(info.Type))
                        using (emitter.BeginReturnCommandBindingsArray()) {
                            foreach (var method in info.Operations) {
                                emitter.EmitCommandBinding(method);
                            }
                        }
                    }
                }
            }
            fileManager.Process(@".\Generated\", true);
        }
    }
}
