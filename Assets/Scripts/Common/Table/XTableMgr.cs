using System.Threading;
using XTable;
using System.Collections.Generic;

public class XTableMgr
{
    const int ThreadCnt = 2;

    static Dictionary<uint, CVSReader> readers;

    public static System.Action tableLoaded;
    static bool loadFinish = false;
    static int loadIndex = 0;
    static int tableCNT = 0;

    public static void Initial()
    { 
        Add<DefaultEquip>();
        Add<EquipSuit>();
        Add<FashionList>();
        Add<FashionSuit>();
        Add<QteStatusList>();
        Add<SceneList>();
        Add<XEntityStatistics>();
        Add<XEntityPresentation>();
        Add<XNpcList>();

        loadIndex = 0;
        loadFinish = false;
        tableCNT = readers.Count;
        ThreadLoad();
    }

    public static void Update()
    {
        if (!loadFinish) CheckFinish();
    }

    private static void CheckFinish()
    {
        if (loadIndex >= tableCNT )
        {
            loadFinish = true;
            if(tableLoaded != null) tableLoaded();
          //  XDebug.LogGreen("All Table loadfinish!");
        }
    }

    private static void Add<T>() where T : CVSReader, new()
    {
        if (readers == null) readers = new Dictionary<uint, CVSReader>();
        uint uid = XCommon.singleton.XHash(typeof(T).Name);
        CVSReader reader = new T();
        readers.Add(uid, reader);
    }


    /// <summary>
    /// 在线程池里加载 - 异步不阻塞
    /// </summary>
    public static void ThreadLoad()
    {
        ThreadPool.SetMaxThreads(ThreadCnt, ThreadCnt);
        var e = readers.GetEnumerator();
        while (e.MoveNext())
        {
            ThreadPool.QueueUserWorkItem(LoadTable, e.Current.Value);
        }
        e.Dispose();
        Thread.Sleep(1);
    }


    public static void LoadTable(object reader)
    {
        (reader as CVSReader).Create();
        loadIndex++;
      //  XDebug.Log("load: " + reader.GetType().Name + " idnex: " + loadIndex);
    }

    public static T GetTable<T>() where T : CVSReader, new()
    {
        uint uid = XCommon.singleton.XHash(typeof(T).Name);
        if (!readers.ContainsKey(uid))
        {
            Add<T>();
            readers[uid].Create();
        }
        return readers[uid] as T;

    }
    
}

