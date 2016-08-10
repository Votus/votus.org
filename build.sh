#!/usr/bin/env bash

echo Starting!

set -o errexit
set -o nounset
set -o pipefail

PACKAGES_FOLDER='packages'
PACKAGES_PATH="$(readlink -f $PACKAGES_FOLDER)"

echo
echo Bash shell configured!
echo
NODE_VERSION='v6.3.1'
echo Provisioning Node.js $NODE_VERSION...
NODE_PACKAGE_NAME="node-$NODE_VERSION-win-x64"
NODE_PACKAGE_PATH="$PACKAGES_PATH/$NODE_PACKAGE_NAME"
node=$NODE_PACKAGE_PATH/node.exe
npm=$NODE_PACKAGE_PATH/npm
echo Paths configured!
echo
echo Provisioning Node.js modules...
$npm install
echo Done!