task DeploySitecoreLocal -depends SetSynchronizeSitecoreItemsPath, CopyBinariesToLocalFolder,CopyUnicornDependenciesToLocalFolder,CopyConfigurationLocal,CopyProjectFilesToLocalFolder

task SetSynchronizeSitecoreItemsPath{
  $path = "$src\AvenueClothing.Installer\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\Pipelines\Installation\Installation.config"
  $xml = [xml](Get-Content $path)  

  $SynchronizeSitecoreItemsComponent = $xml.configuration.components.component| where {$_.id -eq 'AvenueClothing.InstallationPipeline.SynchronizeSitecoreItems'}

  $path = "$src\project\AvenueClothing\serialization";
  if($Apis -eq "CommerceConnect"){
    $path = "$src\project\AvenueClothing-CC\serialization";
  }
  
  $SynchronizeSitecoreItemsComponent.parameters.SynchronizeSitecoreItemsPath = $path;
  if(!(Test-Path -Path "$working_dir\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\Pipelines\Installation\")){
        New-Item -ItemType directory -Path "$working_dir\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\Pipelines\Installation\"
  }
  $xml.Save("$working_dir\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\Pipelines\Installation\Installation.config")
}

task CopyUnicornDependenciesToLocalFolder {
    Copy-Item "$src\packages\Unicorn.Core.3.3.2\lib\net452\Unicorn.dll" "$working_dir\bin\Unicorn.dll" -Force 
    Copy-Item "$src\packages\Rainbow.Core.1.4.1\lib\net452\Rainbow.dll" "$working_dir\bin\Rainbow.dll" -Force 
    Copy-Item "$src\packages\Rainbow.Storage.Yaml.1.4.1\lib\net452\Rainbow.Storage.Yaml.dll" "$working_dir\bin\Rainbow.Storage.Yaml.dll" -Force 
    Copy-Item "$src\packages\Rainbow.Storage.Sc.1.4.1\lib\net452\Rainbow.Storage.Sc.dll" "$working_dir\bin\Rainbow.Storage.Sc.dll" -Force 
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
    Copy-Item "$src\packages\Rainbow.1.3.1\content\App_Config\Include\Rainbow.config" "$working_dir\App_Config\Include\Rainbow.config" -Force 
    Copy-Item "$src\packages\Unicorn.3.2.0\content\App_Config\Include\Unicorn\Unicorn.config" "$working_dir\App_Config\Include\unicorn.config" -Force
    Copy-Item "$src\scripts\Serialization\App_Config\Include\AvenueClothing.Serialization.config" "$working_dir\App_Config\Include\" -Force
    Copy-Item "$src\scripts\Serialization\App_Config\Include\AvenueClothing.Sites.config" "$working_dir\App_Config\Include" -Force
    
    
    if(!(Test-Path -Path "$working_dir\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\Pipelines\Initialize\")){
        New-Item -ItemType directory -Path "$working_dir\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\Pipelines\Initialize\"
    }

    Copy-Item "$src\AvenueClothing.Installer\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\Pipelines\Initialize\Initialize.config" "$working_dir\sitecore modules\Shell\uCommerce\Apps\Avenue Clothing\Pipelines\Initialize\" -Recurse -Force
}


task CopyProjectFilesToLocalFolder {    
    $options = @("/xf", "*.dll", "/xf", "*.cs", "/xf", "*.csproj", "/xf", "packages.config", "/xf", "*.user", "/xf", "*.cache", "/xd", "obj", "/xd", "bin", "/xf", "global.asax");
    
    foreach ($project in $projects) {
        ROBOCOPY "$src\$project" "$working_dir\" $options /e /s
    }
}