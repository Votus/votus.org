#!/usr/bin/env bash

echo Starting!

set -o errexit
set -o nounset
set -o pipefail

echo
echo Bash shell configured!
echo

PACKAGES_FOLDER='packages'
PACKAGES_PATH="$(readlink -f $PACKAGES_FOLDER)"
SHELL_SCRIPTS_PATH="$(readlink -f ./src/shell-scripts)"
NODE_TOOLS_PATH="$(readlink -f $SHELL_SCRIPTS_PATH/node.sh)"
source $NODE_TOOLS_PATH
echo Script $NODE_TOOLS_PATH loaded!
OUTPUT_FOLDER='out'
mkdir -p $OUTPUT_FOLDER
echo Folder \'$OUTPUT_FOLDER\' created/exists!
initialize_node 'v6.3.1' $PACKAGES_PATH
echo
echo Provisioning Node.js modules...
$npm install
echo Done!