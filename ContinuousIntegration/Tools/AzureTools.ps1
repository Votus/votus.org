# This file contains functions specific to working with Azure related things.

function Load-AzurePublishSettings {
    Param(
        $BaseDirectory,
        $EnvironmentName
    )

    Write-Host "Setting the current Azure subscription..." -NoNewline

    $subscriptions = Get-AzureSubscription

    foreach ($subscription in $subscriptions) {
        $name = $subscription.SubscriptionName

        if ($name.Contains($EnvironmentName)) {
            Write-Host "using subscription '$name'..." -NoNewline

            $subscription |
                Select-AzureSubscription `
                    -Current |
                Set-AzureSubscription `
                    -CurrentStorageAccountName $appStorageAccountName

            break
        }
    }
    
    Write-Host "done!"
    Write-Host ""
}

function GetOrCreate-AzureServiceBus {
    Param(
        $AppSettings
    )

    $name = $AppSettings.AzureServiceBusNamespace
    $loc  = $AppSettings.AzureDataCenterLocation

    Write-Host "Checking for Service Bus Namespace $name ($loc)..." -NoNewline

    $sbNamespace = Get-AzureSBNamespace `
        -Name $name

    if ($sbNamespace -eq $null) {
        Write-Host "creating..." -NoNewline

        $sbNamespace = New-AzureSBNamespace `
            -Name     $name `
            -Location $loc

        Write-Host "created..." -NoNewline
    }

    $KeyLength = 44
    $key = $sbNamespace.ConnectionString.Substring($sbNamespace.ConnectionString.Length - $KeyLength, $KeyLength);

    Write-Host "saving key..." -NoNewline
    
    $AppSettings.AzureServiceBusSecretValue = $key

    Write-Host "done!"
    Write-Output $sbNamespace
}

function Deploy-AzureWebsite {
    Param(
        $WebsiteLocalFolderPath,
        $AppSettings
    )

    # Change the current location because running some of the next few commands place
    # artifacts in the 'current' directory, this way the artifacts go to the correct location.
    $oldLocation = Get-Location
    Set-Location -Path $WebsiteLocalFolderPath

    # Get some config settings...
    $AzureServiceName = $AppSettings.AzureWebsiteServiceName

    # Assemble the git URL to push to...
    $pushUrl = "https://$AzureServiceName.scm.azurewebsites.net/$AzureServiceName.git"

    Write-Host ("Attempting to deploy to {0}..." -f $AzureServiceName)

    # Make sure the service exists...
    $website = GetOrCreate-AzureWebsite `
        -WebsiteLocalFolderPath $WebsiteLocalFolderPath `
        -GitPushUrl             $pushUrl `
        -AppSettings            $AppSettings

    # Push via git
    git add *
    git commit -m "Updating deployment."
    git push $pushUrl master --force

    # Revert the current location before exiting...
    Set-Location -Path $oldLocation

    # Sometimes HostNames is a string with a single hostname, sometimes it's an array...
    if ($website.HostNames -is [System.String[]]){
        $deploymentHostname = $website.HostNames[0]
    } else {
        $deploymentHostname = $website.HostNames
    }

    $baseUrl            = "http://$deploymentHostname"
    $baseUrlConfigName  = [Votus.Core.ApplicationSettings]::VotusWebsiteBaseUrlConfigName

    [System.Environment]::SetEnvironmentVariable($baseUrlConfigName, $baseUrl, "Process")
}

function GetOrCreate-AzureWebsite {
    Param(
        $WebsiteLocalFolderPath,
        $GitPushUrl,
        $AppSettings
    )

    $WebsiteName        = $AppSettings.AzureWebsiteServiceName
    $DataCenterLocation = $AppSettings.AzureDataCenterLocation

    try {
        $website = Get-AzureWebsite -Name $WebsiteName

        if ($website -eq $null){
            throw "The website $WebsiteName was not found."
        }
    }
    catch [Exception] {
		[string]$message = $_.Exception.Message

		if ($message.Contains("The website $WebsiteName was not found.") -eq $false -and `
            $message.Contains("(404) Not Found") -eq $false) 
        {
			throw $_.Exception
        }

        Write-Host "The website doesn't exist, creating..."

        $website = New-AzureWebsite `
            -Git `
            -Name     $WebsiteName `
            -Location $DataCenterLocation

        Write-Host "done!"
	}

    $settingsTable = New-Object Hashtable
    $settingsTable[[Votus.Core.ApplicationSettings]::EnvironmentNameConfigName]            = $AppSettings.EnvironmentName
    $settingsTable[[Votus.Core.ApplicationSettings]::AzureDataCenterLocationConfigName]    = $AppSettings.AzureDataCenterLocation
    $settingsTable[[Votus.Core.ApplicationSettings]::AppStorageAccountKeyConfigName]       = $AppSettings.AppStorageAccountKey
    $settingsTable[[Votus.Core.ApplicationSettings]::AzureServiceBusSecretValueConfigName] = $AppSettings.AzureServiceBusSecretValue

    Write-Host "Updating the websites settings..."

    $website |
        Set-AzureWebsite `
            -AppSettings                 $settingsTable `
            -PhpVersion                  Off `
            -HttpLoggingEnabled          $True `
            -DetailedErrorLoggingEnabled $True `
            -RequestTracingEnabled       $True

    if (-Not(Test-Path ".git")) {
        Write-Host "Initializing local git repo in"(Get-Location)
        git init

        Write-Host "Registering azure remote to $GitPushUrl"
        git remote add azure $GitPushUrl
    }

    git config --local user.email "bamboo@votus.org"
    git config --local user.name  "Bamboo-CI"

    Write-Output $website
}

function GetOrCreate-AzureStorageAccount {
    Param(
        $StorageAccountName,
        $Location
    )

    try {
        $account = Get-AzureStorageAccount `
            -StorageAccountName $StorageAccountName
    }
    catch [System.Exception] {
        $exceptionMessage = $_.Exception.Message

        if ($exceptionMessage -like "*ResourceNotFound*") {
            Write-Host "$StorageAccountName doesn't exist, creating..." -NoNewline

            New-AzureStorageAccount `
                -StorageAccountName $StorageAccountName `
                -Location           $Location | Out-Null

            $account = Get-AzureStorageAccount `
                -StorageAccountName $StorageAccountName
        } 
        else { throw }
    }

    Write-Output $account
}