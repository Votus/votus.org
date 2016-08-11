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

    NODE_CURRENT_VERSION="$($node --version)" || true

    if [ "$NODE_CURRENT_VERSION" != "$NODE_VERSION" ]; then
        echo Expected $NODE_VERSION
        
        if [ ! -f $NODE_INSTALL_FILE_LOCAL_PATH ]; then
            echo Downloading $NODE_DIST_URL...
            echo
            curl $NODE_DIST_URL > $NODE_INSTALL_FILE_LOCAL_PATH
            echo
            echo Download complete, saved to $NODE_INSTALL_FILE_LOCAL_PATH
        fi
        
        if [ ! -d $NODE_PACKAGE_PATH ]; then
            echo Extracting $NODE_INSTALL_FILE to $PACKAGES_PATH...
            $EXTRACT_TOOL $NODE_INSTALL_FILE_LOCAL_PATH
            echo Extraction complete!
        fi
    fi
}

# Ensures the local system is configured to run node.
initialize_node() {
    NODE_VERSION=$1
    PACKAGES_PATH=$2

    # OS check, fail if unsupported.
    CURRENT_OS="$(uname)"
    SUPPORTED_OS_WIN='MINGW64_NT-10.0'
    SUPPORTED_OS_LINUX='Linux'

    # Determine the correct node version to use based on the OS.
    case $CURRENT_OS in
        $SUPPORTED_OS_WIN)
            NODE_PACKAGE_NAME="node-$NODE_VERSION-win-x64"
            NODE_INSTALL_FILE="$NODE_PACKAGE_NAME.zip"
            EXTRACT_TOOL="unzip -q -d $PACKAGES_PATH"
            NODE_PACKAGE_PATH="$PACKAGES_PATH/$NODE_PACKAGE_NAME"
            ;;
        $SUPPORTED_OS_LINUX)
            NODE_PACKAGE_NAME="node-$NODE_VERSION-linux-x64"
            NODE_INSTALL_FILE="$NODE_PACKAGE_NAME.tar.gz"
            EXTRACT_TOOL="tar -C $PACKAGES_PATH -xf"
            NODE_PACKAGE_PATH="$PACKAGES_PATH/$NODE_PACKAGE_NAME/bin"
            ;;
        *)
            (>&2 echo "ERROR: Your OS is $CURRENT_OS, only $SUPPORTED_OS is supported.")
            exit 1
            ;;
    esac

    NODE_DIST_URL=https://nodejs.org/dist/$NODE_VERSION/$NODE_INSTALL_FILE
    NODE_INSTALL_FILE_LOCAL_PATH="$PACKAGES_PATH/$NODE_INSTALL_FILE"

    export node="$NODE_PACKAGE_PATH/node"
    export npm="$NODE_PACKAGE_PATH/npm"

    echo
    echo $node $($node --version)
    echo $npm $($npm --version)
    echo
    echo Paths configured!
}