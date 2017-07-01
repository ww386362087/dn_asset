using System;
using UnityEngine;
using System.Threading;
using System.IO;
using System.Collections.Generic;


public delegate void OnLoadedCallback();

internal class XFileReadAsync
{
    internal bool IsDone = false;
    internal string Location = null;

    internal CVSReader Reader = null;
    internal Stream Data = null;
}

public sealed class XTableAsyncLoader
{
    private bool _executing = false;

    private List<XFileReadAsync> _task_list = new List<XFileReadAsync>();
    private OnLoadedCallback _call_back = null;
    private XFileReadAsync currentXfra = null;
    public static int currentThreadCount = 0;
    public static readonly int AsyncPerTime = 8;
    public bool IsDone
    {
        get
        {
            bool done = false;
            if (currentXfra == null)
            {
                if (currentThreadCount < AsyncPerTime)
                {
                    currentThreadCount++;
                }
                done = InnerExecute();

            }
            if (done)
            {
                if (_call_back != null)
                {
                    _call_back();
                    _call_back = null;
                }

                _executing = false;
            }
            return done;
        }
    }

    public void AddTask(string location, CVSReader reader)
    {
        XFileReadAsync xfra = new XFileReadAsync();
        xfra.Location = location;
        xfra.Reader = reader;

        _task_list.Add(xfra);
    }

    private bool InnerExecute()
    {

        if (_task_list.Count > 0)
        {
            if (currentThreadCount <= 0)
            {
                return false;
            }
            currentThreadCount--;
            currentXfra = _task_list[_task_list.Count - 1];
            _task_list.RemoveAt(_task_list.Count - 1);
            ReadFileAsync(currentXfra);
            return false;
        }
        return true;
    }
    public bool Execute(OnLoadedCallback callback = null)
    {
        if (_executing) return false;

        _call_back = callback;
        _executing = true;

        InnerExecute();
        return true;
    }

    public static Dictionary<string, string> tableScriptMap;
    public static void DumpTableScriptMap()
    {
        TableMap tableMap = null;
        string xml = "Assets/Editor/ResImporter/ImporterData/Table/Table.xml";
        try
        {
            System.Xml.Serialization.XmlSerializer formatter = new System.Xml.Serialization.XmlSerializer(typeof(TableMap));
            using (FileStream reader = new FileStream(xml, FileMode.Open))
            {
                tableMap = formatter.Deserialize(reader) as TableMap;
            }
            if (tableMap != null)
            {
                tableMap.tableScriptMap.Clear();
                foreach (KeyValuePair<string, string> kvp in tableScriptMap)
                {
                    TableScriptMap tsm = new TableScriptMap();
                    tsm.table = kvp.Key;
                    tsm.script = kvp.Value;
                    tableMap.tableScriptMap.Add(tsm);
                }
                using (FileStream writer = new FileStream(xml, FileMode.Create))
                {
                    //using Encoding
                    StreamWriter sw = new StreamWriter(writer, System.Text.Encoding.UTF8);
                    System.Xml.Serialization.XmlSerializerNamespaces xsn = new System.Xml.Serialization.XmlSerializerNamespaces();
                    //empty name spaces
                    xsn.Add(string.Empty, string.Empty);
                    formatter.Serialize(sw, tableMap, xsn);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    public static void AddTableScript(string location, Type reader)
    {
        if (tableScriptMap == null)
            tableScriptMap = new Dictionary<string, string>();
        string table = location;
        table = table.Replace("Table/", "");
        tableScriptMap[table] = reader.Name;
    }

    public bool ReadText(string location, MemoryStream stream, bool error = true)
    {
        bool success = true;
        TextAsset data = Resources.Load<TextAsset>(location);

        if (data == null)
        {
            if (error) Debug.LogError(location);
            return false;
        }
        try
        {
            stream.Write(data.bytes, 0, data.bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message+location);
            success = false;
        }
        finally
        {
            Resources.UnloadAsset(data);
        }
        return success;
    }

    private void ReadFileAsync(XFileReadAsync xfra)
    {
        AddTableScript(xfra.Location, xfra.Reader.GetType());
        xfra.Data = new MemoryStream();
        bool success = ReadText(xfra.Location, xfra.Data as MemoryStream, false);

        if (!success)
        {
            Debug.LogError(xfra.Location);
            xfra.Data.Close();
            xfra.Data = null;
            xfra.IsDone = true;
            currentXfra = null;
            return;
        }

        ThreadPool.QueueUserWorkItem(delegate(object state)
        {
            try
            {
                bool res = true;
                res = xfra.Reader.ReadFile(xfra.Data);

                if (!res)
                    Debug.LogError("in File: " + xfra.Location + xfra.Reader.error);
                else
                    xfra.IsDone = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + " in File: " + xfra.Location + xfra.Reader.error);
            }

            if (xfra.Data != null)
            {
                xfra.Data.Close();
                xfra.Data = null;
            }
            currentXfra = null;
        });
    }
}
