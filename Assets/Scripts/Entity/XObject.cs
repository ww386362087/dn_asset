using System.Collections.Generic;


/// <summary>
/// 三个功能：
/// 1.对象生命周期
/// 2.分发事件
/// </summary>
public class XObject
{
   
    private Dictionary<XEventDefine, EventHandler> _eventMap;

    public bool Deprecated { get; set; }

    protected void Initilize()
    {
        Deprecated = false;
        EventSubscribe();
    }

    protected void Unload()
    {
        EventUnsubscribe();
        Deprecated = true;
    }


    /// <summary>
    /// 注册事件
    /// </summary>
    protected virtual void EventSubscribe()
    {
    }

    /// <summary>
    /// 销毁事件
    /// </summary>
    protected void EventUnsubscribe()
    {
        if (_eventMap != null)
            _eventMap.Clear();
        _eventMap = null;
        XEventMgr.singleton.RemoveRegist(this);
    }

    protected void RegisterEvent(XEventDefine eventID, XEventHandler handler)
    {
        if (!Deprecated)
        {
            if (_eventMap == null)
                _eventMap = new Dictionary<XEventDefine, EventHandler>();
            if (_eventMap.ContainsKey(eventID)) return;
            EventHandler eh = new EventHandler();
            eh.eventDefine = eventID;
            eh.handler = handler;
            _eventMap.Add(eventID, eh);
            XEventMgr.singleton.AddRegist(eventID, this);
        }
    }

    public virtual bool DispatchEvent(XEventArgs e)
    {
        if (!Deprecated && _eventMap != null)
        {
            if (_eventMap.ContainsKey(e.ArgsDefine))
            {
                var etor = _eventMap.GetEnumerator();
                while (etor.MoveNext())
                {
                    EventHandler eh = etor.Current.Value;
                    if (eh.eventDefine == e.ArgsDefine)
                    {
                        XEventHandler func = eh.handler;
                        if (func != null) func(e);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    
}
