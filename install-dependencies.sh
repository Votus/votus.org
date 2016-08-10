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

mkdir -p $PACKAGES_PATH
echo Folder \'$PACKAGES_PATH\' created/exists!
echo

# OS check, fail if unsupported.
SUPPORTED_OS_WIN='MINGW64_NT-10.0'
SUPPORTED_OS_LINUX='Linux'
CURRENT_OS="$(uname)"

NODE_VERSION='v6.3.1'

# Determine the correct node version to use based on the OS.
case $CURRENT_OS in
    $SUPPORTED_OS_WIN)
        NODE_PACKAGE_NAME="node-$NODE_VERSION-win-x64"
        NODE_INSTALL_FILE="$NODE_PACKAGE_NAME.zip"
        ;;
    $SUPPORTED_OS_LINUX)
        NODE_PACKAGE_NAME="node-$NODE_VERSION-linux-x64"
        NODE_INSTALL_FILE="$NODE_PACKAGE_NAME.tar.gz"
        ;;
    *)
        (>&2 echo "ERROR: Your OS is $CURRENT_OS, only $SUPPORTED_OS is supported.")
        exit 1
        ;;
esac

NODE_PACKAGE_PATH="$PACKAGES_PATH/$NODE_PACKAGE_NAME"

NODE_DIST_URL=https://nodejs.org/dist/$NODE_VERSION/$NODE_INSTALL_FILE
NODE_INSTALL_FILE_LOCAL_PATH="$PACKAGES_PATH/$NODE_INSTALL_FILE"

echo Provisioning Node.js $NODE_VERSION...
node=$NODE_PACKAGE_PATH/node
npm=$NODE_PACKAGE_PATH/npm
echo Paths configured!

NODE_CURRENT_VERSION="$($node --version)" || true

if [ "$NODE_CURRENT_VERSION" != "$NODE_VERSION" ]; then
    echo Expected $NODE_VERSION!
    
    if [ ! -f $NODE_INSTALL_FILE_LOCAL_PATH ]; then
        echo Downloading $NODE_DIST_URL...
        echo
        curl $NODE_DIST_URL > $NODE_INSTALL_FILE_LOCAL_PATH
        echo
        echo Download complete, saved to $NODE_INSTALL_FILE_LOCAL_PATH
    fi
    
    if [ ! -d $NODE_PACKAGE_PATH ]; then
        echo Extracting $NODE_INSTALL_FILE to $PACKAGES_PATH...
        unzip -q $NODE_INSTALL_FILE_LOCAL_PATH -d $PACKAGES_PATH
        echo
    fi
fi

echo node $($node --version)
echo npm $($npm --version)