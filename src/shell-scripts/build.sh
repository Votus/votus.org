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

source $DOTNET_TOOLS_PATH
echo Script $DOTNET_TOOLS_PATH loaded!
initialize_dotnet '1.0.0-preview2-003121' $DOTNET_PACKAGE_PATH
echo

mkdir -p $OUTPUT_PATH
echo Build output folder \'$OUTPUT_PATH\' created/exists!
echo
echo Compiling the website...
echo Restoring...
dotnet restore "$WEBSITE_PATH/project.json" 
echo Building...
dotnet publish "$WEBSITE_PATH/project.json" --configuration Release --framework netcoreapp1.0 --output "$OUTPUT_PATH/website"