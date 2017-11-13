#include "Logger.h"
#include <cstdlib>  
#include <ctime>  
#include <sstream>

std::ofstream Logger::m_error_log_file;  
std::ofstream Logger::m_info_log_file;  
std::ofstream Logger::m_warn_log_file;

void InitLogger(const std::string& info_log_filename,const std::string& warn_log_filename,const std::string& error_log_filename){  
   Logger::m_info_log_file.open(info_log_filename.c_str());  
   Logger::m_warn_log_file.open(warn_log_filename.c_str());  
   Logger::m_error_log_file.open(error_log_filename.c_str());  
}


Logger::~Logger(void)
{
	GetStream(m_log_rank) << std::endl << std::flush; 
	if (FATAL == m_log_rank) 
	{  
       m_info_log_file.close();  
       m_info_log_file.close();  
       m_info_log_file.close();  
       abort();  
    }  
}

std::ostream& Logger::GetStream(LogLevel level){  
   return (INFO == level) ?  
                (m_info_log_file.is_open() ? m_info_log_file : std::cout) :  
                (WARN == level ?  
                    (m_warn_log_file.is_open()? m_warn_log_file : std::cerr) :  
                    (m_error_log_file.is_open()? m_error_log_file : std::cerr));  
}  


std::ostream& Logger::Start(LogLevel level,std::string text,const int line,const std::string &func) 
{  
   time_t tm;  
   time(&tm);  
   char tmp[64];
   strftime(tmp, sizeof(tmp), "%Y-%m-%d %H:%M:%S",localtime(&tm) );
   std::ostringstream ostr;
   ostr<<tmp<<"\t"<<"function ("<<func<< ")"<< "\tline "<<line<<"\t";
   std::cout<<text<<std::endl;
   return GetStream(level)<<ostr.str()<<text;  
}