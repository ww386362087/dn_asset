#include "XMonster.h"
#include "XAIComponent.h"


XMonster::XMonster()
{
}


XMonster::~XMonster()
{
}


EntityType XMonster::GetType()
{
	return Monster;
}

void  XMonster::OnInitial()
{
	AttachComponent<XAIComponent>();
}