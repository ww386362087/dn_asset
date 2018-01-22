#include "XRole.h"
#include "XAIComponent.h"


XRole::XRole()
{
}


XRole::~XRole()
{
}


EntityType XRole::GetType()
{
	return Role;
}


void XRole::OnInitial()
{
	//AttachComponent<XAIComponent>();
}