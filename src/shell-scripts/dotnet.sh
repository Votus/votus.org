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

    DOTNET_PACKAGE_PATH="$(readlink -f $PACKAGES_PATH/dotnet)"

    # OS check, fail if unsupported.
    CURRENT_OS="$(uname)"

    # Determine the correct node version to use based on the OS.
    case $CURRENT_OS in
        'MINGW64_NT-10.0')
            powershell ./src/shell-scripts/dotnet-install.ps1 -Version $VERSION -InstallDir $DOTNET_PACKAGE_PATH
            export dotnet=$DOTNET_PACKAGE_PATH/dotnet.exe
            ;;
        'Linux')
            # This code may be specific to the particular linux distribution Travi-CI is using.
            # Found here: https://www.microsoft.com/net/core#ubuntu
            sudo sh -c 'echo "deb [arch=amd64] https://apt-mo.trafficmanager.net/repos/dotnet-release/ trusty main" > /etc/apt/sources.list.d/dotnetdev.list'
            sudo apt-key adv --keyserver apt-mo.trafficmanager.net --recv-keys 417A0893
            echo Updating...
            sudo apt-get update
            echo Upgrading...
            sudo apt-get -o Dpkg::Options::="--force-confnew" -q -y upgrade
            echo Installing dotnet-dev-1.0.0-preview2-003121...
            sudo apt-get install dotnet-dev-1.0.0-preview2-003121
#            echo Updating...
#            sudo apt-get update
#            echo Installing...
#            sudo apt-get install libunwind8-dev
#            INSTALL_SCRIPT_PATH="$(readlink -f ./src/shell-scripts/dotnet-install.sh)"
#            chmod +x $INSTALL_SCRIPT_PATH
#            echo Installing DotNet...
#            ./src/shell-scripts/dotnet-install.sh -Version $VERSION -InstallDir $DOTNET_PACKAGE_PATH
            ;;
        *)
            (>&2 echo "ERROR: Your OS is $CURRENT_OS is not supported.")
            exit 1
            ;;
    esac
}