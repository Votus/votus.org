#!/usr/bin/env bash
# This script provides functions for working with Node.js.

set -o errexit
set -o nounset
set -o pipefail

# Ensures the correct version is installed.
provision_node() {
    NODE_VERSION=$1
    PACKAGES_PATH=$2

    echo Provisioning Node.js $NODE_VERSION...
    initialize_node $NODE_VERSION $PACKAGES_PATH

    if [ "$NODE_CURRENT_VERSION" != "$NODE_VERSION" ]; then
        echo Expected $NODE_VERSION, downloading...
        curl -sL https://deb.nodesource.com/setup_6.x | sudo -E bash -
        echo Installing...
        sudo apt-get -y install nodejs=6.5.0-1nodesource1~trusty1
    fi
}

# Ensures the local system is configured to run node.
initialize_node() {
    NODE_VERSION=$1
    PACKAGES_PATH=$2

    # Check current node version, if any
    export NODE_CURRENT_VERSION="$(node --version)" || true
}