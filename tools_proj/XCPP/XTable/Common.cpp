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
 