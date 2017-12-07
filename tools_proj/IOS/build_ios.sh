#
#	i386｜x86_64 是Mac处理器的指令集，i386是针对intel通用微处理器32架构的。x86_64是针对x86架构的64位处理器 这两个是ios模拟器使用
#	standard architectures (including 64-bit)(armv7,arm64) 
#	Build Active Architecture Only  指定是否只对当前连接设备所支持的指令集编译. 当其值设置为YES，这个属性设置为yes，是为了debug的时候编译速度更快，它只编译当前的architecture版本，而设置为no时，会编译所有的版本
#	编译出的版本是向下兼容的，连接的设备的指令集匹配是由高到低（arm64 > armv7s > armv7）依次匹配的
#



# !/bin/sh

path=/Users/huailiang.peng/Documents/unity/dn_asset

cd ${path}

cd tools_proj/IOS/GameCore

echo "xcode clean "

rm -r libGameCore.a

xcodebuild clean 

echo "xcode build"

echo "start build for iphoneos"

#编译 release 版本的.a
xcodebuild -configuration "Release" -target GameCore -sdk iphoneos clean build

eho "start build for simulator"

#编译 release 版本的.a
xcodebuild -configuration "Release" -target GameCore -sdk iphonesimulator clean build

echo "build success"

echo "merge diff start library"

lipo -create build/Release-iphoneos/libGameCore.a build/Release-iphonesimulator/libGameCore.a -output libGameCore.a

lipo -info libGameCore.a

echo "make libGameCore.a success"

echo "start mv to unity Plugins dir"

mv -f libGameCore.a ${path}/Assets/Plugins/iOS/libGameCore.a

echo "done, bye!"