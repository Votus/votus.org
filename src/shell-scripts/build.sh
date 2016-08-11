#!/usr/bin/env bash

echo Starting!

# Set some bash execution settings...
set -o errexit
set -o nounset
set -o pipefail

echo
echo Bash shell configured!
echo

source build-settings.sh
echo Build settings loaded!

if [ ! -d $PACKAGES_PATH ]; then
    echo "ERROR: The path $PACKAGES_PATH does not exist, run ./local-setup.sh then rerun your previous command."
    exit 1
fi

source $NODE_TOOLS_PATH
echo Script $NODE_TOOLS_PATH loaded!
initialize_node 'v6.3.1' $PACKAGES_PATH
echo
echo Provisioning Node.js modules needed for building...
$npm install
echo Done!
echo
OUTPUT_FOLDER='out'
mkdir -p $OUTPUT_FOLDER
echo Build output folder \'$OUTPUT_FOLDER\' created/exists!