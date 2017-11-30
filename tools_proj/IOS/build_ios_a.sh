# !/bin/sh

path=/Users/huailiang.peng/Documents/unity/dn_asset

cd ${path}

cd tools_proj/IOS/XTable

echo "xcode clean "

xcodebuild clean 

echo "xcode build"

xcodebuild

echo "build success"

cd build/Release-iphoneos

ls -al

echo "start mv to unity Plugins dir"

mv -f libXTable.a ${path}/Assets/Plugins/iOS/libXTable.a

echo "done, bye!"