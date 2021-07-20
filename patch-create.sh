#!/usr/bin/env bash

usage() 
{
	# Print asset options
	echo "Asset options:"
	grep '|' AssetManifest.list | grep -v '#' | awk '{split($0,a,"|"); print a[2]}' | sed -e 's/^/     /'

	echo
	echo "Usage: patch-asset.sh <asset-name> "
	exit 1
}

revert_asset() 
{
	echo ""
	echo ""
	echo "Reverting asset: $ASSET_NAME"
	echo "At path: $ASSET_PATH"
	echo "Taking backup from: $BACKUP_FULLPATH"

	if ! [ -d "$BACKUP_FULLPATH" ] ; then
		echo "There is no backup for this asset, sorry!"
		exit 1
	fi

	while true ; do
		read -rp "Continue? (y/n) " RESPONSE
		case "$RESPONSE" in
			y|Y|REVERT ) break ;;
			n|N ) exit 0;;
			* ) continue;;
		esac
	done
	do_revert_asset
}

do_revert_asset()
{
	# Restore this asset
	rm -rf "$ASSET_PATH"
	if mv "${BACKUP_FULLPATH}" "$ASSET_PATH" ; then
		echo "Asset reverted."
		exit 0
	else
		echo "ERROR: Asset revert failed!"
		exit 1
	fi
}

yes_no_prompt()
{
	while true ; do
		read -rp "${PROMPT} (y/n) " RESPONSE
		case "$RESPONSE" in
			y|Y ) break ;;
			n|N ) exit 0;;
			* ) continue;;
		esac
	done
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
BACKUP_PATH="backup/${ASSET_NAME}"
BACKUP_FULLPATH="backup/${ASSET_NAME}/${ASSET_PATH}"

if ! [ -d "$ASSET_PATH" ] ; then
        echo "The asset path does not exist: $ASSET_PATH"

	if [ -d "$BACKUP_FULLPATH" ] ; then
		PROMPT="There is a backup available for this asset, would you like to revert it?"
		yes_no_prompt
		do_revert_asset
	else
        	echo "This is a misconfiguration in the AssetManifest.list."
        	exit 1

	fi
fi

echo "Affected directory: $ASSET_PATH"
echo "Backup will be created: ${BACKUP_FULLPATH}"
echo "Even if you haven't changed any of these assets, its possible that Unity"
echo "  decided on its own to upgrade the assets from a previous version, which"
echo "  we still want to patch."
echo
echo "I am going to do the following:"
echo " 1. Remove any existing backups for this asset"
echo " 2. Move the current asset into a backup directory, by doing this the asset"
echo "  will be removed from the project."
echo
echo "If you are rerunning this script because I destroyed your asset and you want"
echo "  to revert, enter REVERT now."

# Ask the user if they want to immediately apply this patch
while true ; do
	read -rp "Are you ready now? (y/n) " RESPONSE
        case "$RESPONSE" in
        	y|Y|REVERT ) break ;;
        	n|N ) exit 0;;
        	* ) continue;;
        esac
done

# If the user wants to revert, then revert
[[ "$RESPONSE" == "REVERT" ]] && revert_asset

# Create a backup of this asset - its possible the patch could go bad and we need to revert
rm -rf "$BACKUP_PATH"
mkdir -p "${BACKUP_PATH}/Assets"
# Move the current asset to the backup directory
mv -v "$ASSET_PATH" "${BACKUP_PATH}/Assets/"
echo ""
echo ""

# Tell the user to reimport the original package
echo "A backup has been created at: ${BACKUP_FULLPATH}"
echo "Steps to continue:"
echo "  1. Reimport the original asset, if its an asset store asset reimport the package."
echo "  2. Enter the word CONTINUE below to start patching."
echo
echo "You can also enter REVERT to undo the changes that we did."

while true; do
	read -rp "[CONTINUE/REVERT]? " RESPONSE
	if [[ "$RESPONSE" == "CONTINUE" ]] ; then
		if [ -d "$ASSET_PATH" ] ; then
			break;
		else 
			echo "It looks like the asset has not been reimported, either try"
			echo "  again or use REVERT to revert what we've done."
		fi
	fi
	[[ "$RESPONSE" == "REVERT" ]] && break 
done

# If the user wants to revert, then revert
[[ "$RESPONSE" == "REVERT" ]] && revert_asset

# If there is an existing patch for this asset, delete it
mkdir -p "patches"
PATCHFILE="patches/${ASSET_NAME}.patch"

# Remove any existing patch
rm -f "${PATCHFILE}"

# Create the patch
diff -Naurw "$ASSET_PATH" "$BACKUP_FULLPATH" > "$PATCHFILE"
echo "Patch file created: $PATCHFILE $(stat --printf="%s" "$PATCHFILE") bytes."

# Ask the user if they want to immediately apply this patch
PROMPT="Do you want to now apply this patch? We recommend yes."
yes_no_prompt

# Apply the patch
patch -p0 < "patches/$ASSET_NAME.patch"
