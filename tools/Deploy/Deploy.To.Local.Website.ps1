[CmdletBinding()]
Param()

function Get-ScriptDirectory { 
    Split-Path -parent $PSCommandPath 
}

function Run-It () {
    try {  
        $scriptPath = Get-ScriptDirectory

        $src = Resolve-Path "$scriptPath\..\..\src";

	    Import-Module "$scriptPath\..\psake\4.3.0.0\psake.psm1"
    
        $properties = @{
                "src"=$src;
            };

		Write-Host "Starting deployment"
        Invoke-psake "$scriptPath\DeployDemoStoreToLocalWebsite.ps1" "DeployDemoStoreToLocalWebsite" -properties $properties

    } catch {  
        Write-Error $_.Exception.Message -ErrorAction Stop  
    }
}

Run-It