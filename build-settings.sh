#!/usr/bin/env bash

set -o errexit
set -o nounset
set -o pipefail

# Static settings
PACKAGES_FOLDER='packages'                              # Defines the name of the folder where build dependencies should be cached.

# Calculated settings
export PACKAGES_PATH="$(readlink -f $PACKAGES_FOLDER)"  # Specifies the full local path to the packages folder.