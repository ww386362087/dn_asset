using System.Collections.Generic;
using XTable;

public class XQTEStatusLibrary
{
    private static QteStatusList _table = new QteStatusList();
    public static List<string> NameList = null;
   // public static List<string> KeyList = null;

    static XQTEStatusLibrary()
    {
        
        NameList = new List<string>();
        //KeyList = new List<string>();

        for (int i = 0; i < _table.Table.Length; ++i)
        {
            QteStatusList.RowData row = _table.Table[i];
            NameList.Add(row.Value + " " + row.Name);
        }

        //for (int i = 0; i <= (int)KKSG.XSkillSlot.Attack_Max; i++)
        //    KeyList.Add((i).ToString());
    }

    public static int GetStatusValue(int idx)
    {
        if (idx < 0 || idx >= NameList.Count) return 0;

        string[] strs = NameList.ToArray();

        for (int i = 0; i < _table.Table.Length; ++i)
        {
            QteStatusList.RowData row = _table.Table[i];
            if ((row.Value + " " + row.Name) == strs[idx])
                return (int)row.Value;
        }

        return 0;
    }

    public static int GetStatusIdx(int qte)
    {
        string[] strs = NameList.ToArray();

        string str = null;
        for (int i = 0; i < _table.Table.Length; ++i)
        {
            QteStatusList.RowData row = _table.Table[i];
            if (row.Value == qte)
            {
                str = (row.Value + " " + row.Name);
                break;
            }
        }

        if (str != null)
        {
            for (int i = 0; i < strs.Length; i++)
            {
                if (strs[i] == str) return i;
            }
        }

        return 0;
    }
}