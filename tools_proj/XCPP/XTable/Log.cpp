#include "Log.h"
#include <cstdlib>  
#include <ctime>  
#include <sstream>
#include "NativeInterface.h"

std::ofstream Log::m_error_log_file;  
std::ofstream Log::m_info_log_file;  
std::ofstream Log::m_warn_log_file;

CALLBACK callback;

void InitLogger(const std::string& info_log_filename,const std::string& warn_log_filename,const std::string& error_log_filename){  
	Log::m_info_log_file.open(info_log_filename.c_str());  
	Log::m_warn_log_file.open(warn_log_filename.c_str());  
	Log::m_error_log_file.open(error_log_filename.c_str());  
}


Log::~Log(void)
{
	GetStream(m_log_level) << std::endl << std::flush; 
	if (FATAL == m_log_level) 
	{  
       m_info_log_file.close();  
       m_info_log_file.close();  
       m_info_log_file.close();  
       abort();  
    }  
}


std::ostream& Log::GetStream(LogLevel level){  
   return (INFO == level) ?  
                (m_info_log_file.is_open() ? m_info_log_file : std::cout) :  
                (WARN == level ?  
                    (m_warn_log_file.is_open()? m_warn_log_file : std::cerr) :  
                    (m_error_log_file.is_open()? m_error_log_file : std::cerr));  
}  


std::ostream& Log::Start(LogLevel level,std::string text,const int line,const std::string &func) 
{  
   time_t tm;  
   time(&tm);  
   char tmp[64]; 
   strftime(tmp, sizeof(tmp), "%Y-%m-%d %H:%M:%S",localtime(&tm));
   char buff[255];   
   strcpy_s(buff,text.c_str());
   callback(buff);
   std::ostringstream ostr;
   ostr<<tmp<<"\t"<<"function ("<<func<< ")"<< "\tline "<<line<<"\t";
   return GetStream(level)<<ostr.str()<<text;  
}
