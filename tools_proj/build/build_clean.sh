# !/bin/sh

path=/Users/huailiang.peng/Documents/unity/dn_asset/tools_proj/build/

cd ${path}

pwd

rm -rf cmake_install.cmake

rm -rf CMakeCache.txt

rm -rf *.xcodeproj

rm -rf CMakeFiles/

rm -rf CMakeScripts/

rm -rf *.build/

echo "clean success"