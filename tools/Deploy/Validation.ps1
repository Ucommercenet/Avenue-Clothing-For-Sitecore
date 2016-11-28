task ValidateSetup -description "Validates the setup prerequirement" {

    if((IsVersionNumber $version))
    {
        $script:version = "$version." + (Get-Date).Year.ToString().Substring(2) + "" + (Get-Date).DayOfYear.ToString("000")
    }
    else
    {
        throw "Version '$version' is not a valid string. Use: x.x.x"
    }

    Assert($base_dir -ne $null) "base_dir should never be null. This should be specifed in the call powershell script file."
}