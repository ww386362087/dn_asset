#pragma once
#include<iostream>
#include<vector>
#include "CSVReader.h";



class EquipSuit : public CSVReader
{

public:
	EquipSuit(void);
	~EquipSuit(void);
	bool OnHeaderLine(char **Fields, int FieldCount);
	bool OnCommentLine(char **Fields, int FieldCount);
	bool OnLine(char **Fields, int FieldCount);


	struct RowData
	{
		int SuitID;
		std::string SuitName;
		int Level;
		int ProfID;
		int SuitQuality;
		bool IsCreateShow;
		std::vector<int> EquipID;
		std::string Effect1;
		std::string Effect2;
		std::string Effect3;
		std:: string Effect4;
		std::string Effect5;
		std:: string Effect6;
		std::string Effect7;
		std::string Effect8;
		std::string Effect9;
		std::string Effect10;
	};


private:
	std::string filename;
};

