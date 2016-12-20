task DeploySitecoreLocal -depends CopyBinariesToFilesFolder,CopyUnicornDependenciesToFilesFolder,CopyConfigurationFiles, CopyProjectFilesToFilesFolder

}
  

task SetSitecoreVars -description "Set paths for Deploy.To.Package." {
    $script:hash.ucommerce_dir = "$working_dir\files\sitecore modules\Shell\ucommerce" 
    $script:hash.bin_dir = "$working_dir\files\bin"
    $script:hash.files_root_dir = "$working_dir\files"
}

task CopyUnicornDependenciesToFilesFolder {
    Copy-Item "$src\packages\Unicorn.Core.3.3.2\lib\net452\Unicorn.dll" "$working_dir\bin\Unicorn.dll" -Force 
    Copy-Item "$src\packages\Rainbow.Core.1.4.1\lib\net452\Rainbow.dll" "$working_dir\bin\Rainbow.dll" -Force 
    Copy-Item "$src\packages\Rainbow.Storage.Yaml.1.4.1\lib\net452\Rainbow.Storage.Yaml.dll" "$working_dir\bin\Rainbow.Storage.Yaml.dll" -Force 
    Copy-Item "$src\packages\Rainbow.Storage.Sc.1.4.1\lib\net452\Rainbow.Storage.Sc.dll" "$working_dir\bin\Rainbow.Storage.Sc.dll" -Force 
}

task CopyBinariesToFilesFolder {
    foreach ($project in $projects) {
        Copy-Item "$src\$project\bin\$project.dll" "$working_dir\bin" -Force
        if ($Configuration -eq "Debug") {
            Copy-Item "$src\$project\bin\$project.pdb" "$working_dir\bin" -Force            
        }
    }
}

task CopyConfigurationFiles {
    Copy-Item "$src\packages\Rainbow.1.3.1\content\App_Config\Include\Rainbow.config" "$working_dir\App_Config\Include\Rainbow.config" -Force 
    Copy-Item "$src\packages\Unicorn.3.2.0\content\App_Config\Include\Unicorn\Unicorn.config" "$working_dir\App_Config\Include\unicorn.config" -Force
    Copy-Item "$src\scripts\Serialization\App_Config\Include\AvenueClothing.Serialization.config" "$working_dir\sitecore modules\Shell\ucommerce\install\config_include\" -Force
    Copy-Item "$src\scripts\Serialization\App_Config\Include\AvenueClothing.Sites.config" "$working_dir\sitecore modules\Shell\ucommerce\install\config_include\" -Force
}


task CopyProjectFilesToFilesFolder {    
    $options = @("/xf", "*.dll", "/xf", "*.cs", "/xf", "*.csproj", "/xf", "packages.config", "/xf", "*.user", "/xf", "*.cache", "/xd", "obj", "/xd", "bin", "/xf", "global.asax");
    
    foreach ($project in $projects) {
        ROBOCOPY "$src\$project" "$working_dir\files" $options /e /s
    }
}