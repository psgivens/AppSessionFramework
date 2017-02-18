using PhillipScottGivens.Library.PsgCore.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework.Generators {
    public partial class SessionProxyEmitter : ITextTransform {
        private readonly CodeGenerationTools code = new CodeGenerationTools();
        private readonly CodeTransformHelper helper;

        public SessionProxyEmitter() {
            helper = new CodeTransformHelper(code, this);
        }

        public new StringBuilder GenerationEnvironment {
            get { return base.GenerationEnvironment; }
        }

        public CodeGenerationTools Code {
            get { return code; }
        }

        public void EmitUsingStatements(SessionGeneratorContext context) {
            helper.EmitUsings(new string[]
            {
                "PhillipScottGivens.Library.AppSessionFramework",
                //"PhillipScottGivens.Library.AppSessionFramework.ComponentModel",
                "System",
                "System.ComponentModel",                
            });
        }

        public IDisposable BeginNamespace(string @namespace) {
            return helper.BeginNamespace(@namespace);
        }

        public IDisposable BeginEmitSessionClass(Type type) {
            //WriteLine("[TypeDescriptionProvider(typeof(SessionTypeDescriptionProvider<{0}>))]", type.Name);
            return helper.BeginEmitBlock(
                string.Format("public class {0} : {1}", type.Name, type.FullName));
        }

        public void EmitOperation(System.Reflection.MethodInfo method) {
            if (method.ReturnType == typeof(void)) {
                T4EmitVoidOperation(method);
            }
            else {
                T4EmitTypeOperation(method);
            }
        }

        public void EmitDataProperty(PropertyInfo property) {
            T4EmitDataProperty(property,
                code.SpaceAfter(property.GetSetMethod(true).IsFamily ? "protected" : string.Empty));
        }

        public IDisposable BeginEmitSessionContainer() {
            return helper.BeginEmitBlock("public static class SessionContainer");
        }

        public IDisposable BeginEmitSessionContainerInitialize() {
            return helper.BeginEmitBlock("public static void Initialize(ISessionRegistrar registrar)");
        }

        public void EmitSessionContainerRegistration(Type type) {
            WriteLine("registrar.RegisterSession<{0}, {1}>();", type.FullName, type.Name);
        }

        internal void EmitConstructor(Type type) {
            foreach (var constructor in type.GetConstructors()) {
                T4EmitConstructor(code.OverrideConstructorSignature(type, constructor));
            }
        }
    }
}
