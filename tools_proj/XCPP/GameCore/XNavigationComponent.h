#ifndef  __XNavigationComponent__
#define  __XNavigationComponent__


#include "Vector3.h"

class XNavigationComponent
{
public:
	XNavigationComponent();
	~XNavigationComponent();
	void Navigate(Vector3 dest);
	void NavEnd();
};

#endif