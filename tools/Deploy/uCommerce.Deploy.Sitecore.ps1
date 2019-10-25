task DeploySitecoreLocal -depends SetSynchronizeSitecoreItemsPath, CopyBinariesToLocalFolder, CopyUnicornDependenciesToLocalFolder, CopyConfigurationLocal, CopyProjectFilesToLocalFolder

task SetSynchronizeSitecoreItemsPath{
  # C:\projects\Avenue Clothing for Sitecore\src\scripts\Serialization\App_Config\Include\AvenueClothing.Serialization.config

  $path = "$src\scripts\Serialization\App_Config\Include\AvenueClothing.Serialization.config"
  $xml = [xml](Get-Content $path)  

  $SynchronizeSitecoreItemsComponent = $xml.configuration.sitecore.unicorn.configurations.configuration.targetDataStore

  $path = "$src\project\AvenueClothing\serialization";
  if($Apis -eq "CommerceConnect"){
    $path = "$src\project\AvenueClothing-CC\serialization";
  }
  
  $SynchronizeSitecoreItemsComponent.SetAttribute("physicalRootPath", $path)

  $xml.Save("$working_dir\App_Config\Include\AvenueClothing.Serialization.config")
}

task CopyUnicornDependenciesToLocalFolder {
    Copy-Item "$src\AvenueClothing.Project.Website\bin\Unicorn.dll" "$working_dir\bin\Unicorn.dll" -Force 
    Copy-Item "$src\AvenueClothing.Project.Website\bin\Configy.dll" "$working_dir\bin\Configy.dll" -Force 
    Copy-Item "$src\AvenueClothing.Project.Website\bin\MicroCHAP.dll" "$working_dir\bin\MicroCHAP.dll" -Force 
    Copy-Item "$src\AvenueClothing.Project.Website\bin\Kamsar.WebConsole.dll" "$working_dir\bin" -Force 
    Copy-Item "$src\AvenueClothing.Project.Website\bin\Rainbow.dll" "$working_dir\bin\Rainbow.dll" -Force 
    Copy-Item "$src\AvenueClothing.Project.Website\bin\Rainbow.Storage.Yaml.dll" "$working_dir\bin\Rainbow.Storage.Yaml.dll" -Force 
    Copy-Item "$src\AvenueClothing.Project.Website\bin\\Rainbow.Storage.Sc.dll" "$working_dir\bin\Rainbow.Storage.Sc.dll" -Force 
}


task CopyBinariesToLocalFolder {
    foreach ($project in $projects) {
        Copy-Item "$src\$project\bin\$project.dll" "$working_dir\bin" -Force
        if ($Configuration -eq "Debug") {
            Copy-Item "$src\$project\bin\$project.pdb" "$working_dir\bin" -Force            
        }
    }
    
    #Handle installer project as library with another bin structure!
    if ($Configuration -eq "Debug") {
        Copy-Item "$src\AvenueClothing.Installer\bin\Debug\AvenueClothing.Installer.dll" "$working_dir\bin" -Force
        Copy-Item "$src\AvenueClothing.Installer\bin\Debug\AvenueClothing.Installer.pdb" "$working_dir\bin" -Force
        Copy-Item "$src\AvenueClothing.Foundation.MvcExtensions\bin\Debug\AvenueClothing.Foundation.MvcExtensions.dll" "$working_dir\bin" -Force
        Copy-Item "$src\AvenueClothing.Foundation.MvcExtensions\bin\Debug\AvenueClothing.Foundation.MvcExtensions.pdb" "$working_dir\bin" -Force
    }
    else {
        Copy-Item "$src\AvenueClothing.Installer\bin\Release\AvenueClothing.Installer.dll" "$working_dir\bin" -Force   
        Copy-Item "$src\AvenueClothing.Foundation.MvcExtensions\bin\Release\AvenueClothing.Foundation.MvcExtensions.dll" "$working_dir\bin" -Force   
    }

    Copy-Item "$src\..\lib\WebGrease\System.Web.Optimization.dll" "$working_dir\bin\System.Web.Optimization.dll" -Force
    Copy-Item "$src\..\lib\WebGrease\WebGrease.dll" "$working_dir\bin\WebGrease.dll" -Force

}

task CopyConfigurationLocal {
	Write-Host "Copying app_config to $working_dir"
	Copy-Item "$src\AvenueClothing.Installer\App_Config" "$working_dir" -Recurse -Force

    Copy-Item "$src\scripts\Serialization\App_Config\Include\AvenueClothing.Sites.config" "$working_dir\App_Config\Include" -Force
    
    
    if(!(Test-Path -Path "$working_dir\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\Pipelines\Initialize\")){
        New-Item -ItemType directory -Path "$working_dir\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\Pipelines\Initialize\"
    }
    if(!(Test-Path -Path "$working_dir\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\Pipelines\Installation\")){
        New-Item -ItemType directory -Path "$working_dir\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\Pipelines\Installation\"
    }
    Copy-Item "$src\AvenueClothing.Installer\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\*" "$working_dir\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\" -Recurse -Force
}


task CopyProjectFilesToLocalFolder {    
    $options = @("/xf", "*.dll", "/xf", "*.cs", "/xf", "*.csproj", "/xf", "packages.config", "/xf", "*.user", "/xf", "*.cache", "/xd", "obj", "/xd", "bin", "/xf", "global.asax", "/xf", "web.debug.config", "/xf", "web.release.config", "/xf", "web.config");
    
    foreach ($project in $projects) {
        ROBOCOPY "$src\$project" "$working_dir\" $options /e /s
    }
}