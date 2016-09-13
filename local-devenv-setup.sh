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

# Ensure the correct version of dotnet is used.
source $DOTNET_TOOLS_PATH
echo Script $DOTNET_TOOLS_PATH loaded!
provision_dotnet '1.0.0-preview2-003121' $DOTNET_PACKAGE_PATH
echo
echo Build dependency provisioning complete!