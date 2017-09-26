using System.Text;
using UnityEngine;
using System.IO;

public enum LogLevel
{
    Log,
    Green,
    Warn,
    Error,
    None, // 不输出log
}

/// <summary>
/// log输出
/// 1. log控制开关
/// 2. 字符串拼接 没有额外开销
/// 3. 输出本地文件 outlevel
/// </summary>
public class XDebug
{
    static StringBuilder sb = new StringBuilder();

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
        if (loglevel >= LogLevel.Log)
            Log(param, null);
    }

    public static void Log(object param1, object param2)
    {
        if (loglevel >= LogLevel.Log)
            Log(param1, param2, null);
    }

    public static void Log(object param1, object param2, object param3)
    {
        if (loglevel >= LogLevel.Log)
            Log(param1, param2, param3, null);
    }

    public static void Log(object param1, object param2, object param3, object param4)
    {
        Append(param1, param2, param3, param4);
        if (loglevel >= LogLevel.Log)
            Debug.Log(sb);
        if (outlevel >= LogLevel.Log)
            Write(sb);
    }

    public static void LogGreen(object param)
    {
        if (loglevel >= LogLevel.Green)
            LogGreen(param, null);
    }

    public static void LogGreen(object param1, object param2)
    {
        if (loglevel >= LogLevel.Green)
            LogGreen(param1, param2, null);
    }

    public static void LogGreen(object param1, object param2, object param3)
    {
        if (loglevel >= LogLevel.Green)
            LogGreen(param1, param2, param3, null);
    }

    public static void LogGreen(object param1, object param2, object param3, object param4)
    {
        GreenAppend(param1, param2, param3, param4);
        if (loglevel >= LogLevel.Green)
            Debug.Log(sb);
        if (outlevel >= LogLevel.Green)
            Write(sb);
    }

    public static void LogWarning(object param)
    {
        if (loglevel >= LogLevel.Warn)
            LogWarning(param, null);
    }
    public static void LogWarning(object param1, object param2)
    {
        if (loglevel >= LogLevel.Warn)
            LogWarning(param1, param2, null);
    }

    public static void LogWarning(object param1, object param2, object param3)
    {
        if (loglevel >= LogLevel.Warn)
            LogWarning(param1, param2, param3, null);
    }

    public static void LogWarning(object param1, object param2, object param3, object param4)
    {
        Append(param1, param2, param3, param4);
        if (loglevel >= LogLevel.Warn)
            Debug.LogWarning(sb);
        if (outlevel >= LogLevel.Warn)
            Write(sb);
    }

    public static void LogError(object param)
    {
        if (loglevel >= LogLevel.Error)
            LogError(param, null);
    }

    public static void LogError(object param1, object param2)
    {
        if (loglevel >= LogLevel.Error)
            LogError(param1, param2, null);
    }

    public static void LogError(object param1, object param2, object param3)
    {
        if (loglevel >= LogLevel.Error)
            LogError(param1, param2, param3, null);
    }

    public static void LogError(object param1, object param2, object param3, object param4)
    {
        Append(param1, param2, param3, param4);
        if (loglevel >= LogLevel.Error)
            Debug.LogError(sb);
        if (outlevel >= LogLevel.Error)
            Write(sb);
    }

    private static void Append(object param1, object param2, object param3, object param4)
    {
        sb.Length = 0;
        sb.Append(param1);
        if (param2 != null)
            sb.Append(param2);
        if (param3 != null)
            sb.Append(param3);
        if (param4 != null)
            sb.Append(param4);
    }


    private static void GreenAppend(object param1, object param2, object param3, object param4)
    {
        sb.Length = 0;
        sb.Append("<color=green>");
        sb.Append(param1);
        if (param2 != null)
            sb.Append(param2);
        if (param3 != null)
            sb.Append(param3);
        if (param4 != null)
            sb.Append(param4);
        sb.Append("</color>");
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
        if(File.Exists(log_file_path))
        {
            File.Delete(log_file_path);
        }
    }

}

