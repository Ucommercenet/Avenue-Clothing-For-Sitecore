task CreateSitecorePackage -depends ValidateSetup, CleanSitecoreWorkingDirectory, CleanWebBinDirectory, Rebuild, CopySitecoreFiles, CreateSitecoreZipFile, DeleteTempPackage {

}

task SetSitecoreVars -description "Set paths for Deploy.To.Package." {
    $script:hash.ucommerce_dir = "$working_dir\files\sitecore modules\Shell\ucommerce" 
    $script:hash.bin_dir = "$working_dir\files\bin"
    $script:hash.files_root_dir = "$working_dir\files"
}

task DeleteTempPackage {
	Remove-Item "c:\tmp\package.zip"
}

task CopySitecoreFiles -description "Copy all the sitecore files needs for a deployment" {
    
    # Copy bin files from the demostore installer project.
    $options = @("/S", "/XF *.cs", "/xf *.??proj", "/xf *.user", "/xf *.old", "/xf *.vspscc", "/xf xsltExtensions.config", "/xf uCommerce.key", "/xf *.tmp", "/xd _Resharper*", "/xd .svn", "/xd _svn")    
    
    $ucommerce_dir = $script:hash["ucommerce_dir"]
    $bin_dir = $script:hash["bin_dir"]
    $files_root = $script:hash["files_root_dir"]



    # Installer files ... needs to be copied from the Installer Project since it's not referenced in the Main
    $ucommerceSitecoreInstallerBins = @("uCommerce.DemoStore.Installer.dll", "unicorn.dll" , "Kamsar.WebConsole.dll")

    CopyFiles "$src\uCommerce.DemoStore.Installer\bin\$configuration\" "$bin_dir" $ucommerceSitecoreInstallerBins
    #END copy of bins from demostore installer project.
    
    # COPY package folder with installer and meta data (required sitecore package format)
    Copy-Item "$src\uCommerce.DemoStore.Installer\package\*" "$working_dir" -Recurse -Force
    # END COPY package folder.
    
    #copy all site specific files into the files folder of the package 
    & robocopy "$src\uCommerce.DemoStore\Controls" "$files_root\Controls" /E
    & robocopy "$src\uCommerce.DemoStore\Css" "$files_root\Css" /E
    & robocopy "$src\uCommerce.DemoStore\img" "$files_root\img" /E
    & robocopy "$src\uCommerce.DemoStore\Layouts" "$files_root\Layouts" /E /xf *.cs
    & robocopy "$src\uCommerce.DemoStore\Scripts" "$files_root\Scripts" /E
    & robocopy "$src\uCommerce.DemoStore.Installer\bin\$configuration" "$files_root\bin" /E
    & robocopy "$src\uCommerce.DemoStore\bin\UCommerce.DemoStore.dll" "$files_root\bin" /E
    
    #copy serialised items (Sitecore serialized items) into a folder where we can find them when syncronizing items in post installation step.
    & robocopy "$src\uCommerce.DemoStore.SitecoreItems\master" "$ucommerce_dir\install\unicorn\master" /E 
    
    & robocopy "$src\uCommerce.DemoStore.Installer\Install\App_Config" "$files_root\sitecore modules\shell\ucommerce\install\App_Config" /E
}

task CleanSitecoreWorkingDirectory -description "Cleans the sitecore working directory. This should NOT be used when using Deploy.To.Local" -depends SetSitecoreVars{
    # Create directories
    if(Test-Path $working_dir)
    {
        Remove-Item -Recurse "$working_dir\*" -Force
    }
    else
    {
        New-Item "$working_dir" -Force -ItemType Directory    
    }

    New-Item "$working_dir\files\sitecore modules\Shell\ucommerce\install" -Force -ItemType Directory
    New-Item "$working_dir\installer" -Force -ItemType Directory
    New-Item "$working_dir\metadata" -Force -ItemType Directory
    New-Item "$working_dir\files\bin" -Force -ItemType Directory
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
    Get-Content "$src\uCommerce.DemoStore.Installer\PackageInformation\Readme.txt" > "$src\UCommerce.DemoStore.Installer\package\metadata\sc_readme.txt"
    "$version" > "$src\uCommerce.DemoStore.Installer\package\metadata\sc_version.txt"
    "Avenue-clothing Demo Store $version" > "$src\uCommerce.DemoStore.Installer\package\metadata\sc_name.txt"
}