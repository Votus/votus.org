#!/usr/bin/env bash

echo Starting build dependency provisioning!

# Set some bash execution settings...
set -o errexit
set -o nounset
set -o pipefail

echo
echo Bash shell configured!
echo

source build-settings.sh
echo Build settings loaded!

source $NODE_TOOLS_PATH
echo Script $NODE_TOOLS_PATH loaded!

mkdir -p $PACKAGES_PATH
echo Folder \'$PACKAGES_PATH\' created/exists!
echo
provision_node 'v6.3.1' $PACKAGES_PATH
echo
echo Build dependency provisioning complete!