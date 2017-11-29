#include "NativeInterface.h"
#include <fstream>
#include "picotest.h"

extern "C"
{

	int iAdd(int a, int b)
	{
		return a+b;
	}

	int iSub(int* a, int* b)
	{
		return *a - *b;
	}

	void iInitial(const char* stream,const char* cache)
	{
		std::string s = cache;
		InitPath(stream,cache);
		LOG(UNITY_STREAM_PATH);
		InitLogger(s+"Log/info.txt",s+"Log/warn.txt",s+"Log/error.txt");
		LOG("c++ initial success with path: "+ s);
	}

	void iInitCallbackCommand(CALLBACK cb)
	{
		callback = cb;   
	}

	void iJson()
	{
		LOG("READ JSON START");
		std::ifstream json_file;  
		json_file.open("json.txt");  
		std::string json;  
		if (!json_file.is_open())  
		{  
			ERROR("Error opening file");
			exit(1);  
		}  
		std::string s;
		while(getline(json_file,s))
		{
			json+=s;
		}
		json_file.close();
		
		/*picojson π”√≤Œ’’: 
		 *	http://www.sokoide.com/wp/2015/07/26/header-only-cpp-json-library-picojson/
		 *	https://github.com/kazuho/picojson
		 */
		picojson::value v;
		std::string err = picojson::parse(v, json);
	    if(err.size())
		{
			ERROR(err);
			return;
		}
		
		picojson::object& o = v.get<picojson::object>();
		std::string sub = o["data_subtype"].get<std::string>();
		std::string task = o["task_type"].get<std::string>();

		picojson::array ar = o["questions"].get<picojson::array>();
		for (picojson::array::iterator it = ar.begin(); it != ar.end(); it++)
		{
			picojson::object& item = it->get<picojson::object>();
			double imageid = item["image_id"].get<double>();
			double questid = item["question_id"].get<double>();

			double rt = imageid-questid;
			std::string que = item["question"].get<std::string>();
			LOG("question:"+que);
			LOG("imageid: "+tostring(imageid));
			LOG("questid: "+tostring(questid));
			LOG("sub rt£∫"+tostring(rt));
		 }
	}

}