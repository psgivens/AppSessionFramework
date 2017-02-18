param($installPath, $toolsPath, $package, $project)

$proxyGeneratorAssembly = "CodeGen.SessionProxies.exe"
$proxyGeneratorTaskName = "GenerateSessionProxies"
$codeGenerator = "CodeGen.SessionProxies"


# Read the project file		
$projectXml = [xml] (Get-Content $project.FileName)
$defaultNamespace = $projectXml.Project.NamespaceURI

# Remove the UsingTask for our proxy generator task		
$usingTask = $projectXml.Project.UsingTask | Where { $_.TaskName -eq $proxyGeneratorTaskName }
if ($usingTask -ne $null) {
	$projectXml.Project.RemoveChild($usingTask)
}

$usingTask.SetAttribute("AssemblyFile", $proxyGeneratorPath)
$afterBuild = $projectXml.Project.Target | Where { $_.Name -eq "AfterBuild" }
if ($afterBuild -ne $null) {
	$sessionProxyGeneratorTask = $afterBuild.GenerateSessionProxies
	
	# Remove the SessionProxies task if found
	if ($sessionProxyGeneratorTask -ne $null) {
		$afterBuild.RemoveChild($sessionProxyGeneratorTask)
	}
	
	# Remove the AfterBuild target if no longer used. 
	if (!$afterBuild.HasChildNodes) {
		$projectXml.Project.RemoveChild($afterBuild)
	}
}
$projectXml.Save($project.FileName);	
