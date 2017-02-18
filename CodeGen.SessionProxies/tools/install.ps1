param($installPath, $toolsPath, $package, $project)

$proxyGeneratorAssembly = "CodeGen.SessionProxies.exe"
$proxyGeneratorTaskName = "GenerateSessionProxies"
$codeGenerator = "CodeGen.SessionProxies"


# Read the project file
$projectXml = [xml] (Get-Content $project.FileName)
$defaultNamespace = $projectXml.Project.NamespaceURI

# Add a UsingTask so that we can use our proxy generator task		
$usingTask = $projectXml.Project.UsingTask | Where { $_.TaskName -eq $proxyGeneratorTaskName }
if ($usingTask -eq $null) {
	$usingTask = $projectXml.CreateElement("UsingTask", $defaultNamespace);
	$projectXml.Project.AppendChild($usingTask);
	$usingTask.SetAttribute("TaskName", $proxyGeneratorTaskName)
}
$proxyGeneratorPath = "`$(SolutionDir)" + (Join-Path $toolsPath.Substring($toolsPath.IndexOf("packages\")) $proxyGeneratorAssembly)
$usingTask.SetAttribute("AssemblyFile", $proxyGeneratorPath)

# Add the proxy generator task to the AfterBuild target
$afterBuild = $projectXml.Project.Target | Where { $_.Name -eq "AfterBuild" }
if ($afterBuild -eq $null) {
	$afterBuild = $projectXml.CreateElement("Target", $defaultNamespace)
	$afterBuild.SetAttribute("Name", "AfterBuild")
	$projectXml.Project.AppendChild($afterBuild)
}
if ($afterBuild.GenerateSessionProxies -eq $null){		
	$sessionProxyGeneratorTask = $projectXml.CreateElement($proxyGeneratorTaskName, $defaultNamespace)	
	$sessionProxyGeneratorTask.SetAttribute("RootNamespace", "`$(RootNamespace)")
	$sessionProxyGeneratorTask.SetAttribute("SolutionDir", "`$(SolutionDir)")
	$sessionProxyGeneratorTask.SetAttribute("TargetPath", "`$(TargetPath)")
	$sessionProxyGeneratorTask.SetAttribute("CodeGenerator", $codeGenerator)
	$sessionProxyGeneratorTask.SetAttribute("OutputFolder", ("{0}.Proxies" -f $project.Name))
	$afterBuild.AppendChild($sessionProxyGeneratorTask)
}
$projectXml.Save($project.FileName);	
