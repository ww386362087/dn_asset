using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// 处理对象生命周期、事件相关
/// </summary>
public class XObject
{

    private List<EventHandler> _eventMap;
    
    public bool Deprecated { get; set; }
    
    protected void Initilize()
    {
        Deprecated = false;
        EventSubscribe();
    }

    protected void Unload()
    {
        if (_eventMap != null)
            _eventMap.Clear();
        _eventMap = null;
        Deprecated = true;
    }

    /// <summary>
    /// 登录进入
    /// </summary>
    protected virtual void OnAttachToHost()
    {
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    protected virtual void OnDetachFromHost()
    {
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
        _eventMap.Clear();
    }

    protected void RegisterEvent(XEventDefine eventID, XEventHandler handler)
    {
        if (!Deprecated)
        {
            if (_eventMap == null)
                _eventMap = new List<EventHandler>();
            int length = _eventMap.Count;
            for (int i = 0; i < length; ++i)
            {
                EventHandler ceh = _eventMap[i];
                if (ceh.eventDefine == eventID) return;
            }
            EventHandler eh = new EventHandler();
            eh.eventDefine = eventID;
            eh.handler = handler;
            _eventMap.Add(eh);
        }
    }

    public virtual bool DispatchEvent(XEventArgs e)
    {
        if (!Deprecated && _eventMap!=null)
        {
            int length = _eventMap.Count;
            for (int i = 0; i < length; ++i)
            {
                EventHandler eh = _eventMap[i];
                if (eh.eventDefine == e.ArgsDefine)
                {
                    XEventHandler func = eh.handler;
                    if (func != null) func(e);
                    return true;
                }
            }
        }
        return false;
    }
}
