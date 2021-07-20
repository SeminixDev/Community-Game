#!/usr/bin/env bash

usage() {
	echo "Usage: patch-asset.sh <asset-name> "
	exit 1
}

[[ $# != 1 ]] && usage

# Figure out where this asset lives
REGEX="*\\|[[:space:]]*${1}[[:space:]]*\\|*"
DEFINITIONS_COUNT=$(grep -c "${REGEX}" < AssetManifest.list)
DEFINITION=$(grep "${REGEX}" < AssetManifest.list)

if [[ $DEFINITIONS_COUNT == 0 ]] ; then
        echo "No asset definition found for asset: $1"
        exit 1
fi

if [[ $DEFINITIONS_COUNT != 1 ]] ; then
        echo "More than one asset matches this definition: $1"
        exit 1
fi

ASSET_NAME="$(echo "$1" | sed 's/ *$//g')"
ASSET_PATH="Assets/$(echo "$DEFINITION" | awk '{split($0,a,"|"); print a[1]}')"
ASSET_PATH="$(echo "$ASSET_PATH" | sed 's/ *$//g')"
echo "Asset path: $ASSET_PATH"

if ! [ -d "$ASSET_PATH" ] ; then
	echo "The asset path does not exist: $ASSET_PATH"
	echo "This is a misconfiguration in the AssetManifest.list."
	exit 1
fi

# Create a backup of this asset - its possible the patch could go bad and we need to revert
mkdir -p "backup/$ASSET_NAME"
rm -f "backup/$ASSET_NAME.tar.gz"
tar czf "backup/$ASSET_NAME.tar.gz" "$ASSET_PATH"

# Tell the user to reimport the original package
echo "Steps to continue:"
echo "  1. Delete the asset folder"
echo "  2. Reimport the original asset, if its an asset store asset reimport the package."
echo "  3. Enter the word CONTINUE below."

while true; do
	read -rp "[CONTINUE]? " RESPONSE
	[[ "$RESPONSE" == "CONTINUE" ]] && break 
done

# Apply the patch
patch -p0 < "patches/${ASSET_NAME}.patch"
