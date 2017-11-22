#include "Common.h"

void tobytes(std::string str)
{
	size_t len = str.length();
	for(size_t i=0;i<len;i++)
	{
		printf("0x%x,",(unsigned char)str[i]);  
	}
	printf("\n");
}
 
std::wstring str2wstr(std::string str)
{
    size_t len = str.size();
    wchar_t* b = (wchar_t *)malloc((len+1)*sizeof(wchar_t));
    MBCS2Unicode(b,str.c_str());
    std::wstring r(b);
    free(b);
    return r;
}

wchar_t * MBCS2Unicode(wchar_t* buff, const char* str)
{
    wchar_t * wp = buff;
    char * p = (char *)str;
    while(*p)
	{
        if(*p & 0x80)
		{
            *wp = *(wchar_t *)p;
            p++;
        }
        else
		{
            *wp = (wchar_t)*p;
        }
        wp++;
        p++;
    }
    *wp = 0x0000;
    return buff;
}