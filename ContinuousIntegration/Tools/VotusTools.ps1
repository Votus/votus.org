# This file contains functions specific to working with Votus related things.

function Load-VotusLibraries {
    Param(
        $BaseDirectory
    )

    $coreDllPath = Join-Path `
        $BaseDirectory Votus.Core.dll

    Add-Type `
        -Path $coreDllPath
}

function Set-ConfigSetting {
    Param(
        $SettingName,
        $SettingValue
    )

    # Save to environment variables for now...
    [Environment]::SetEnvironmentVariable($SettingName, $SettingValue, "User")
}

function Get-DiKernel {
    if ($Kernel -eq $null){
        # Instantiate the injection module for the config infrastructure.
        $coreInjectionModule = New-Object Votus.Core.CoreInjectionModule

        # Instantiate a Ninject Kernel with our injection module.
        $Kernel = New-Object Ninject.StandardKernel `
            $coreInjectionModule

        # Store the kernel at the script level.
        $Script:Kernel = $Kernel
    }

    Write-Output $Kernel
}

function Get-Instance {
    Param(
        [System.Type]$Type
    )

    $di = Get-DiKernel
    Write-Output ([Ninject.ResolutionExtensions]::Get($di, $Type))
}

function Get-AppSettings {
    # Get an instance of the ApplicationSettings from Ninject
    $appSettings = Get-Instance -Type Votus.Core.ApplicationSettings
    
    # Output the appSettings
    Write-Output $appSettings
}