# Copyright (C) 
# Author: AlexPeng
# Date:	  2017-11-25 
# Function:	make c++ code to so 
#

LOCAL_PATH := $(call my-dir)
include $(CLEAR_VARS)

#so 文件名
LOCAL_MODULE   := GameCore

#  c++目录的相对路径
MY_FILES_PATH  :=  $(LOCAL_PATH)/../../XCPP/GameCore

#$(warning $(MY_FILES_PATH))

# c++后缀
MY_FILES_SUFFIX := %.cpp %.c

# 递归遍历目录下的所有的文件
rwildcard=$(wildcard $1$2) $(foreach d,$(wildcard $1*),$(call rwildcard,$d/,$2))

# 获取相应的源文件
MY_ALL_FILES := $(foreach src_path,$(MY_FILES_PATH), $(call rwildcard,$(src_path),*.*) ) 
MY_ALL_FILES := $(MY_ALL_FILES:$(MY_CPP_PATH)/./%=$(MY_CPP_PATH)%)
MY_SRC_LIST  := $(filter $(MY_FILES_SUFFIX),$(MY_ALL_FILES)) 
MY_SRC_LIST  := $(MY_SRC_LIST:$(LOCAL_PATH)/%=%)

# 去除字串的重复单词
define uniq =
  $(eval seen :=)
  $(foreach _,$1,$(if $(filter $_,${seen}),,$(eval seen += $_)))
  ${seen}
endef

# 递归遍历获取所有目录
MY_ALL_DIRS := $(dir $(foreach src_path,$(MY_FILES_PATH), $(call rwildcard,$(src_path),*/) ) )
MY_ALL_DIRS := $(call uniq,$(MY_ALL_DIRS))

# 赋值给NDK编译系统
LOCAL_SRC_FILES  := $(MY_SRC_LIST)
LOCAL_C_INCLUDES := $(MY_ALL_DIRS)

#在这里设置宏
LOCAL_CFLAGS := -D__ANDROID__

# Add additional include directories
LOCAL_C_INCLUDES += $(LOCAL_PATH)/../../
#LOCAL_C_INCLUDES += $(LOCAL_PATH)/../../../Eigen-3.2.2   
#必须从Android.mk配置文件中拿掉对Eigen的直接包含，放到程序代码中用相对路径包含：
# #include "../../Eigen-3.2.2/Eigen"
# using namespace Eigen;

#$(warning $(LOCAL_SRC_FILES))
#$(warning $(LOCAL_C_INCLUDES))

# use log system in NDK
LOCAL_LDLIBS += -llog

include $(BUILD_SHARED_LIBRARY)
