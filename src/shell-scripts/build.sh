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

source $DOTNET_TOOLS_PATH
echo Script $DOTNET_TOOLS_PATH loaded!
initialize_dotnet '1.0.0-preview2-003121' $PACKAGES_PATH
echo

mkdir -p $OUTPUT_PATH
echo Build output folder \'$OUTPUT_PATH\' created/exists!
echo
echo Compiling the website...
export COREHOST_TRACE=1
export DOTNET_CLI_TELEMETRY_OPTOUT=1
echo Restoring...
$dotnet restore "$WEBSITE_PATH/project.json" 
echo Building...
$dotnet build "$WEBSITE_PATH/project.json" --configuration Release --framework netcoreapp1.0 --output "$OUTPUT_PATH"
echo