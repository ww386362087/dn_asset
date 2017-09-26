using System.Threading;
using XTable;
using System.Collections.Generic;


public class BaseRow
{
    public int sortID;
}

public class XTableMgr
{
    const int ThreadCnt = 2;

    static Dictionary<uint, CVSReader> readers;

    public static System.Action<bool> tableLoaded;
    static bool loadFinish = false;

    public static void Initial()
    { 
        Add<DefaultEquip>();
        Add<EquipSuit>();
        Add<FashionList>();
        Add<FashionSuit>();
        Add<QteStatusList>();
        Add<SceneList>();
        Add<XEntityPresentation>();
		Add<XEntityStatistics>();
		Add<XNpcList>();
		loadFinish = false;
        ThreadLoad();
    }

    public static void Update()
    {
        if (!loadFinish) CheckFinish();
    }

    private static void CheckFinish()
    {
        if (readers != null)
        {
            bool finish = true;
            Dictionary<uint, CVSReader>.Enumerator e = readers.GetEnumerator();
            while (e.MoveNext())
            {
                if (!e.Current.Value.isDone)
                {
                    finish = false;
                    break;
                }
            }
            if (finish)
            {
                loadFinish = true;
                if(tableLoaded!=null)
                {
                    tableLoaded(true);
                }
                //tableLoaded?.Invoke(true);
            }
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
        Dictionary<uint, CVSReader>.Enumerator e= readers.GetEnumerator();
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
    }

    public static T GetTable<T>() where T : CVSReader, new()
    {
        uint uid = XCommon.singleton.XHash(typeof(T).Name);
        if (readers == null)
        {
            //此情况下主要是编辑器模式会用到 一开始并没有读取所有的表格
            Add<T>();
            readers[uid].Create();
        }
        return readers[uid] as T;

    }
    
}

