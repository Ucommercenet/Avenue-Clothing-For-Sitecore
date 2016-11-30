function GetWorkingDirectories{
    $working_directories = @{
        "Sitecore"=(Get-Item $env:TEMP).FullName + "\uCommerceTmp\accelerator"
    };

    return $working_directories;
}

function IsVersionNumber($thisVersion){
    return [System.Text.RegularExpressions.Regex]::IsMatch($thisVersion, "^[0-9]{1,5}(\.[0-9]{1,5}){2}$")
}

function CopyFiles($srcDirectory, $dstDirectory, $files){
    foreach($file in $files)
    {
        if(Test-Path "$srcDirectory\$file")
        {
            Copy-Item "$srcDirectory\$file" "$dstDirectory\$file" -Force
        }
    }
}

function MoveFiles($srcDirectory, $dstDirectory, $files){
    foreach($file in $files)
    {
        if(Test-Path "$srcDirectory\$file")
        {
            Move-Item "$srcDirectory\$file" "$dstDirectory\$file" -Force
        }
        else{
            throw "source file '$srcDirectory\$file' does not exist"
        }
    }
}

function RemoveFiles($directory, $files){
    foreach($file in $files)
    {
        if(Test-Path "$directory\$file")
        {
            Remove-Item "$directory\$file" -Force
        }
    }
}

function GetZipFilename {
    $newFileName = [string]::Format($zipDestinationFolder + "\" + $zipFileName, $target, $script:version)

    # Make sure to delete the file else 7Zip will append files to the zip.
    if(Test-Path $newFileName)
    {
        Remove-Item $newFileName -Force
    }

    return $newFileName;
}