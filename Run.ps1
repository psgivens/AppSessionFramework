

. $profile
$DebugPreference = 'Continue'
$VerbosePreference = 'Continue'
pushd CodeGen.SessionProxies
Write-Nuspec CodeGen.SessionProxies
Publish-SolutionProject -ProjectName CodeGen.SessionProxies -NugetServer "http://nuget.phillipgivens.com" -ApiKey keyofphillip
popd