using System;
using System.IO;
using System.Text;

namespace XForm
{
    /// <summary>
    /// 0.5s保存一次 节省写文件开销
    /// </summary>
    public class XDebug
    {
        static StringBuilder sb = new StringBuilder();

        static DateTime lastSaveTime = DateTime.MinValue;

        static string _path = string.Empty;

        static string _dir = string.Empty;

        enum Type
        {
            LOG,
            WARN,
            ERROR
        };

        static string dir
        {
            get
            {
                if (string.IsNullOrEmpty(_dir))
                {
                    _dir = XCForm.unity_proj_path + @"tools_proj\XForm\Log";
                }
                return _dir;
            }
        }


        static string path
        {
            get
            {
                if (string.IsNullOrEmpty(_path))
                {
                    _path = XCForm.unity_proj_path + @"tools_proj\XForm\Log\log-" + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + ".log";
                }
                return _path;
            }
        }

        public static void Begin()
        {
            CleanOldLog();
            sb.Length = 0;
        }

        public static void Log(string str)
        {
            string time = DateTime.Now.ToString("yyyy -MM-dd HH：mm：ss");
            sb.Append(string.Format("{0} \t[{1}]\t{2}\n\r", time, Type.LOG.ToString(), str));
            Save();
        }


        public static void LogWarning(string str)
        {
            string time = DateTime.Now.ToString("yyyy -MM-dd HH：mm：ss");
            sb.Append(string.Format("{0} \t[{1}]\t{2}\n\r", time, Type.WARN.ToString(), str));
            Save();
        }


        public static void LogError(string str)
        {
            string time = DateTime.Now.ToString("yyyy -MM-dd HH：mm：ss");
            sb.Append(string.Format("{0} \t[{1}]\t{2}\n\r", time, Type.ERROR.ToString(), str));
            Save();
        }


        public static void Save()
        {
            TimeSpan span = DateTime.Now - lastSaveTime;
           // if (span.TotalSeconds > 0.2f)
            {
                if (sb.Length > 0) File.WriteAllText(path, sb.ToString());
                lastSaveTime = DateTime.Now;
            }
        }


        public static void CleanOldLog()
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            DirectoryInfo direct = new DirectoryInfo(dir);
            FileInfo[] files = direct.GetFiles();
            for (int i = 0, max = files.Length; i < max; i++)
            {
                TimeSpan span = DateTime.Now - files[i].LastWriteTime;
                if (span.TotalHours > 1) files[i].Delete();
            }
        }

    }

}

