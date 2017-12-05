#pragma once
#include <assert.h>
#include <string.h>
#include <stdlib.h>
#include <iostream>
#include "Log.h"
#include "patch.h"

typedef unsigned char byte;

typedef struct
{
    hpatch_TStreamOutput base;
    FILE*               m_file;
    hpatch_StreamPos_t  out_pos;
    hpatch_StreamPos_t  out_length;
    hpatch_BOOL         fileError;
    hpatch_BOOL         is_random_out;
} TFileStreamOutput;

typedef struct
{
    hpatch_TStreamInput base;
    FILE*               m_file;
    hpatch_StreamPos_t  m_fpos;
    size_t              m_offset;
    hpatch_BOOL         fileError;
} TFileStreamInput;

#define _check_error(is_error,errorInfo){ \
    if (is_error){  \
        exitCode=1; \
		std::cout<<errorInfo<<std::endl; \
    } \
}

#define _fileError_return { self->fileError=hpatch_TRUE; return -1; }

int readSavedSize(const byte* data,size_t dataSize,hpatch_StreamPos_t* outSize)
{
    size_t lsize;
    if (dataSize<4) return -1;
    lsize=data[0]|(data[1]<<8)|(data[2]<<16);
    if (data[3]!=0xFF)
	{
        lsize|=data[3]<<24;
        *outSize=lsize;
        return 4;
    }
	else
	{
        size_t hsize;
        if (dataSize<9) return -1;
        lsize|=data[4]<<24;
        hsize=data[5]|(data[6]<<8)|(data[7]<<16)|(data[8]<<24);
        *outSize=lsize|(((hpatch_StreamPos_t)hsize)<<32);
        return 9;
    }
}

hpatch_BOOL fileTell64(FILE* file,hpatch_StreamPos_t* out_pos)
{
#ifdef _MSC_VER
    __int64 fpos=_ftelli64(file);
#else
    off_t fpos=ftello(file);
#endif
    hpatch_BOOL result=(fpos>=0);
    if (result) *out_pos=fpos;
    return result;
}

static hpatch_BOOL fileSeek64(FILE* file,hpatch_StreamPos_t seekPos,int whence)
{
#ifdef _MSC_VER
    int ret=_fseeki64(file,seekPos,whence);
#else
    off_t fpos=seekPos;
    if ((fpos<0)||((hpatch_StreamPos_t)fpos!=seekPos)) return hpatch_FALSE;
    int ret=fseeko(file,fpos,whence);
#endif
    return (ret==0);
}

hpatch_BOOL fileRead(FILE* file,byte* buf,byte* buf_end)
{
    static const size_t kBestSize=1<<20;
    while (buf<buf_end) 
	{
        size_t readLen=(size_t)(buf_end-buf);
        if (readLen>kBestSize) readLen=kBestSize;
        if (readLen!=fread(buf,1,readLen,file)) return hpatch_FALSE;
        buf+=readLen;
    }
    return buf==buf_end;
}

long _read_file(hpatch_TStreamInputHandle streamHandle,const hpatch_StreamPos_t readFromPos,byte* out_data,byte* out_data_end)
{
    size_t readLen;
    TFileStreamInput* self=(TFileStreamInput*)streamHandle;
    assert(out_data<=out_data_end);
    readLen=(size_t)(out_data_end-out_data);
    if ((readFromPos+readLen<readFromPos)
        ||(readFromPos+readLen>self->base.streamSize)) _fileError_return;
    if (self->m_fpos!=readFromPos+self->m_offset){
        if (!fileSeek64(self->m_file,readFromPos+self->m_offset,SEEK_SET)) _fileError_return;
    }
    if (!fileRead(self->m_file,out_data,out_data+readLen)) _fileError_return;
    self->m_fpos=readFromPos+self->m_offset+readLen;
    return (long)readLen;
}

long _write_file(hpatch_TStreamInputHandle streamHandle,const hpatch_StreamPos_t writeToPos,const byte* data,const byte* data_end)
{
    unsigned long writeLen,writed;
    TFileStreamOutput* self=(TFileStreamOutput*)streamHandle;
    assert(data<data_end);
    writeLen=(unsigned long)(data_end-data);
    if ((writeToPos+writeLen<writeToPos) ||(writeToPos+writeLen>self->base.streamSize)) _fileError_return;
    if (writeToPos!=self->out_pos)
	{
        if (self->is_random_out)
		{
            if (!fileSeek64(self->m_file,writeToPos,SEEK_SET)) _fileError_return;
            self->out_pos=writeToPos;
        }
		else
		{
            _fileError_return;
        }
    }
    writed=(unsigned long)fwrite(data,1,writeLen,self->m_file);
    if (writed!=writeLen)  _fileError_return;
    self->out_pos=writeToPos+writed;
    self->out_length=(self->out_length>=self->out_pos)?self->out_length:self->out_pos;
    return (long)writed;
}

bool TFileStreamInput_open(TFileStreamInput* self,const char* fileName)
{
	
    assert(self->m_file==0);
    if (self->m_file) return false;
    
    self->m_file=fopen(fileName, "rb");
    if (self->m_file==0) return hpatch_FALSE;
    if (!fileSeek64(self->m_file, 0, SEEK_END)) return hpatch_FALSE;
    if (!fileTell64(self->m_file,&self->base.streamSize)) return hpatch_FALSE;
    if (!fileSeek64(self->m_file, 0, SEEK_SET)) return hpatch_FALSE;
    
    self->base.streamHandle=self;
    self->base.read=_read_file;
    self->m_fpos=0;
    self->m_offset=0;
    self->fileError=hpatch_FALSE;
    return true;
}

static hpatch_BOOL TFileStreamOutput_open(TFileStreamOutput* self,const char* fileName, hpatch_StreamPos_t max_file_length)
{
    assert(self->m_file==0);
    if (self->m_file) return hpatch_FALSE;
    
    self->m_file=fopen(fileName, "wb");
    if (self->m_file==0) return hpatch_FALSE;
    self->base.streamHandle=self;
    self->base.streamSize=max_file_length;
    self->base.write=_write_file;
    self->out_pos=0;
    self->out_length=0;
    self->is_random_out=hpatch_FALSE;
    self->fileError=hpatch_FALSE;
    return hpatch_TRUE;
}

void TFileStreamInput_setOffset(TFileStreamInput* self,size_t offset)
{
    assert(self->m_offset==0);
    assert(self->base.streamSize>=offset);
    self->m_offset=offset;
    self->base.streamSize-=offset;
}

hpatch_BOOL _close_file(FILE** pfile)
{
    FILE* file=*pfile;
    if (file)
	{
        *pfile=0;
        if (fclose(file)!=0) return hpatch_FALSE;
    }
    return hpatch_TRUE;
}

hpatch_BOOL TFileStreamOutput_close(TFileStreamOutput* self)
{
    return _close_file(&self->m_file);
}

hpatch_BOOL TFileStreamInput_close(TFileStreamInput* self)
{
    return _close_file(&self->m_file);
}

//diffFile need create by HDiff
int Dodiff( const char* oldFileName, const char* diffFileName, const char* outNewFileName)
{
    int exitCode=0;
    TFileStreamOutput newData;
    TFileStreamInput diffData;
    TFileStreamInput oldData;

    hpatch_TStreamInput* poldData=&oldData.base;
	memset(&oldData,0,sizeof(TFileStreamInput));
	memset(&diffData,0,sizeof(TFileStreamInput));
	memset(&newData,0,sizeof(TFileStreamOutput));
	
    //open file
    int kNewDataSizeSavedSize=9;
    byte buf[9];
    hpatch_StreamPos_t savedNewSize=0;

    if (!TFileStreamInput_open(&oldData,oldFileName))
		std::cout<<"open oldFile for read error!";
    if (!TFileStreamInput_open(&diffData,diffFileName))
		std::cout<<"open diffFile error!";

    //read savedNewSize
    if (kNewDataSizeSavedSize>diffData.base.streamSize)
        kNewDataSizeSavedSize=(int)diffData.base.streamSize;
    if (kNewDataSizeSavedSize!=diffData.base.read(diffData.base.streamHandle,0, buf,buf+kNewDataSizeSavedSize))
		std::cout<<"read savedNewSize error!";

    kNewDataSizeSavedSize=readSavedSize(buf,kNewDataSizeSavedSize,&savedNewSize);
	if (kNewDataSizeSavedSize<=0) std::cout<<"read savedNewSize error!";
    TFileStreamInput_setOffset(&diffData,kNewDataSizeSavedSize);
  
    if (!TFileStreamOutput_open(&newData, outNewFileName,savedNewSize))
		std::cout<<"open out newFile error!";
 
    if (!patch_stream(&newData.base,poldData,&diffData.base))
	{
        const char* kRunErrInfo="patch_stream() run error!";
        _check_error(oldData.fileError,"oldFile read error!");
        _check_error(diffData.fileError,"diffFile read error!");
        _check_error(newData.fileError,"out newFile write error!");
		std::cout<<kRunErrInfo;
    }

	if (newData.out_length!=newData.base.streamSize) std::cout<<"size not equal";

	_check_error(!TFileStreamOutput_close(&newData),"out newFile close error!");
    _check_error(!TFileStreamInput_close(&diffData),"diffFile close error!");
	_check_error(!TFileStreamInput_close(&oldData),"oldFile close error!");
    return exitCode;
}
