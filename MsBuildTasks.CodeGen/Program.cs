using PhillipScottGivens.CodeGen.SessionProxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsBuildTasks.CodeGen {
    class Program {
        static void Main(string[] args) {
            var task = new GenerateSessionProxies {
                RootNamespace = "Seal.Proxies",
                SolutionDir = @"C:\R\GH\AppSessionFramework\Seal\",
                TargetPath = @"C:\R\GH\AppSessionFramework\Seal\bin\Debug\Seal.exe",                
                OutputFolder = @"Seal.Proxies",                
                CodeGenerator = "CodeGen.SessionProxies",                
            };
            task.Execute();
        }
    }
}
