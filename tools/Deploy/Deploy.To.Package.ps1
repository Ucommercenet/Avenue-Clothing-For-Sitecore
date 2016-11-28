[CmdletBinding()]
Param(
    [Parameter(Mandatory=$False)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [Parameter(Mandatory=$False)]
    [string]$Version = "current"
)

function Get-ScriptDirectory { 
    Split-Path -parent $PSCommandPath 
}

function Run-It () {
  
        $scriptPath = Get-ScriptDirectory
        . "$scriptPath\Deploy.Common.ps1"

        $working_directories = GetWorkingDirectories;
    
        $src = Resolve-Path "$scriptPath\..\..\src";
        $base_dir = Resolve-Path "$scriptPath\..\.."

	    Import-Module "$scriptPath\..\psake\4.3.0.0\psake.psm1"
    
        $properties = @{
                "configuration"="$Configuration"; 
                "version"="$Version";
                "base_dir"="$base_dir";
                "src"=$src;
                "working_dir"=$working_directories["Sitecore"]
                "target"="Sitecore"
            };
    
        Invoke-PSake "$scriptPath\uCommerce.build.ps1" "CreateSitecorePackage" -properties $properties
        

}

Run-It