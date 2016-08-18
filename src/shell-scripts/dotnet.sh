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
            sudo apt-get update
            sudo apt-get install curl libunwind8 gettext
            INSTALL_SCRIPT_PATH="$(readlink -f ./src/shell-scripts/dotnet-install.sh)"
            chmod +x $INSTALL_SCRIPT_PATH
            ./src/shell-scripts/dotnet-install.sh -Version $VERSION -InstallDir $DOTNET_PACKAGE_PATH
            ;;
        *)
            (>&2 echo "ERROR: Your OS is $CURRENT_OS is not supported.")
            exit 1
            ;;
    esac
}