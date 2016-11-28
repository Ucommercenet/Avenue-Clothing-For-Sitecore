task CleanSolution -description "Cleans the complete solution" {
    Push-Location "$src"
    Exec { msbuild "$solution_file" /t:Clean /p:VisualStudioVersion=12.0 }
    Pop-Location
}

task Rebuild -depend CleanSolution, Compile

task Compile -description "Compiles the complete solution" -depends UpdateAssemblyInfo, UpdateSitecorePackageInfo { 
    Push-Location "$src"
	Exec { msbuild "$solution_file" /p:Configuration=$configuration /m /p:VisualStudioVersion=12.0 }
    Pop-Location
}