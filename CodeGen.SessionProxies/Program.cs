using PhillipScottGivens.Library.AppSessionFramework.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.CodeGen.SessionProxies
{
    public class Program
    {
        static void Main(string[] args)
        {
            string @namespace = args[0];
            string outputDirectory = args[1];
            string assemblyToProxy = args[2];

            var generator = new SessionProxyGenerator(@namespace + ".Proxies");
            
            // Load the AppSessionFramework dynamically to ensure the same instance as used by the target. 
            var sessionFrameworkLocation  = Path.Combine(Path.GetDirectoryName(assemblyToProxy), "Library.AppSessionFramework.dll");
            if (!File.Exists(sessionFrameworkLocation)) {
                sessionFrameworkLocation = Path.Combine(Path.GetDirectoryName(assemblyToProxy), "Portable.Library.AppSessionFramework.dll");
                if (!File.Exists(sessionFrameworkLocation)) {
                    throw new InvalidOperationException("Library.AppSessionFramework.dll was not found in "
                        + Path.Combine(Path.GetDirectoryName(assemblyToProxy)));
                }
            }
            //if (string.IsNullOrEmpty(sessionFrameworkLocation)) 
            //    throw new InvalidOperationException("Library.AppSessionFramework.dll was not found in " 
            //        + Path.Combine(Path.GetDirectoryName(assemblyToProxy)));

            Console.WriteLine("sessionFrameworkLocation: " + sessionFrameworkLocation);
            SessionProxyGeneratorBase.SetFrameworkAssembly(Assembly.LoadFrom(sessionFrameworkLocation));

            generator.AddAssembly(Assembly.LoadFrom(assemblyToProxy));
            generator.GenerateProxies(Path.Combine(outputDirectory, "Generated"));


        }
    }
}
