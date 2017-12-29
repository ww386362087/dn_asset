#if TEST

using AOT;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using GameCore;

public class TestCPP : MonoBehaviour
{
    
#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iInitCallbackCommand(CppDelegate cb);

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iInitial(string stream,string persist);

    
#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern int iAdd(int x, int y);


#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iJson(String file);


#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern int iSub(IntPtr x, IntPtr y);

    
#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iPatch(string oldf,string diff,string newf);

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iStartCore();

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iStopCore();

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iTickCore(float delta);

    //c++的回调指令 最多支持256个指令
    const byte CLog   = 76;//'L'
    const byte CWarn  = 87;//'W'
    const byte CErr = 69;//'E'
    const byte CLoad  = 71;//'G'
    const byte CUnLo =  85;//'U'

    public delegate void CppDelegate(byte type, IntPtr p);
    
    GUILayoutOption[] ui_opt = new GUILayoutOption[] { GUILayout.Width(120), GUILayout.Height(40) };
    GUILayoutOption[] ui_op2 = new GUILayoutOption[] { GUILayout.Width(480), GUILayout.Height(240) };
    GUIStyle ui_sty = new GUIStyle();
    string ui_rst = string.Empty;
    bool m_tick = true;
    float m_tick_time = 0f;
    float spf = 0.033f;//30fps
    public void Start()
    {
        ui_sty.normal.textColor = Color.red;
        ui_sty.fontSize = 20;
        iInitCallbackCommand(new CppDelegate(OnCallback));
        iInitial(Application.streamingAssetsPath + "/", Application.persistentDataPath + "/");
    }

    void Update()
    {
        if (m_tick)
        {
            m_tick_time += Time.deltaTime;
            if (m_tick_time > spf)
            {
                iTickCore(m_tick_time);
                m_tick_time = 0;
            }
        }
    }

    [MonoPInvokeCallback(typeof(CppDelegate))]
    static void OnCallback(byte t, IntPtr ptr)
    {
        string command = Marshal.PtrToStringAnsi(ptr);
        switch (t)
        {
            case CLog: XDebug.TCLog(command); break;
            case CWarn: XDebug.TCWarn(command); break;
            case CErr: XDebug.TCError(command); break;
            case CLoad: XDebug.TCLog("load object: " + command); break;
            case CUnLo: XDebug.TCLog("unload: " + command); break;
            default:
                XDebug.LogError(t + " is not parse symbol: " + command);
                break;
        }
    }

    public void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        TableGUI();
        NativeGUI();
        GUILayout.Space(50);
        GUILayout.TextArea(ui_rst, ui_sty, ui_op2);
        GUILayout.EndHorizontal();
    }
    
    
    private void TableGUI()
    {
        GUILayout.BeginVertical();
        if (GUILayout.Button("Get-Qte-Row", ui_opt))
        {
            int len = CQteStatusList.length;
            ui_rst = "\nqte status list table line cnt: " + len + "\n";
            for (int i = 0; i < 26; i++)
            {
                var rst = CQteStatusList.GetRow(i);
                ui_rst += string.Format("\nvalue:{0,-4} name:{1,-20} comment:{2,-30}", rst.Value, rst.Name, rst.Comment);
            }
        }
        if (GUILayout.Button("Get-Suit-Row", ui_opt))
        {
            int len = CEquipSuit.length;
            ui_rst = "\nequi suit table line cnt: " + len;
            for (int i = 0; i < 26; i++)
            {
                var rst = CEquipSuit.GetRow(i);
                ui_rst += "\nsuitid:" + rst.SuitID + " name:" + rst.SuitName + " level:" + rst.Level + " effect2: " + rst.Effect2[0];
            }
        }
        if (GUILayout.Button("Get-Statics-Row", ui_opt))
        {
            int len = CXEntityStatistics.length;
            ui_rst = "\nstatics table line cnt: " + len;
            for (int i = 0; i < 26; i++)
            {
                var rst = CXEntityStatistics.GetRow(i);
                ui_rst += "\nuid:" + rst.UID + " name:" + rst.Name + " preid:" + rst.PresentID;
            }
        }
        if (GUILayout.Button("Get-Fashion-Row", ui_opt))
        {
            int len = CFashionList.length;
            ui_rst = "\nfashionlist table line cnt: " + len;
            for (int i = 0; i < 12; i++)
            {
                var rst = CFashionList.GetRow(i);
                ui_rst += "\nuid:" + rst.ItemID + " name:" + rst.ItemName + " pos:" + rst.EquipPos;
            }
        }
        if (GUILayout.Button("Get-Fasuit-Row", ui_opt))
        {
            int len = CFashionSuit.length;
            ui_rst = "\ntable line cnt: " + len;
            for (int i = 0; i < 12; i++)
            {
                var rst = CFashionSuit.GetRow(i);
                ui_rst += "\nuid:" + rst.SuitID + " name:" + rst.SuitName + " preid:" + rst.SuitIcon;
            }
        }
        if (GUILayout.Button("Get-Defaut-Row", ui_opt))
        {
            int len = CDefaultEquip.length;
            ui_rst = "\ntable line cnt: " + len;
            for (int i = 0; i < 12; i++)
            {
                var rst = CDefaultEquip.GetRow(i);
                ui_rst += "\nuid:" + rst.ProfID + " hair:" + rst.Hair + " face:" + rst.Face;
            }
        }
        if (GUILayout.Button("Get-NPCList-Row", ui_opt))
        {
            int len = CXNpcList.length;
            ui_rst = "\ntable line cnt: " + len;
            for (int i = 0; i < 12; i++)
            {
                var rst = CXNpcList.GetRow(i);
                ui_rst += "\nuid:" + rst.NPCID + " name:" + rst.NPCName + " type:" + rst.NPCType;
            }
        }
        if (GUILayout.Button("Get-Scene-Row", ui_opt))
        {
            int len = CSceneList.length;
            ui_rst = "\ntable line cnt: " + len;
            for (int i = 0; i < 12; i++)
            {
                var rst = CSceneList.GetRow(i);
                ui_rst += "\nuid:" + rst.SceneID + " path:" + rst.ScenePath;
            }
        }
        if (GUILayout.Button("Get-Present-Row", ui_opt))
        {
            int len = CXEntityPresentation.length;
            ui_rst = "\npresent table line cnt: " + len;
            for (int i = 0; i < 20; i++)
            {
                var rst = CXEntityPresentation.GetRow(i);
                ui_rst += "\nuid:" + rst.UID + " name:" + rst.Name + " pref: " + rst.Prefab + " ani: " + rst.AnimLocation;
                for (int j = 0, max = rst.Hit_f.Length; j < max; j++)
                {
                    Debug.Log(rst.Hit_f[j]);
                }
            }
        }
        GUILayout.EndVertical();
    }


    private void NativeGUI()
    {
        GUILayout.BeginVertical();
        if (GUILayout.Button("Cal-Add", ui_opt))
        {
            int i = iAdd(8, 7);
            ui_rst = "8+7=" + i;
        }
        if (GUILayout.Button("Cal-Sub", ui_opt))
        {
            int a = 8, b = 2;
            IntPtr p1 = Marshal.AllocCoTaskMem(Marshal.SizeOf(a));
            Marshal.StructureToPtr(a, p1, false);
            IntPtr p2 = Marshal.AllocCoTaskMem(Marshal.SizeOf(b));
            Marshal.StructureToPtr(b, p2, false);
            int rst = iSub(p1, p2);
            ui_rst = a + "-" + b + "=" + rst;
        }
        if (GUILayout.Button("Native-Json", ui_opt))
        {
            iJson(Application.streamingAssetsPath + "/Patch/json.txt");
            ui_rst = "native parse json finish!";
            XDebug.Log(ui_rst);
        }
        if (GUILayout.Button("Native-Patch", ui_opt))
        {
            string old = Application.streamingAssetsPath + "/Patch/old.txt";
            string diff = Application.streamingAssetsPath + "/Patch/diff.patch";
            string newf = Application.streamingAssetsPath + "/Patch/new.txt";
            XDebug.Log(old + " " + diff + " " + newf);
            iPatch(old, diff, newf);
            XDebug.Log("patch finish");
        }
        if (GUILayout.Button("Native-Start-Game", ui_opt))
        {
            m_tick = true;
            ui_rst = "start gamecore ";
            iStartCore();
        }
        if (GUILayout.Button("Native-Stop-Game", ui_opt))
        {
            m_tick = false;
            ui_rst = "stop gamecore";
            iStopCore();
        }
        if (GUILayout.Button("Native-Tick-Game", ui_opt))
        {
            m_tick = false;
            ui_rst = "tick gamecore";
            iTickCore(Time.deltaTime);
        }
        GUILayout.EndVertical();
    }
    
}

#endif