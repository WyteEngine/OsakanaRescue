#!/bin/bash

cd Assets/Wyte/
git config core.sparsecheckout true
echo "Assets/Wyte" > ../../.git/modules/wyte/info/sparse-checkout
git read-tree -mu HEAD
