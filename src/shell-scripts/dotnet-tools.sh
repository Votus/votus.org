#!/usr/bin/env bash
# This script provides functions for working with DotNet Core.

set -o errexit
set -o nounset
set -o pipefail

# Ensures the correct version is installed.
provision_dotnet() {
    VERSION=$1      # Specifies the version of dotnet to provision.
    PACKAGE_PATH=$2 # Specifies the path where the dotnet package folder can be found/stored.

    echo Provisioning DotNet Core...
    initialize_dotnet $VERSION $PACKAGE_PATH

    if [ "$DOTNET_CURRENT_LOCAL_VERSION" != "$VERSION" ]; then
        echo Expected $VERSION, rectifying...

        INSTALL_SCRIPT_PATH="$(readlink -f ./src/shell-scripts/dotnet-install.sh)"
        chmod +x $INSTALL_SCRIPT_PATH
        echo Installing DotNet...
        ./src/shell-scripts/dotnet-install.sh -Version $VERSION -InstallDir $PACKAGE_PATH

        # re-initialize
        initialize_dotnet $VERSION $PACKAGE_PATH
    fi
}

initialize_dotnet() {
    VERSION=$1      # Specifies the version of dotnet to initialize.
    PACKAGE_PATH=$2 # Specifies the path where the dotnet package folder can be found/stored.

    # Configure the path so the local dotnet packages can be used, if they exist.
    PATH=$PACKAGE_PATH:$PATH

    # Opt out of stat reporting and excessive log messages...
    export DOTNET_CLI_TELEMETRY_OPTOUT=1    

    # Get the current version, if any.
    export DOTNET_CURRENT_LOCAL_VERSION="$(dotnet --version)" || "NOT INSTALLED"

    echo "Local DotNet Core Version: $DOTNET_CURRENT_LOCAL_VERSION"
}