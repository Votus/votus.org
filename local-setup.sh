#!/usr/bin/env bash

echo Starting build dependency provisioning!

# Set some bash execution settings...
set -o errexit
set -o nounset
set -o pipefail
echo
echo Bash shell configured!
echo

# Load scripts and settings...
source build-settings.sh
echo Build settings loaded!

# Ensure the packages directory exists.
mkdir -p $PACKAGES_PATH
echo Folder \'$PACKAGES_PATH\' created/exists!
echo

# Load OS scripting tools.
source $OS_TOOLS_PATH
echo Script $OS_TOOLS_PATH loaded!
echo_os
echo

# Ensure the correct version of node is used.
source $NODE_TOOLS_PATH
echo Script $NODE_TOOLS_PATH loaded!
provision_node 'v6.3.1' $PACKAGES_PATH
echo

# Ensure the correct version of dotnet is used.
source $DOTNET_TOOLS_PATH
echo Script $DOTNET_TOOLS_PATH loaded!
#provision_dotnet '1.0.0-preview2-003121' $PACKAGES_PATH
echo DOTNET:
dotnet --version
echo
echo Build dependency provisioning complete!


