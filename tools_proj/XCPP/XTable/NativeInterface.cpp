#include "NativeInterface.h"
#include <fstream>
//#include "rapidjson\document.h"
//#include "rapidjson\prettywriter.h"
//#include "rapidjson\filereadstream.h"  
//#include "rapidjson\filewritestream.h"  
//#include "rapidjson\stringbuffer.h"  


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
		std::ifstream json_file;  
		json_file.open("json.txt");  
		std::string json;  
		if (!json_file.is_open())  
		{  
			std::cout << "Error opening file" << std::endl;  
			exit(1);  
		}  
		std::string s;
		while(getline(json_file,s))
		{
			json+=s;
		}
		json_file.close();

	/*	rapidjson::Document doc;  
		doc.Parse<0>(json.c_str());  
		std::string task_type = doc["task_type"].GetString();
		std::string data_type = doc["data_type"].GetString();
		LOG("task_type:"+task_type);
		LOG("data_type:"+data_type);

		rapidjson::Value & ques = doc["questions"];  
		std::string temp_ques;  
		int temp_ima_id, temp_ques_id;  
		if (ques.IsArray())  
		{  
			for (size_t i = 0; i < ques.Size(); ++i)  
			{  
				rapidjson::Value& v = ques[i];  
				assert(v.IsObject());  
				if (v.HasMember("question") && v["question"].IsString()) {  
					temp_ques = v["question"].GetString();  
				}  
				if (v.HasMember("image_id") && v["image_id"].IsInt()) {  
					temp_ima_id = v["image_id"].GetInt();  
				}  
				if (v.HasMember("question_id") && v["question_id"].IsInt()) {  
					temp_ques_id = v["question_id"].GetInt();  
				}
				LOG("temp_ques:"+temp_ques);
				LOG("image_id:"+tostring(temp_ima_id));
				LOG("question_id:"+tostring(temp_ques_id));
			}  
		}*/ 
	}

}