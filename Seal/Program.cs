using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CodeGen.SessionPartials
{
    class Program
    {
        static void Main(string[] args)
        {
            // namespace provided by VS provider.
            string defaultNamespace = "SampleNamespace";

            System.Diagnostics.Debugger.Break();
            //Directory.SetCurrentDirectory(args[0]);

            //SessionProxyConfiguration configuration = SessionProxyConfiguration.DeserializeFromXML();

            //var generator = new SessionGenerator(configuration.Namespace);
            //foreach (var assemblyInfo in configuration.Assemblies)
            //    generator.AddAssembly(Assembly.LoadFrom(assemblyInfo.GetFullPath()));

            //generator.GenerateSessionPartials(new SessionPartialsEmitter());
        }
    }
}
