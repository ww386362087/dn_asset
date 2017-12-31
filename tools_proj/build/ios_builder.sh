# !/bin/sh

path=/Users/huailiang.peng/Documents/unity/dn_asset/tools_proj/build/

cd ${path}

echo pwd

sh build_clean.sh

echo "clean ..."

#generate ios project
cmake -DCMAKE_TOOLCHAIN_FILE=toolchains/ios.toolchain.cmake -DIOS_PLATFORM=iPhoneOS -DCMAKE_OSX_ARCHITECTURES='armv7 armv7s arm64' -GXcode 
	
echo "xcode clean "

rm -r out/iOS/libGameCore.a

xcodebuild clean 

echo "xcode build"

echo "start build for iphoneos"

#编译 release 版本的.a 
xcodebuild -configuration "Release" -target GameCore -sdk iphoneos clean build

echo "build success"

echo "merge diff start library"

lipo -info out/iOS/libGameCore.a

echo "make libGameCore.a success"