#!/bin/bash

cd Assets/Wyte/
git config core.sparsecheckout true
if [ ! -d ../../.git/modules/wyte/info ]; then
	mkdir ../../.git/modules/wyte/info
fi

echo "Assets/Wyte" > ../../.git/modules/wyte/info/sparse-checkout
git read-tree -mu HEAD
