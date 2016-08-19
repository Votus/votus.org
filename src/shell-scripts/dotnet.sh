#!/usr/bin/env bash
# This script provides functions for working with DotNet Core.

set -o errexit
set -o nounset
set -o pipefail

# Ensures the correct version is installed.
provision_dotnet() {
    VERSION=$1          # Specifies the version of dotnet to provision.
    PACKAGES_PATH=$2    # Specifies the path where the dotnet package folder can be found/stored.

    echo Provisioning DotNet Core...

    DOTNET_PACKAGE_PATH=$PACKAGES_PATH/dotnet

    # OS check, fail if unsupported.
    CURRENT_OS="$(uname)"

    # Determine the correct node version to use based on the OS.
    case $CURRENT_OS in
        'MINGW64_NT-10.0')
            powershell ./src/shell-scripts/dotnet-install.ps1 -Version $VERSION -InstallDir $DOTNET_PACKAGE_PATH
            ;;
        'Linux')
            echo Updating...
            sudo apt-get update

            echo Installing...
            sudo apt-get install libunwind8

            INSTALL_SCRIPT_PATH="$(readlink -f ./src/shell-scripts/dotnet-install.sh)"
            chmod +x $INSTALL_SCRIPT_PATH
            echo Installing DotNet...
            ./src/shell-scripts/dotnet-install.sh -Version $VERSION -InstallDir $DOTNET_PACKAGE_PATH
            ;;
        *)
            (>&2 echo "ERROR: Your OS is $CURRENT_OS is not supported.")
            exit 1
            ;;
    esac
}

initialize_dotnet() {
    VERSION=$1          # Specifies the version of dotnet to initialize.
    PACKAGES_PATH=$2    # Specifies the path where the dotnet package folder can be found/stored.

    export DOTNET_CLI_TELEMETRY_OPTOUT=1
    export dotnet=$PACKAGES_PATH/dotnet/dotnet

    CURRENT_OS="$(uname)"

    echo OS: $CURRENT_OS
    
    if [ $CURRENT_OS = "MINGW64_NT-10.0" ]; then
        # Use winpty to execute dotnet to avoid annoying errors.
        export dotnet="winpty $dotnet"
        echo Updated dotnet from '$dotnet' to 'winpty $dotnet'.
    fi
}