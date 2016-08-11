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

SHELL_SCRIPTS_PATH="$(readlink -f ./src/shell-scripts)"
NODE_TOOLS_PATH="$(readlink -f $SHELL_SCRIPTS_PATH/node.sh)"
source $NODE_TOOLS_PATH
echo Script $NODE_TOOLS_PATH loaded!
mkdir -p $PACKAGES_PATH
echo Folder \'$PACKAGES_PATH\' created/exists!
echo
provision_node 'v6.3.1' $PACKAGES_PATH
echo
echo Build dependency provisioning complete!