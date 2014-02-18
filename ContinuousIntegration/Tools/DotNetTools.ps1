# This file contains functions specific to working with .NET related things.


function Update-AssemblyInfo
{
    Param (
        [string]$FilePath, 
        [string]$Version
    )

    $NewVersion     = 'AssemblyVersion("' + $Version + '")';
    $NewFileVersion = 'AssemblyFileVersion("' + $Version + '")';
  
    $content = Get-Content $FilePath

    # Update the contents and save them back to the original file.
    $content |
        %{$_ -replace 'AssemblyVersion\(.*\)', $NewVersion } |
        %{$_ -replace 'AssemblyFileVersion\(.*\)', $NewFileVersion }  | Out-File $FilePath -Encoding utf8

}