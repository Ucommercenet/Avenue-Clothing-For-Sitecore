task CreateSitecorePackage -depends ValidateSetup, CleanSitecoreWorkingDirectory, CleanWebBinDirectory, Rebuild, CreateWorkingDir, CopyMetaDataToWorkingDir, CopyProjectFilesToFilesFolder, CreateSitecoreZipFile, DeleteTempPackage {

}

task SetSitecoreVars -description "Set paths for Deploy.To.Package." {
    $script:hash.ucommerce_dir = "$working_dir\files\sitecore modules\Shell\ucommerce" 
    $script:hash.bin_dir = "$working_dir\files\bin"
    $script:hash.files_root_dir = "$working_dir\files"
}

task DeleteTempPackage {
	Remove-Item "c:\tmp\package.zip"
}

task CleanSitecoreWorkingDirectory -description "Cleans the sitecore working directory. This should NOT be used when using Deploy.To.Local" -depends SetSitecoreVars{
    # Create directories
    if(Test-Path $working_dir)
    {
        Remove-Item -Recurse "$working_dir\*" -Force
    }
}

task CreateWorkingDir {
	
	#Create directories in tmp for all files that makes up the package
    if(!(Test-Path $working_dir))
    {
        New-Item "$working_dir" -Force -ItemType Directory
    }
	
    New-Item "$working_dir\files\sitecore modules\Shell\ucommerce\install" -Force -ItemType Directory
    New-Item "$working_dir\installer" -Force -ItemType Directory
    New-Item "$working_dir\metadata" -Force -ItemType Directory
    New-Item "$working_dir\files\bin" -Force -ItemType Directory
}

task CopyMetaDataToWorkingDir {
	Copy-Item "$src\AvenueClothing.Installer\package\*" "$working_dir" -Recurse -Force
}

task CopyProjectFilesToFilesFolder {
    foreach ($project in $projects) {
        Copy-Item "$src\$project\bin\$project.dll" "$working_dir\files\bin" -Force
        if ($Configuration -eq "Debug") {
            Copy-Item "$src\$project\bin\$project.pdb" "$working_dir\files\bin" -Force            
        }
    }
}

task CreateSitecoreZipFile -description "Creates the Sitecore Zip fil" {
    Assert($script:version -ne $null) "'version' cannot be null."
    Assert($zipDestinationFolder -ne $null) "'zipDestinationFolder' cannot be null."
    Assert($zipFileName -ne $null) "'zipFileName' cannot be null."

    # Create the filename.
    $newFileName = GetZipFilename

    $packageZipFullName = "$zipDestinationFolder\package.zip"
    if(Test-Path $packageZipFullName)
    {
        Remove-Item $packageZipFullName
    }

    # Create a zip file from the working_dir.
    Exec { Invoke-Expression "& '$base_dir\tools\7zip\7z.exe' a -r -tZip -mx9 $packageZipFullName '$working_dir\*'" }

    Exec { Invoke-Expression "& '$base_dir\tools\7zip\7z.exe' a -tZip -mx9 $newFileName '$packageZipFullName'" }
}

task UpdateSitecorePackageInfo -description "Updates the Sitecore package information file" -precondition { return IsVersionNumber $version } {
    $version = $script:version
    Get-Content "$src\AvenueClothing.Installer\PackageInformation\Readme.txt" > "$src\AvenueClothing.Installer\package\metadata\sc_readme.txt"
    "$version" > "$src\AvenueClothing.Installer\package\metadata\sc_version.txt"
    "Avenue-clothing Demo Store $version" > "$src\AvenueClothing.Installer\package\metadata\sc_name.txt"
}