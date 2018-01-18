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

void XLevelWave::ReadFromFile(std::ifstream& infile)
{
	std::string line;
	getline(infile, line);
	if (line != "bw") return;
	while (true)
	{
		getline(infile, line);
		if (line == "ew") break;
		ParseInfo(line);
	}
}

bool XLevelWave::IsScriptWave()
{
	return !levelscript.empty();
}

XLevelWave::~XLevelWave() { }

void XLevelWave::ParseInfo(const std::string& data)
{
	LevelInfoType type = TypeNone;
	if (data.find("id") == 0) type = TypeId;
	else if (data.find("bi") == 0) type = BaseInfo;
	else if (data.find("pw") == 0) type = PreWave;
	else if (data.find("ei") == 0) type = EditorInfo;
	else if (data.find("ti") == 0) type = TransformInfo;
	else if (data.find("si") == 0) type = Script;
	else if (data.find("es") == 0) type = ExString;
	else if (data.find("st") == 0) type = SpawnType;

	int offset = data[data.size() - 1] == '\r' ? 1 : 0;
	std::string rawData = data.substr(3, data.size() - 3 - offset);

	switch (type)
	{
	case TypeId:
		ID = atoi(rawData.c_str());
		break;
	case SpawnType:
		spawnType = (LevelSpawnType)(atoi(rawData.c_str()));
		break;
	case BaseInfo:
	{
		std::vector<std::string> strInfos = SplitNotEscape(rawData, ',');
		time = (int)(convert<float>(strInfos[0].c_str()));
		m_LoopInterval = atoi(strInfos[1].c_str());
		uid = atoi(strInfos[2].c_str());
		m_YRotate = atoi(strInfos[5].c_str());
		if (strInfos.size() > 6)
			radius = (float)atof(strInfos[6].c_str());

		if (strInfos.size() > 7)
			count = atoi(strInfos[7].c_str());

		if (strInfos.size() > 8)
			isAroundPlayer = strcmp(strInfos[8].c_str(), "True") == 0;

		if (strInfos.size() > 11)
			repeat = strcmp(strInfos[11].c_str(), "True") == 0;
	}
	break;
	case ExString:
		exString = rawData;
		break;
	case PreWave:
	{
		std::vector<std::string> strPreWaves = split(rawData, ',');
		for (size_t i = 0; i < strPreWaves.size(); i++)
		{
			int preWave = atoi(strPreWaves[i].c_str());
			m_PreWaves.push_back(preWave);
		}
	}
	break;
	case TransformInfo:
	{
		std::vector<std::string> strFloats = split(rawData, ',');
		int index = atoi(strFloats[0].c_str());
		float x = (float)atof(strFloats[1].c_str());
		float y = (float)atof(strFloats[2].c_str());
		float z = (float)atof(strFloats[3].c_str());
		pos = Vector3(x, y, z);
		rotateY = (float)atof(strFloats[4].c_str());
	}
	break;
	case Script:
		levelscript = rawData;
		break;
	default:
		break;
	}
}
