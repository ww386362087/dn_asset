using UnityEngine;
using System.IO;
using System.Text;

public sealed class GameEnine : XObject
{
    private static MonoBehaviour _entrance;
    private static string _log_path;
    private static string _log_string;

    public static MonoBehaviour entrance { get { return _entrance; } }

    public static void Init(MonoBehaviour en)
    {
        _entrance = en;
        Application.targetFrameRate = 60;
#if Native
        NativeInterface.Init();
#endif
        XTimerMgr.singleton.Init();
        XConfig.Initial(LogLevel.Log, LogLevel.Error);
        XGlobalConfig.Initial();
        XTableMgr.Initial();
        ShaderMgr.Init();
        XResources.Init();
        UIManager.singleton.Initial();
        Documents.singleton.Initial();
        RegistCallbackLog();
    }

    public static void Update(float delta)
    {
        //xtouch must be update first
        XTouch.singleton.Update(delta);

        XTimerMgr.singleton.Update(delta);
        XResources.Update();
        XEntityMgr.singleton.Update(delta);
        XScene.singleton.Update(delta);
        XAutoFade.Update();
        XBulletMgr.singleton.Update(delta);
#if Native
        NativeScene.singleton.Update(delta);
        NativeEntityMgr.singleton.Update(delta);
#endif
    }


    public static void LateUpdate()
    {
        XEntityMgr.singleton.LateUpdate();
        XScene.singleton.LateUpdate();
    }

    public static void OnUnintial()
    {
        UIManager.singleton.UnInitial();
        Documents.singleton.Unintial();
    }

    public static void OnApplicationQuit()
    {
        XDebug.Log("game quit!");
    }

    public static void SetMonoForTest(MonoBehaviour mono)
    {
        _entrance = mono;
    }

    private static void RegistCallbackLog()
    {
        _log_path = Path.Combine(Application.temporaryCachePath, "log.txt");
        _log_string = string.Empty;
        if (File.Exists(_log_path)) File.Delete(_log_path);
        Application.logMessageReceived -= HandleLog;
        Application.logMessageReceived += HandleLog;
    }

    private static void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (LogType.Exception == type || LogType.Assert == type)
        {
            string s = MakeLogString(logString, stackTrace, type);
            try { File.AppendAllText(_log_path, s); }
            catch { }
        }
    }

    private static string MakeLogString(string log, string stack, LogType type)
    {
        if (!log.Equals(_log_string))
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(type);
            sb.Append("]\t");
            sb.Append(System.DateTime.Now.ToString());
            sb.Append("\t");
            sb.Append(log);
            sb.Append("\n");
            sb.Append(stack);
            sb.Append("\n\n");
            return sb.ToString();
        }
        return null;
    }

}

