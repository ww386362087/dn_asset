using System.Text;
using UnityEngine;
using System.IO;

public enum LogLevel
{
    Log,
    Green,
    Cpp,
    Warn,
    Error,
    None, // 不输出log
}

/// <summary>
/// log输出
/// 1. log控制开关
/// 2. 字符串拼接 没有额外开销
/// 3. 输出本地文件 outlevel
/// 4. 实现双击log跳转到对应的逻辑代码 而不是debug
/// </summary>
public class XDebug
{
    static StringBuilder shareSB = new StringBuilder();

    internal static LogLevel loglevel = LogLevel.Error;

    internal static LogLevel outlevel = LogLevel.None;

    internal static string log_file_path;

    internal static void Init(LogLevel log, LogLevel tofile)
    {
        loglevel = log;
        outlevel = tofile;
        log_file_path = XConfig.cache_path + @"/log.txt";
        CleanLogFile();
    }

    public static void Log(object param)
    {
        if (loglevel <= LogLevel.Log)
            Log(param, null);
    }

    public static void Log(object param1, object param2)
    {
        Log(param1, param2, null);
    }

    public static void Log(object param1, object param2, object param3)
    {
        Log(param1, param2, param3, null);
    }

    public static void Log(object param1, object param2, object param3, object param4)
    {
        Log(param1, param2, param3, param4, null);
    }

    public static void Log(object param1, object param2, object param3, object param4, object param5)
    {
        if (loglevel <= LogLevel.Log)
        {
            Append(param1, param2, param3, param4, param5);
            Debug.Log(shareSB);
            Write(shareSB);
        }
    }

    public static void LogGreen(object param)
    {
        LogGreen(param, null);
    }

    public static void LogGreen(object param1, object param2)
    {
        LogGreen(param1, param2, null);
    }

    public static void LogGreen(object param1, object param2, object param3)
    {
        LogGreen(param1, param2, param3, null);
    }

    public static void LogGreen(object param1, object param2, object param3, object param4)
    {
        LogGreen(param1, param2, param3, param4, null);
    }

    public static void LogGreen(object param1, object param2, object param3, object param4, object param5)
    {
        if (loglevel <= LogLevel.Green)
        {
            GreenAppend(param1, param2, param3, param4, param5);
            Debug.Log(shareSB);
            Write(shareSB);
        }
    }
    
    // 专门给C/C++调用的Log
    public static void TCLog(string param)
    {
        if (loglevel <= LogLevel.Cpp)
        {
            shareSB.Length = 0;
            shareSB.Append("++> ");
            shareSB.Append(param);
            Debug.Log(shareSB);
            Write(shareSB);
        }
    }

    // 专门给C/C++调用的Log
    public static void TCWarn(string param)
    {
        if (loglevel <= LogLevel.Cpp)
        {
            shareSB.Length = 0;
            shareSB.Append("++> ");
            shareSB.Append(param);
            Debug.LogWarning(shareSB);
            Write(shareSB);
        }
    }

    public static void TCError(string param)
    {
        if (loglevel <= LogLevel.Cpp)
        {
            shareSB.Length = 0;
            shareSB.Append("++> ");
            shareSB.Append(param);
            Debug.LogError(shareSB);
            Write(shareSB);
        }
    }

    public static void LogWarning(object param)
    {
        LogWarning(param, null);
    }
    public static void LogWarning(object param1, object param2)
    {
        LogWarning(param1, param2, null);
    }

    public static void LogWarning(object param1, object param2, object param3)
    {
        LogWarning(param1, param2, param3, null);
    }

    public static void LogWarning(object param1, object param2, object param3, object param4)
    {
        LogWarning(param1, param2, param3, param4, null);
    }

    public static void LogWarning(object param1, object param2, object param3, object param4, object param5)
    {
        if (loglevel <= LogLevel.Warn)
        {
            Append(param1, param2, param3, param4, param5);
            Debug.LogWarning(shareSB);
            Write(shareSB);
        }
    }
    
    public static void LogError(object param)
    {
        LogError(param, null);
    }

    public static void LogError(object param1, object param2)
    {
        LogError(param1, param2, null);
    }

    public static void LogError(object param1, object param2, object param3)
    {
        LogError(param1, param2, param3, null);
    }

    public static void LogError(object param1, object param2, object param3, object param4)
    {
        LogError(param1, param2, param3, param4, null);
    }

    public static void LogError(object param1, object param2, object param3, object param4, object param5)
    {
        if (loglevel <= LogLevel.Error)
        {
            Append(param1, param2, param3, param4, param5);
            Debug.LogError(shareSB);
            Write(shareSB);
        }
    }
    
    private static void Append(object param1, object param2, object param3, object param4, object param5)
    {
        shareSB.Length = 0;
        shareSB.Append(param1);
        if (param2 != null)
            shareSB.Append(param2);
        if (param3 != null)
            shareSB.Append(param3);
        if (param4 != null)
            shareSB.Append(param4);
        if (param5 != null)
            shareSB.Append(param5);
    }


    private static void GreenAppend(object param1, object param2, object param3, object param4, object param5)
    {
        shareSB.Length = 0;
        shareSB.Append("<color=green>");
        shareSB.Append(param1);
        if (param2 != null)
            shareSB.Append(param2);
        if (param3 != null)
            shareSB.Append(param3);
        if (param4 != null)
            shareSB.Append(param4);
        if (param5 != null)
            shareSB.Append(param5);
        shareSB.Append("</color>");
    }

    /// <summary>
    /// 如果由 path 指定的文件不存在，则创建该文件。如果该文件存在，则对 StreamWriter 的写入操作将文本追加到该文件。
    /// 允许其他线程在文件打开后读取该文件。
    /// </summary>
    private static void Write(StringBuilder sb)
    {
        if (!string.IsNullOrEmpty(log_file_path))
        {
            using (StreamWriter writer = File.AppendText(log_file_path))
            {
                writer.WriteLine(sb.ToString());
            }
        }
    }


    private static void CleanLogFile()
    {
        if (File.Exists(log_file_path))
        {
            File.Delete(log_file_path);
        }
    }

}

