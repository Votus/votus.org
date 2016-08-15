#!/usr/bin/env bash

set -o errexit
set -o nounset
set -o pipefail

# Static settings
SOURCE_FOLDER='src'                                                 # Defines the name of the folder where source code should be stored.
OUTPUT_FOLDER='out'                                                 # Defines the name of the folder where the build output should be stored.
PACKAGES_FOLDER='packages'                                          # Defines the name of the folder where build dependencies should be cached.

# Calculated settings
export SOURCE_PATH="$(readlink -f $SOURCE_FOLDER)"                  # Specifies the full local path to the source folder.
export OUTPUT_PATH="$(readlink -f $OUTPUT_FOLDER)"                  # Specifies the full local path to the output folder.
export WEBSITE_PATH="$(readlink -f $SOURCE_FOLDER/website)"         # Specifies the full local path to the website folder.
export PACKAGES_PATH="$(readlink -f $PACKAGES_FOLDER)"              # Specifies the full local path to the packages folder.
export SHELL_SCRIPTS_PATH="$(readlink -f ./src/shell-scripts)"      # Specifies the full local path to the shell scripts folder.
export NODE_TOOLS_PATH="$(readlink -f $SHELL_SCRIPTS_PATH/node.sh)" # Specifies the full local path to the node shell tools.