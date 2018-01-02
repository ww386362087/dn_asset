#include "XLevelWave.h"

std::vector<std::string> SplitNotEscape(const std::string &s, const char sep)
{
	int start = 0, end = 0;
	std::vector<std::string> vec;
	std::string str("");
	for (uint i = 0; i < s.length(); ++i)
	{
		if (s.at(i) == (sep))
		{
			end = i;
			if (start == end)
			{
				str.clear();
				vec.push_back(str);
			}
			else if (end > start)
			{
				str = s.substr(start, (end - start));
				vec.push_back(str);
				str.clear();
			}
			++end;
			start = end;
			if (i == (s.length() - 1) && s.at(i) == sep)
			{
				str.clear();
				vec.push_back(str);
			}
		}
		else if (i == (s.length() - 1))
		{
			str = s.substr(start, i - start + 1);
			vec.push_back(str);
		}
	}
	return vec;
}

void XLevelWave::ParseInfo(const std::string& data)
{
	LevelInfoType type = TYPE_NONE;
	if (data.find("id") == 0) type = TYPE_ID;
	else if (data.find("bi") == 0) type = TYPE_BASEINFO;
	else if (data.find("pw") == 0) type = TYPE_PREWAVE;
	else if (data.find("ei") == 0) type = TYPE_EDITOR;
	else if (data.find("mi") == 0) type = TYPE_MONSTERINFO;
	else if (data.find("si") == 0) type = TYPE_SCRIPT;
	else if (data.find("es") == 0) type = TYPE_EXSTRING;
	else if (data.find("st") == 0) type = TYPE_SPAWNTYPE;

	int offset = data[data.size() - 1] == '\r' ? 1 : 0;
	std::string rawData = data.substr(3, data.size() - 3 - offset);
	//rawData[rawData.size()-1] = 0; // È¥µôÐÐÎ²µÄ '\n'

	switch (type)
	{
	case TYPE_NONE:
	{
		break;
	}
	case TYPE_ID:
	{
		m_Id = atoi(rawData.c_str());
	}
	break;
	case TYPE_SPAWNTYPE:
	{
		m_SpawnType = (LevelSpawnType)(atoi(rawData.c_str()));
	}
	break;
	case TYPE_BASEINFO:
	{
		std::vector<std::string> strInfos = SplitNotEscape(rawData, ',');

		m_Time = (int)(convert<float>(strInfos[0].c_str()) * 1000);
		m_LoopInterval = atoi(strInfos[1].c_str());

		m_EnemyID = atoi(strInfos[2].c_str());

		m_YRotate = atoi(strInfos[5].c_str());

		if (strInfos.size() > 6)
			m_RoundRidus = (float)atof(strInfos[6].c_str());

		if (strInfos.size() > 7)
			m_RoundCount = atoi(strInfos[7].c_str());

		if (strInfos.size() > 8)
		{
			m_RandomID = atoi(strInfos[8].c_str());
		}

		if (strInfos.size() > 9)
		{
			std::vector<std::string> ids = split(strInfos[9], "|");
			for (size_t i = 0; i < ids.size(); ++i)
			{
				int doodadID = atoi(ids[i].c_str());
				if (doodadID)
					m_DoodadID.push_back(doodadID);
			}
		}

		if (strInfos.size() > 10)
		{
			int percent = atoi(strInfos[10].c_str());
			m_DoodadPercent = percent / 100.0f;
		}
		if (strInfos.size() > 11)
		{
			m_Repeat = strcmp(strInfos[11].c_str(), "True") == 0 ? 1 : 0;
		}
	}
	break;
	case TYPE_PREWAVE:
	{
		std::vector<std::string> strInfos = split(rawData, '|');

		if (strInfos.size() > 0)
		{
			std::string strPreWave = strInfos[0];

			if (!strPreWave.empty())
			{
				std::vector<std::string> strPreWaves = split(strPreWave, ',');

				for (size_t i = 0; i < strPreWaves.size(); i++)
				{
					int preWave = atoi(strPreWaves[i].c_str());

					m_PreWaves.push_back(preWave);
				}
			}
		}

		if (strInfos.size() > 1)
		{
			int percent = atoi(strInfos[1].c_str());
			m_PreWavePercent = percent / 100.0f;
		}
	}
	break;
	case TYPE_EDITOR:
		break;
	case TYPE_MONSTERINFO:
	{
		std::vector<std::string> strFloats = split(rawData, ',');
		int index = atoi(strFloats[0].c_str());

		// generate game object in scene
		float x = (float)atof(strFloats[1].c_str());
		float y = (float)atof(strFloats[2].c_str());
		float z = (float)atof(strFloats[3].c_str());

		m_Monsters.insert(std::make_pair(index, Vector3(x, y, z)));
		m_MonsterRotation.insert(std::make_pair(index, (float)atof(strFloats[4].c_str())));
	}
	break;
	case TYPE_SCRIPT:
	{
		m_Levelscript = rawData;
	}
	break;
	case TYPE_EXSTRING:
	{
		m_ExString = rawData;
	}
	break;

	default:
		break;
	}
}
