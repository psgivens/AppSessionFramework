using PhillipScottGivens.Library.PsgCore.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Library.AppSessionFramework.Generators
{
    public partial class WpfCommandsEmitter : ITextTransform
    {
        private readonly CodeGenerationTools code = new CodeGenerationTools();
        private readonly CodeTransformHelper helper;

        public WpfCommandsEmitter()
        {
            helper = new CodeTransformHelper(code, this);
        }

        public new StringBuilder GenerationEnvironment
        {
            get { return base.GenerationEnvironment; }
        }

        public CodeGenerationTools Code
        {
            get { return code; }
        }

        public void EmitUsingStatements(SessionGeneratorContext context)
        {
            helper.EmitUsings(new string[]{
                "System.Collections",
                "System.Windows",
                "System.Windows.Input",
            });
        }

        public IDisposable BeginNamespace(string @namespace)
        {
            return helper.BeginNamespace(@namespace);
        }

        public IDisposable BeginEmitCommandsClass(Type type)
        {
            return helper.BeginEmitBlock(string.Format(
                "public class {0} : DependencyObject",
                type.Name));
                
        }

        internal void EmitRoutedCommand(SessionMetaInfo info, MethodInfo method)
        {
            WriteLine("public static readonly RoutedCommand {0} = new RoutedCommand(\"{0}\", typeof({1}));",
                method.Name, info.Type.Name);
        }

        internal IDisposable BeginEmitBindCommandsMethod(Type type)
        {
            return helper.BeginEmitBlock(string.Format(
                "public static ICollection BindCommands({0} session)",
                type.FullName));
        }

        internal IDisposable BeginReturnCommandBindingsArray()
        {
            return helper.BeginEmitBlock("return new CommandBinding[]", ";");
        }

        internal void EmitCommandBinding(MethodInfo method)
        {
            T4EmitCommandBinding(method.Name);
        }

    }
}
