# !/bin/sh

path=/Users/huailiang.peng/Documents/unity/dn_asset/tools_proj/

cd ${path}

echo "start build android "

cd Android

sh build_android.sh

echo "start build osx "

cd ../OSX

sh build_osx.sh

echo "start build IOS"

cd ../IOS

sh build_ios.sh


cd ../

echo "build done, bye!"