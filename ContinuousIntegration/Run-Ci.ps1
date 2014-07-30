# Configure Paths
$BasePath             = [System.IO.Path]::GetFullPath(".")

$CiPath               = Join-Path $BasePath          "ContinuousIntegration"
$ToolsPath            = Join-Path $CiPath            "Tools"
$BuildNumberFilePath  = Join-Path $BasePath          "build-number.txt"
$SolutionPath         = Join-Path $BasePath          "Source\Votus.sln"
$CiOutputPath         = Join-Path $BasePath          "Ci-Output"
$CompileOutputPath    = Join-Path $CiOutputPath      "CompileOutput"
$TestOutputPath       = Join-Path $CiOutputPath      "TestOutput"
$ReleaseOutputPath    = Join-Path $CiOutputPath      "ReleaseOutput"
$WebReleaseOutputPath = Join-Path $ReleaseOutputPath "Votus.Web"
$XunitPath            = Join-Path $ToolsPath         "xunit\xunit.console.clr4.exe"

# Include external functions
Include (Join-Path "$ToolsPath" "AzureTools.ps1")
Include (Join-Path "$ToolsPath" "VotusTools.ps1")
Include (Join-Path "$ToolsPath" "DotNetTools.ps1")
Include (Join-Path "$ToolsPath" "BambooTools.ps1")

# Define input properties and default values.
properties {
    $ProductVersionNumber = "1.0"
    $BuildNumber          = "0"
    $RevisionNumber       = "0"
    $Verbosity            = "Minimal"
}

# Define the default task to run when nothing is specified.              
task default `
    -depends UnitTest                     

task OutputProperties `
    -description "Outputs the various property values being used for this run." {
    Write-Host "BasePath:                $BasePath"
    Write-Host "CiOutputPath:            $CiOutputPath"
    Write-Host "WebReleaseOutputPath:    $WebReleaseOutputPath"
    Write-Host "ProductVersionNumber:    $ProductVersionNumber"
    Write-Host "BuildNumber:             $BuildNumber"
    Write-Host "RevisionNumber:          $RevisionNumber"
}

task InstallPrerequisites {
    $ProductsToInstall      = "VWDOrVs2013AzurePack.2.3,WindowsAzurePowershell"
    $InstallHistoryFileName = Join-Path $BasePath "setup.history.ci.txt"

    Write-Host "Checking prerequisites..." -NoNewline

    if (Test-Path $InstallHistoryFileName) {
        Write-Host "reading $InstallHistoryFileName..." -NoNewline

        # Read history file to see what has been installed previously...
        $PreviousProductsString = [System.IO.File]::ReadAllText($InstallHistoryFileName)
    }

    # Only run the web platform installer if it hasn't been run previously with the current parameters.
    if ($ProductsToInstall -ne $PreviousProductsString) {
        Write-Host "installing prerequisites..."

        # Make sure prerequisite software is installed.
	    exec {webpicmd `
            /Install `
            /Products:$ProductsToInstall `
            /AcceptEula `
            /IISExpress `
            /ForceReboot}

        [System.IO.File]::WriteAllText($InstallHistoryFileName, $ProductsToInstall)
    } else {
        Write-Host "update to date!"
    }
}

# TODO: Convert this task to a function.
task SetVersionNumber `
    -description "Updates the version information that will be compiled in to the binaries." {
    $VersionNumber = "$ProductVersionNumber.$BuildNumber.$RevisionNumber"

    Write-Host "Version $VersionNumber"

    Update-AssemblyInfo `
        -FilePath "$BasePath\Source\VersionInfo.cs" `
        -Version  $VersionNumber
}

task Clean `
    -description "Removes all local artifacts that may have been left over from a previous run." {

    Stop-Process `
        -ProcessName chromedriver `
        -Force `
        -ErrorAction SilentlyContinue


    if (Test-Path $CiOutputPath) {
        Write-Host "Cleaning $CiOutputPath"

        Remove-Item `
            $CiOutputPath `
            -Recurse `
            -Force
    }
}

task Compile `
    -depends OutputProperties,Clean,SetVersionNumber,InstallPrerequisites `
    -description "Compiles the code." {
	msbuild `
        $SolutionPath `
        /p:Configuration=Release `
        /p:AllowedReferenceRelatedFileExtensions=none `
        /verbosity:$Verbosity `
        /m
}

task UnitTest `
    -depends Compile `
    -description "Unit tests the compiled code." { 
    # Make sure the test output path exists.
    New-Item `
        -ItemType Directory `
        -Path $TestOutputPath `
        -Force | Out-Null

    & $XunitPath `
        (Join-Path $CompileOutputPath     Votus.Testing.Unit.dll) `
        /nunit (Join-Path $TestOutputPath Votus.Testing.Unit.dll.xml)
}

# TODO: Convert to a function
task LoadEnvironmentConfigSettings `
    -description "Loads your configuration settings for your environment so the following tasks can manage the services within it." {
    Write-Host "Loading application configuration settings..." -NoNewline

    Load-VotusLibraries `
        -BaseDirectory $CompileOutputPath
    
    $Script:AppSettings = Get-AppSettings

    Write-Host "done!"
    Write-Host ""

    if ($AppSettings.EnvironmentName -eq $null) {
        $EnvironmentName = Read-Host "Please provide a one word name for your development environment"
        $AppSettings.EnvironmentName = $EnvironmentName
        Write-Host ""
    }

    if ($AppSettings.AzureDataCenterLocation -eq $null) {
        $AzureDataCenterLocation = Read-Host "Please specify the Azure data center location to deploy to [East US, West US, North Central US, South Central US, North Europe, West Europe, East Asia, Southeast Asia]"
        $AppSettings.AzureDataCenterLocation = $AzureDataCenterLocation
        Write-Host ""
    }
    
    $environmentName       = $AppSettings.EnvironmentName
    $appStorageAccountName = $AppSettings.AppStorageAccountName
    $dcLocation            = $AppSettings.AzureDataCenterLocation

    Write-Host "Environment Name:           $environmentName"
    Write-Host "Azure Data Center Location: $dcLocation"
    Write-Host "Azure Storage Account Name: $appStorageAccountName"
    Write-Host ""
    
    # Load the latest .publishsettings file...
    Load-AzurePublishSettings `
        -BaseDirectory   $BasePath `
        -EnvironmentName $EnvironmentName
    
    Write-Host "Getting latest storage account key..." -NoNewline

    $keys = GetOrCreate-AzureStorageAccount `
        -StorageAccountName $appStorageAccountName `
        -Location           $dcLocation |
            Get-AzureStorageKey

    $appStorageAccountKey = $keys.Primary

    Write-Host "saving storage key..." -NoNewline

    $AppSettings.AppStorageAccountKey = $appStorageAccountKey

    Write-Host "done!"

    # Set the Azure Subscription that will be used in the subsequent Azure API calls...
    Get-AzureSubscription -Default |
        Set-AzureSubscription `
            -CurrentStorageAccount $appStorageAccountName
}

task SetupEnvironment `
    -depends LoadEnvironmentConfigSettings `
    -description "Set up the runtime environment." {
    $key = GetOrCreate-AzureCacheService `
        -AppSettings $AppSettings |
            Get-AzureManagedCacheAccessKey

    $AppSettings.AzureCacheServicePrimaryAccessKey = $key.Primary

    GetOrCreate-AzureServiceBus `
        -AppSettings $AppSettings | Out-Null
}

task Deploy `
    -depends SetupEnvironment `
    -description "Deploys the binaries to the target environment." {
    Deploy-AzureWebsite `
        -WebsiteLocalFolderPath $WebReleaseOutputPath `
        -AppSettings            $AppSettings
}

task IntegrationTest `
    -depends OutputProperties `
    -description "Performs integration testing on the target environment." {
    Write-Host "Target URL:"$AppSettings.WebsiteBaseUri
    Write-Host ""

    # Make sure the test output path exists.
    New-Item `
        -ItemType Directory `
        -Path $TestOutputPath `
        -Force | Out-Null

    # Now run the integration tests!
    & $XunitPath `
        (Join-Path $CompileOutputPath     Votus.Testing.Integration.dll) `
        /nunit (Join-Path $TestOutputPath Votus.Testing.Integration.dll.xml)
}

task WipeEnvironment `
    -depends LoadEnvironmentConfigSettings `
    -description "Removes all data and services to bring the environment back to a clean state." {

    $siteName = $AppSettings.AzureWebsiteServiceName
    Write-Host "Removing Azure Website $siteName..." -NoNewline
    Remove-AzureWebsite -Name $siteName -Force -ErrorAction SilentlyContinue | Out-Null
    Write-Host "done!"

    $cacheName = $AppSettings.AzureCacheServiceName
    Write-Host "Removing Azure Managed Cache $cacheName..." -NoNewline
    Remove-AzureManagedCache -Name $cacheName -Force -ErrorAction SilentlyContinue | Out-Null
    Write-Host "done!"

    $storeName = $AppSettings.AppStorageAccountName
    Write-Host "Removing Azure Storage Account $storeName..." -NoNewline
    Remove-AzureStorageAccount -StorageAccountName $storeName -ErrorAction SilentlyContinue | Out-Null
    $AppSettings.AppStorageAccountKey = $null
    Write-Host "done!"

    $serviceName = $AppSettings.AzureServiceBusNamespace
    Write-Host "Removing Azure Service Bus Namespace $serviceName..." -NoNewline
    Remove-AzureSBNamespace -Name $serviceName -Force -ErrorAction SilentlyContinue | Out-Null
    $AppSettings.AzureServiceBusSecretValue = $null
    Write-Host "done!"
}

task Pull `
    -depends OutputProperties `
    -description "Gets the latest version of the code from the remote source control." {
    git pull
}
                                                                         
task Push `
    -depends Pull,FullCI `
    -description "Syncs your latest changes with the latest code in remote source control and then tests it." {
    git push
}
 
task FullCI `
    -depends UnitTest,Deploy,IntegrationTest `
    -description "Performs the tasks needed to compile, deploy and test from scratch to the extent that the release can be promoted to production if it all passes." {
}

task ProdCI `
    -depends Deploy,IntegrationTest `
    -description "Performs the tasks needed to deploy the latest release to production and verify the deployment. This task is mainly to be called by the build/ci system." {
}
                                                                         
# Runs automatically after each task.                                    
TaskTearDown {
	if ($LASTEXITCODE -ne $null -and $LASTEXITCODE -ne 0) {
        $message = "Task failed with exit code '$LASTEXITCODE', see previous errors for more information."
		throw $message
	}

	"" # Adds a blank line between the log output from the tasks.
}