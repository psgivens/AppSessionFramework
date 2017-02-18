using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using PhillipScottGivens.Library.AppSessionFramework.Generators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PhillipScottGivens.CodeGen.SessionProxies {
    [Serializable]
    public class GenerateSessionProxies : Task {
        [Required]
        public string OutputFolder { get; set; }

        [Required]
        public string TargetPath { get; set; }

        [Required]
        public string RootNamespace { get; set; }

        // Not currently used. 
        [Required]
        public string CodeGenerator { get; set; }

        [Required]
        public string SolutionDir { get; set; }

        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        public override bool Execute() {

            if (CodeGenerator != "CodeGen.SessionProxies")
                throw new ArgumentException("CodeGenerator only supports CoeGen.SessionProxies");

            if (!Path.IsPathRooted(OutputFolder))
                OutputFolder = Path.Combine(SolutionDir, OutputFolder);

            if (!Path.IsPathRooted(TargetPath))
                TargetPath = Path.Combine(SolutionDir, TargetPath);

            string location = CodeGenerator == "CodeGen.SessionProxies"
                ? typeof(Program).Assembly.Location
                : "";

            var process = System.Diagnostics.Process.Start(location,
                string.Format("\"{0}\" \"{1}\" \"{2}\"", RootNamespace, OutputFolder, TargetPath));

            return process.WaitForExit(2000);
        }
    }
}
