#!/usr/bin/env bash
# This script provides functions for interacting with operating systems.

set -o errexit
set -o nounset
set -o pipefail

echo_os() {
    echo "cat /proc/version: $(cat /proc/version)"
    echo "uname -a: $(uname -a)"
}