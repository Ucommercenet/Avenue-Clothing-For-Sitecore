properties {
    $configuration = 'Debug'
    $src = "."
    $zipFileName = "uCommerce-Demo-Store-for-{0}-{1}.zip"
    $zipDestinationFolder = $null
    $solution_file = "AvenueClothing.sln"
    $working_dir = $null
    $version = $null
    $base_dir = $null
    $updatePackageVersion = $false
    $script:hash = @{}
    $target="none"
    $CreatePackage = $True
	$projects = @("AvenueClothing.Project.Website", "AvenueClothing.Project.Catalog", "AvenueClothing.Project.Header", "AvenueClothing.Project.Navigation", "AvenueClothing.Project.Transaction", "AvenueClothing.Project.UserFeedback")
    $Apis = $null
}
. .\Deploy.Common.ps1
. .\Validation.ps1
. .\Compile.ps1
. .\uCommerce.Sitecore.ps1
. .\uCommerce.Deploy.Sitecore.ps1

task default -depends Compile

task UpdateAssemblyInfo -description "Updates the AssemblyInfo.cs file if there is a valid version string supplied" -precondition { return IsVersionNumber $version } {
    
    if ($UpdateAssemblyInfo -eq "True") {
    
        Push-Location $src
    
        $assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
        $fileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
        $assemblyVersion = 'AssemblyVersion("' + $script:version + '")';
        $fileVersion = 'AssemblyFileVersion("' + $script:version + '")';
    
        Get-ChildItem -r -filter AssemblyInfo.cs | ForEach-Object {
            $filename = $_.Directory.ToString() + '\' + $_.Name
            $filename + ' -> ' + $script:version
        
            # If you are using a source control that requires to check-out files before 
            # modifying them, make sure to check-out the file here.
            # For example, TFS will require the following command:
            # tf checkout $filename
            (Get-Content $filename) | ForEach-Object {
                % {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
                % {$_ -replace $fileVersionPattern, $fileVersion }
            } | Set-Content $filename
        }

        Pop-Location
    }
}

task CleanWebBinDirectory -description "Cleans the bin directory of the web project" {
    Push-Location "$src"
    if (Test-Path $src\AvenueClothing.Project.Website\bin)
    {
        Remove-Item -Recurse "$src\AvenueClothing.Project.Website\bin\*" -Force
    }
    Pop-Location
}