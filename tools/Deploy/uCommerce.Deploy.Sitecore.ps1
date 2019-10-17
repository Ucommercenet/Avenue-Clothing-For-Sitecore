task DeploySitecoreLocal -depends SetSynchronizeSitecoreItemsPath, CopyBinariesToLocalFolder, CopyUnicornItemsLocal, CopyConfigIncludeFilesLocal, CopyUnicornDependenciesToLocalFolder, CopyConfigurationLocal, CopyProjectFilesToLocalFolder

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

task CopyUnicornItemsLocal {
    Copy-Item "$src\Project\AvenueClothing" "$working_dir\App_Data\tmp\accelerator\" -Recurse -Force
}

task CopyConfigIncludeFilesLocal {

    Copy-Item "$src\packages\Rainbow.2.1.1\content\App_Config\Include\Rainbow.config" "$working_dir\App_Config\Include\Rainbow.config" -Force
    Copy-Item "$src\packages\Unicorn.4.1.1\content\App_Config\Include\Unicorn\Unicorn.config" "$working_dir\App_Config\Include\unicorn.config" -Force
    Copy-Item "$src\packages\Unicorn.4.1.1\content\App_Config\Include\Unicorn\Unicorn.UI.config" "$working_dir\App_Config\Include\unicorn.UI.config" -Force
    Copy-Item "$src\scripts\Serialization\App_Config\Include\AvenueClothing.Serialization.config" "$working_dir\App_Config\Include\AvenueClothing.Serialization.Installation.config" -Force
    Copy-Item "$src\scripts\Serialization\App_Config\Include\AvenueClothing.Sites.config" "$working_dir\App_Config\Include\AvenueClothing.Sites.config" -Force
}


task CopyUnicornDependenciesToLocalFolder {

    Copy-Item "$src\packages\Unicorn.Core.4.1.1\lib\net452\Unicorn.dll" "$working_dir\bin\Unicorn.dll" -Force
    Copy-Item "$src\packages\Rainbow.Core.2.1.1\lib\net452\Rainbow.dll" "$working_dir\bin\Rainbow.dll" -Force
    Copy-Item "$src\packages\Rainbow.Storage.Yaml.2.1.1\lib\net452\Rainbow.Storage.Yaml.dll" "$working_dir\bin\Rainbow.Storage.Yaml.dll" -Force
    Copy-Item "$src\packages\Rainbow.Storage.Sc.2.1.1\lib\net452\Rainbow.Storage.Sc.dll" "$working_dir\bin\Rainbow.Storage.Sc.dll" -Force

    Copy-Item "$src\packages\Kamsar.WebConsole.2.0.1\lib\net40\Kamsar.WebConsole.dll" "$working_dir\bin\Kamsar.WebConsole.dll" -Force

    Copy-Item "$src\packages\Configy.1.0.0\lib\net45\Configy.dll" "$working_dir\bin\Configy.dll" -Force
    Copy-Item "$src\packages\MicroCHAP.1.2.2.2\lib\net45\MicroCHAP.dll" "$working_dir\bin\MicroCHAP.dll" -Force

    Copy-Item "$src\..\lib\WebGrease\WebGrease.dll" "$working_dir\bin\WebGrease.dll" -Force

    Copy-Item "$src\AvenueClothing.Installer\App_Config\" "$working_dir\files" -Recurse -Force
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
    if(!(Test-Path -Path "$working_dir\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\Pipelines\Initialize\")){
        New-Item -ItemType directory -Path "$working_dir\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\Pipelines\Initialize\"
    }
    if(!(Test-Path -Path "$working_dir\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\Pipelines\Installation\")){
        New-Item -ItemType directory -Path "$working_dir\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\Pipelines\Installation\"
    }
    Copy-Item "$src\AvenueClothing.Installer\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\*" "$working_dir\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\" -Recurse -Force
}


task CopyProjectFilesToLocalFolder {    
    $options = @("/xf", "*.dll", "/xf", "*.cs", "/xf", "*.csproj", "/xf", "packages.config", "/xf", "*.user", "/xf", "*.cache", "/xd", "obj", "/xd", "bin", "/xf", "global.asax");
    
    foreach ($project in $projects) {
        ROBOCOPY "$src\$project" "$working_dir\" $options /e /s
    }
}