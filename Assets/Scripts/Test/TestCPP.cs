#if TEST

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using GameCore;
using System.Text;

public class TestCPP : MonoBehaviour
{
    GUILayoutOption[] ui_opt = new GUILayoutOption[] { GUILayout.Width(120), GUILayout.Height(40) };
    GUILayoutOption[] ui_op2 = new GUILayoutOption[] { GUILayout.Width(480), GUILayout.Height(240) };
    GUIStyle ui_sty = new GUIStyle();
    string ui_rst = string.Empty;
    StringBuilder ui_sb = new StringBuilder();
    bool m_tick = true;
    float m_tick_time = 0f;
    float spf = 0.033f;//30fps

    public void Start()
    {
        NativeDef.Init();
        ShaderMgr.Init();
        ui_sty.normal.textColor = Color.red;
        ui_sty.fontSize = 20;
    }

    void Update()
    {
        if (m_tick)
        {
            m_tick_time += Time.deltaTime;
            if (m_tick_time > spf)
            {
                NativeDef.iTickCore(m_tick_time);
                m_tick_time = 0;
            }
        }
    }

    
    public void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        TableGUI();
        NativeGUI();
        GUILayout.Space(20);
        GUILayout.TextArea(ui_rst, ui_sty, ui_op2);
        GUILayout.EndHorizontal();
    }

    void OnApplicationQuit()
    {
        NativeDef.iQuitCore();
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
            ui_sb.Length = 0;
            ui_sb.AppendLine("table line cnt: " + len);
            for (int i = 0; i < 26; i++)
            {
                var rst = CEquipSuit.GetRow(i);
                ui_sb.AppendLine("suitid:" + rst.SuitID + " name:" + rst.SuitName + " level:" + rst.Level + " effect2: " + rst.Effect2[0]);
            }
            ui_rst = ui_sb.ToString();
        }
        if (GUILayout.Button("Get-Statics-Row", ui_opt))
        {
            int len = CXEntityStatistics.length;
            ui_sb.Length = 0;
            ui_sb.AppendLine("table line cnt: " + len);
            for (int i = 0; i < 26; i++)
            {
                var rst = CXEntityStatistics.GetRow(i);
                ui_sb.AppendLine("uid:" + rst.UID + " name:" + rst.Name + " preid:" + rst.PresentID);
            }
            ui_rst = ui_sb.ToString();
        }
        if (GUILayout.Button("Get-Fashion-Row", ui_opt))
        {
            int len = CFashionList.length;
            ui_sb.Length = 0;
            ui_sb.AppendLine("table line cnt: " + len);
            for (int i = 0; i < 12; i++)
            {
                var rst = CFashionList.GetRow(i);
                ui_sb.AppendLine("uid:" + rst.ItemID + " name:" + rst.ItemName + " pos:" + rst.EquipPos);
            }
            ui_rst = ui_sb.ToString();
        }
        if (GUILayout.Button("Get-Fasuit-Row", ui_opt))
        {
            int len = CFashionSuit.length;
            ui_sb.Length = 0;
            ui_sb.AppendLine("table line cnt: " + len);
            for (int i = 0; i < 12; i++)
            {
                var rst = CFashionSuit.GetRow(i);
                ui_sb.AppendLine("uid:" + rst.SuitID + " name:" + rst.SuitName + " preid:" + rst.SuitIcon);
            }
            ui_rst = ui_sb.ToString();
        }
        if (GUILayout.Button("Get-Defaut-Row", ui_opt))
        {
            int len = CDefaultEquip.length;
            ui_sb.Length = 0;
            ui_sb.AppendLine("table line cnt: " + len);
            for (int i = 0; i < 12; i++)
            {
                var rst = CDefaultEquip.GetRow(i);
                ui_sb.AppendLine("uid:" + rst.ProfID + " hair:" + rst.Hair + " face:" + rst.Face);
            }
            ui_rst = ui_sb.ToString();
        }
        if (GUILayout.Button("Get-NPCList-Row", ui_opt))
        {
            int len = CXNpcList.length;
            ui_sb.Length = 0;
            ui_sb.AppendLine("table line cnt: " + len);
            for (int i = 0; i < 12; i++)
            {
                var rst = CXNpcList.GetRow(i);
                ui_sb.AppendLine("uid:" + rst.NPCID + " name:" + rst.NPCName + " type:" + rst.NPCType);
            }
            ui_rst = ui_sb.ToString();
        }
        if (GUILayout.Button("Get-Scene-Row", ui_opt))
        {
            int len = CSceneList.length;
            ui_sb.Length = 0;
            ui_sb.AppendLine("table line cnt: " + len);
            for (int i = 0; i < 12; i++)
            {
                var rst = CSceneList.GetRow(i);
                ui_sb.AppendLine("uid:" + rst.SceneID + " path:" + rst.ScenePath + " icon:" + rst.UIIcon);
            }
            ui_rst = ui_sb.ToString();
        }
        if (GUILayout.Button("Get-Present-Row", ui_opt))
        {
            int len = CXEntityPresentation.length;
            ui_sb.Length = 0;
            ui_sb.AppendLine("present table line cnt: " + len);
            for (int i = 0; i < 24; i++)
            {
                var rst = CXEntityPresentation.GetRow(i);
                ui_sb.AppendLine("uid:" + rst.UID + " name:" + rst.Name + " pref: " + rst.Prefab + " ani: " + rst.AnimLocation);
            }
            ui_rst = ui_sb.ToString();
        }
        GUILayout.EndVertical();
    }


    private void NativeGUI()
    {
        GUILayout.BeginVertical();
        if (GUILayout.Button("Cal-Add", ui_opt))
        {
            int i = NativeDef.iAdd(8, 7);
            ui_rst = "8+7=" + i;
        }
        if (GUILayout.Button("Cal-Sub", ui_opt))
        {
            int a = 8, b = 2;
            IntPtr p1 = Marshal.AllocCoTaskMem(Marshal.SizeOf(a));
            Marshal.StructureToPtr(a, p1, false);
            IntPtr p2 = Marshal.AllocCoTaskMem(Marshal.SizeOf(b));
            Marshal.StructureToPtr(b, p2, false);
            int rst = NativeDef.iSub(p1, p2);
            ui_rst = a + "-" + b + "=" + rst;
        }
        if (GUILayout.Button("Native-Json", ui_opt))
        {
            NativeDef.iJson(Application.streamingAssetsPath + "/Patch/json.txt");
            ui_rst = "native parse json finish!";
            XDebug.Log(ui_rst);
        }
        if (GUILayout.Button("Native-Patch", ui_opt))
        {
            string old = Application.streamingAssetsPath + "/Patch/old.txt";
            string diff = Application.streamingAssetsPath + "/Patch/diff.patch";
            string newf = Application.streamingAssetsPath + "/Patch/new.txt";
            XDebug.Log(old + " " + diff + " " + newf);
            NativeDef.iPatch(old, diff, newf);
            XDebug.Log("patch finish");
        }
        if (GUILayout.Button("Native-Start-Game", ui_opt))
        {
            m_tick = true;
            ui_rst = "start gamecore ";
            NativeDef.iStartCore();
        }
        if (GUILayout.Button("Native-Stop-Game", ui_opt))
        {
            m_tick = false;
            ui_rst = "stop gamecore";
            NativeDef.iStopCore();
        }
        if (GUILayout.Button("Native-Tick-Game", ui_opt))
        {
            m_tick = false;
            ui_rst = "tick gamecore";
            NativeDef.iTickCore(Time.deltaTime);
        }
        if (GUILayout.Button("Native-Quit-Game", ui_opt))
        {
            m_tick = false;
            ui_rst = "quit gamecore";
            NativeDef.iQuitCore();
        }
        GUILayout.EndVertical();
    }

}

#endif