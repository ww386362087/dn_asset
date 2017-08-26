using System.Collections.Generic;


public delegate void XEventHandler(XEventArgs e);

public struct EventHandler
{
    public XEventDefine eventDefine;
    public XEventHandler handler;
}

public class XEventMgr : XSingleton<XEventMgr>
{


    public Dictionary<XEventDefine, HashSet<XObject>> pool;


    public void AddRegist(XEventDefine def, XObject obj)
    {
        if (pool == null)
            pool = new Dictionary<XEventDefine, HashSet<XObject>>();

        if (pool.ContainsKey(def))
        {
            pool[def].Add(obj);
        }
        else
        {
            HashSet<XObject> hash = new HashSet<XObject>();
            hash.Add(obj);
            pool.Add(def, hash);
        }
    }


    public void RemoveRegist(XObject o)
    {
        if (pool != null)
        {
            List<XEventDefine> list = new List<XEventDefine>();
            foreach (var item in pool)
            {
                if (item.Value.Contains(o))
                {
                    item.Value.Remove(o);
                    list.Add(item.Key);
                }
            }
            //从池子移掉不再需要的对象 减少遍历的时间
            for (int i = 0, max = list.Count; i < max; i++)
            {
                if (pool[list[i]].Count == 0) pool.Remove(list[i]);
            }
        }
    }


    public bool FireEvent(XEventArgs args)
    {
        if (pool != null)
        {
            if (pool.ContainsKey(args.ArgsDefine))
            {
                var objs = pool[args.ArgsDefine];
                var e = objs.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.DispatchEvent(args);
                }
                if (!args.ManualRecycle) args.Recycle();
                return true;
            }
        }
        return false;
    }


    public bool FireEvent(XEventArgs args, float delay)
    {
        TimerManager.singleton.AddTimer((int)(delay * 1000), 0, OnTimer, args);
        return true;
    }

    private void OnTimer(int seq, object obj)
    {
        FireEvent((XEventArgs)obj);
    }

}

