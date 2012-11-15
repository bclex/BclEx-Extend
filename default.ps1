properties { 
  $base_dir = resolve-path .
  $build_dir = "$base_dir\build"
  $packageinfo_dir = "$base_dir\nuspecs"
  $35_build_dir = "$build_dir\3.5\"
  $40_build_dir = "$build_dir\4.0\"
  $release_dir = "$base_dir\Release"
  $sln_file = "$base_dir\BclEx-Extend.sln"
  $tools_dir = "$base_dir\tools"
  $version = "1.0.0" #Get-Version-From-Git-Tag
  $35_config = "Release"
  $40_config = "Release.4"
  $run_tests = $true
}
Framework "4.0"

#include .\psake_ext.ps1
	
task default -depends Package

task Clean {
	remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue
	remove-item -force -recurse $release_dir -ErrorAction SilentlyContinue
}

task Init -depends Clean {
	new-item $build_dir -itemType directory 
	new-item $release_dir -itemType directory 
}

task Compile -depends Init {
	msbuild $sln_file /p:"OutDir=$35_build_dir;Configuration=$35_config"
	msbuild $sln_file /target:Rebuild /p:"OutDir=$40_build_dir;Configuration=$40_config"
}

task Test -depends Compile -precondition { return $run_tests } {
	$old = pwd
	cd $build_dir
	#& $tools_dir\xUnit\xunit.console.clr4.exe "$40_build_dir\System.Abstract.Tests.dll" /noshadow
	#& $tools_dir\xUnit\xunit.console.clr4.exe "$40_build_dir\System.Abstract.Tests.dll" /noshadow
	#& $tools_dir\xUnit\xunit.console.clr4.exe "$40_build_dir\System.Abstract.Tests.dll" /noshadow
	#& $tools_dir\xUnit\xunit.console.clr4.exe "$40_build_dir\System.Abstract.Tests.dll" /noshadow
	#& $tools_dir\xUnit\xunit.console.clr4.exe "$40_build_dir\System.Abstract.Tests.dll" /noshadow
	#& $tools_dir\xUnit\xunit.console.clr4.exe "$40_build_dir\System.Abstract.Tests.dll" /noshadow
	cd $old
}

task Dependency {
	$package_files = @(Get-ChildItem src -include *packages.config -recurse)
	foreach ($package in $package_files)
	{
		Write-Host $package.FullName
		& $tools_dir\NuGet.exe install $package.FullName -o packages
	}
}

task Release -depends Dependency, Compile, Test {
	cd $build_dir
	& $tools_dir\7za.exe a $release_dir\BclEx-Extend.zip `
		*\Contoso.CoreEx.dll `
		*\Contoso.CoreEx.xml `
		*\Contoso.VisualStudio.dll `
		*\Contoso.VisualStudio.xml `
		*\System.CoreEx.dll `
		*\System.CoreEx.xml `
		*\System.DirectoryServicesEx.dll `
		*\System.DirectoryServicesEx.xml `
		*\System.Text.Lalr.dll `
		*\System.Text.Lalr.xml `
    	..\license.txt
	if ($lastExitCode -ne 0) {
		throw "Error: Failed to execute ZIP command"
    }
}

task Package -depends Release {
	$spec_files = @(Get-ChildItem $packageinfo_dir)
	foreach ($spec in $spec_files)
	{
		& $tools_dir\NuGet.exe pack $spec.FullName -o $release_dir -Version $version -Symbols -BasePath $base_dir
	}
}
