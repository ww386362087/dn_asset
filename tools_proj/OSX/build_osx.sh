# !/bin/sh

path=/Users/huailiang.peng/Documents/unity/dn_asset/

cd ${path}

cd tools_proj/OSX/XTable

echo "start clean "

xcodebuild clean 

echo "start build"

xcodebuild 

echo "build success"

cd build/Release

ls -al

echo "start mv to unity Plugins dir"

rm -drf ${path}/Assets/Plugins/XTable.bundle

mv -f XTable.bundle ${path}/Assets/Plugins/XTable.bundle

echo "done, bye!"