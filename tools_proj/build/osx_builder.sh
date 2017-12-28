
# !/bin/sh

path=/Users/huailiang.peng/Documents/unity/dn_asset/

cd ${path}

cd tools_proj/build/

echo pwd

sh build_clean.sh

rm -rf out/OSX/GameCore.bundle/

#-D 对应cmakelists.txt 的平台
cmake -DOSX=1 -G "Xcode" . 

echo "Xcode clean"

xcodebuild clean

echo "xcode build"

xcodebuild

echo "build success"

cd out/OSX

ls -al

echo "mv to unity Plugins dir"

rm -drf ${path}/Assets/Plugins/OSX/GameCore.bundle

mv -f GameCore.bundle ${path}/Assets/Plugins/OSX/

echo "done, bye!"
