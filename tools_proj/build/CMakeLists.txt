cmake_minimum_required(VERSION 3.0.2)
#project(GameCore)

include_directories(${CMAKE_CURRENT_SOURCE_DIR})

file(GLOB_RECURSE source_cpp_files "${CMAKE_CURRENT_SOURCE_DIR}/../XCPP/GameCore/*.cpp")
file(GLOB_RECURSE source_c_files "${CMAKE_CURRENT_SOURCE_DIR}/../XCPP/GameCore/*.c")

# 设置使用 C++11 标准
set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)


# 编译 Debug 模式还是 Release 模式
set(CMAKE_BUILD_TYPE Release)

# 根据 OSX、Android、iOS 等不同平台，分别设置不同的配置
if (OSX)
    message("is building osx platform")

    include(CheckCXXCompilerFlag)  
    CHECK_CXX_COMPILER_FLAG("-std=c++11" COMPILER_SUPPORTS_CXX11)  
    CHECK_CXX_COMPILER_FLAG("-std=c++0x" COMPILER_SUPPORTS_CXX0X)  
    if(COMPILER_SUPPORTS_CXX11)  
            set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")  
    elseif(COMPILER_SUPPORTS_CXX0X)  
            set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++0x")  
    else()  
         message(STATUS "The compiler ${CMAKE_CXX_COMPILER} has no C++11 support. Please use a different C++ compiler.")  
    endif()  

    add_library(GameCore MODULE ${source_cpp_files} ${source_c_files})

    # OSX 下面的 Unity 插件要是 GameCore.bundle 格式的
    set_target_properties(GameCore PROPERTIES BUNDLE TRUE)

    # 设置代码库的输出路径
    set(OUT_PATH ${CMAKE_CURRENT_SOURCE_DIR}/out/OSX/)

    # 添加编译使用的宏
    ADD_DEFINITIONS(-D__MACH__)

elseif(IOS)
    message("is building ios platform")

	add_library(GameCore STATIC ${source_cpp_files} ${source_c_files})

    # 设置编译器, 由于 iOS 和 OSX 使用的是相同的编译器，所以此处设置为 Mac 上的Clang 的路径
    # 但是对于 Android 来讲，交叉编译的环境为 NDK 的编译器环境，所以需要对 Android 设置为 NDK 下面的编译器路径。
    set(CMAKE_C_COMPILER clang)
    set(CMAKE_CXX_COMPILER clang++)
    # bitcode 设置为 NO
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -fembed-bitcode")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -fembed-bitcode")

    # 最低支持的 iOS 版本
    set(CMAKE_XCODE_ATTRIBUTE_IPHONEOS_DEPLOYMENT_TARGET "7.1")

    # 支持的指令架构
    set(CMAKE_OSX_ARCHITECTURES "armv7 armv7s arm64")

    # 使用哪个 iOS SDK 进行打包，这里需要注意是对 iPhone 真机打包还是 iPhone 模拟器打包，二者的 SDK 是不同的。
    # 我们这里使用的是 iPhone 真机的 SDK。
    set(CMAKE_OSX_SYSROOT /Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS.sdk)

    # 设置代码库的输出路径
    set(OUT_PATH ${CMAKE_CURRENT_SOURCE_DIR}/out/iOS/)
    
    # 添加编译使用的宏
    ADD_DEFINITIONS(-D__iOS__)

elseif(ANDROID)
    message("is building android platform")

    add_library(GameCore SHARED ${source_cpp_files} ${source_c_files})

    # 由于 Unity 在 Android 下面对 C++ 只支持 .so 的动态库格式，
    # 而如果你用 Mac 机编译时，默认的动态库格式为 .dylib,
    # 所以我们需要显示的使用 .so 后缀。
    set_target_properties(GameCore PROPERTIES SUFFIX ".so")

    # 设置代码库的输出路径
    set(OUT_PATH ${CMAKE_CURRENT_SOURCE_DIR}/out/Android/${ANDROID_ABI}/)
    
    # 添加编译使用的宏
    ADD_DEFINITIONS(-D__ANDROID__)

else()
    message("warn: unknown platform")
    message(${CMAKE_SYSTEM})
    add_library(GameCore STATIC ${source_cpp_files} ${source_c_files})
    set(OUT_PATH ${CMAKE_CURRENT_SOURCE_DIR}/out/else/)
endif()


# 设置代码库的输出路径, 对 Debug 和 Release 模式都设置成同一个路径
set_target_properties(GameCore PROPERTIES
        LIBRARY_OUTPUT_DIRECTORY ${OUT_PATH}
        ARCHIVE_OUTPUT_DIRECTORY ${OUT_PATH}
        LIBRARY_OUTPUT_DIRECTORY_RELEASE ${OUT_PATH}
        ARCHIVE_OUTPUT_DIRECTORY_RELEASE ${OUT_PATH}
        LIBRARY_OUTPUT_DIRECTORY_DEBUG ${OUT_PATH}
        ARCHIVE_OUTPUT_DIRECTORY_DEBUG ${OUT_PATH}
        LIBRARY_OUTPUT_DIRECTORY_RELEASE ${OUT_PATH}
        ARCHIVE_OUTPUT_DIRECTORY_RELEASE ${OUT_PATH}
        )

