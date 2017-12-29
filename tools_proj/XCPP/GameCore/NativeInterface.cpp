#include "NativeInterface.h"
#include "GameMain.h"

class XEntity;

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

	void iInitial(const char* stream, const char* cache)
	{
		std::string s = cache;
		LOG(stream);
		InitPath(stream, cache);
		LOG(s);
		InitLogger(s + "Log/info.txt", s + "Log/warn.txt", s + "Log/error.txt");
		PRINT << stream << " " << cache;
		LOG("c++ initial success with path: " + s);
	}

	void iInitCallbackCommand(SharpCALLBACK cb)
	{
		callback = cb;
	}

	void  iInitEntityCall(EntyCallBack cb)
	{
		eCallback = cb;
	}

	void iInitCompnentCall(CompCallBack cb)
	{
		cCallback = cb;
	}

	void iPatch(const char* old_file, const char* diff_file, const char* new_file)
	{
		LOG("old:" + tostring(old_file) + " diff: " + tostring(diff_file) + " new_file: " + tostring(new_file));
		//Dodiff(old_file,diff_file,new_file);
	}

	void iJson(const char* file)
	{
		LOG("READ JSON START");
		std::string json = readFile(file);


		/*picojsonÊ¹ÓÃ²ÎÕÕ:
		 *	http://www.sokoide.com/wp/2015/07/26/header-only-cpp-json-library-picojson/
		 *	https://github.com/kazuho/picojson
		 */
		picojson::value v;
		std::string err = picojson::parse(v, json);
		if (err.size())
		{
			ERR(err);
			return;
		}

		picojson::object& o = v.get<picojson::object>();
		std::string sub = o["data_subtype"].get<std::string>();
		std::string task = o["task_type"].get<std::string>();
		object vv = o["task_type"];
		std::string rst = vv.get<std::string>();
		LOG("subtype: " + sub + " sub2:" + rst);

		picojson::array ar = o["questions"].get<picojson::array>();
		for (picojson::array::iterator it = ar.begin(); it != ar.end(); it++)
		{
			picojson::object& item = it->get<picojson::object>();
			double imageid = item["image_id"].get<double>();
			double questid = item["question_id"].get<double>();

			double rt = imageid - questid;
			std::string que = item["question"].get<std::string>();
			LOG("question:" + que);
			LOG("imageid: " + tostring(imageid));
			LOG("questid: " + tostring(questid));
			LOG("sub" + tostring(rt));
		}
	}

	void iVector()
	{
		Vector3* v3 = new Vector3(2.0f, 2.0f, 4.0f);
		LOG(tostring(*v3));
		Vector3* v4 = new Vector3(1.0f, 1.2f, 2.0f);
		*v3 = Vector3::one + *v4;
		LOG(tostring(*v3));
		Vector3 zv = Vector3::zero;
		LOG(tostring(zv));
	}


	void iStartCore()
	{
		LOG("iStartCore");
		GameMain::Instance()->Start();
	}

	void iStopCore()
	{
		GameMain::Instance()->Stop();
	}

	void iTickCore(float delta)
	{
		GameMain::Instance()->Ontick(delta);
	}
}
