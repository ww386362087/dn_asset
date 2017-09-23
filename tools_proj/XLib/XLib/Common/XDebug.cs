using System.Text;
using UnityEngine;

/// <summary>
/// log输出
/// 1. log控制开关
/// 2. 字符串拼接 没有额外开销
/// </summary>
public class XDebug
{
    static StringBuilder sb = new StringBuilder();

    enum LogLevel
    {
        None, // 不输出log
        Log,
        Green,
        Warn,
        Error
    }

    static readonly LogLevel loglevel = LogLevel.Error;

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
        if (loglevel >= LogLevel.Log)
            Debug.Log(Append(param1, param2, param3, param4));
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
        if (loglevel >= LogLevel.Green)
            Debug.Log(GreenAppend(param1, param2, param3, param4));
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
        if (loglevel >= LogLevel.Warn)
            Debug.LogWarning(Append(param1, param2, param3, param4));
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
        if (loglevel >= LogLevel.Error)
            Debug.LogError(Append(param1, param2, param3, param4));
    }

    private static string Append(object param1, object param2, object param3, object param4)
    {
        sb.Length = 0;
        sb.Append(param1);
        if (param2 != null)
            sb.Append(param2);
        if (param3 != null)
            sb.Append(param3);
        if (param4 != null)
            sb.Append(param4);
        return sb.ToString();
    }


    private static string GreenAppend(object param1, object param2, object param3, object param4)
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
        return sb.ToString();
    }

}

