using PhillipScottGivens.Library.AppSessionFramework.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.CodeGen.WpfCommands
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(args[0]);

            SessionProxyConfiguration configuration = SessionProxyConfiguration.DeserializeFromXML();

            var generator = new SessionProxyGeneratorBase(configuration.Namespace);
            foreach (var assemblyInfo in configuration.Assemblies)
                generator.AddAssembly(Assembly.LoadFrom(assemblyInfo.GetFullPath()));

            generator.GenerateWpfCommands(new WpfCommandsEmitter());
        }
    }
}
