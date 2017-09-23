#include "EquipSuit.h"


EquipSuit::EquipSuit(void)
{
	filename="EquipSuit";

}


EquipSuit::~EquipSuit(void)
{
}

bool EquipSuit::OnHeaderLine(char **Fields, int FieldCount)
{
	return true;
}

bool EquipSuit::OnCommentLine(char **Fields, int FieldCount)
{
	return true;
}

bool EquipSuit::OnLine(, int FieldCount)
{


	return true;
}
