function Get-BuildNumber
{
    Param (
        [string]$BambooBuildNumberFilePath
    )
    
    if (Test-Path $BambooBuildNumberFilePath) {
        # Define the line in the Bamboo build file that contains the build number.
        $pattern = "build.number=(?<number>[\d]+)"

        # Extract the build number from the file and return it.
        Get-Content $BambooBuildNumberFilePath |
            Select-String $pattern |
            %{ $null = $_.Line -match $pattern; $matches['number'] }
    } else { 
        # Return a 0!
        0
    }
}