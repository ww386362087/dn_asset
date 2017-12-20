using System.Collections.Generic;


/// <summary>
/// 三个功能：
/// 1.对象生命周期
/// 2.分发事件
/// 3.挂载component, 作为对象的属性
/// </summary>
public class XObject
{
    protected Dictionary<uint, XComponent> components;

    private Dictionary<XEventDefine, EventHandler> _eventMap;

    public bool Deprecated { get; set; }

    protected void Initilize()
    {
        Deprecated = false;
        components = new Dictionary<uint, XComponent>();
        EventSubscribe();
    }

    protected void Unload()
    {
        DetachAllComponents();
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

    private void CheckCondtion()
    {
        if (components == null)
        {
            throw new XComponentException("object is not initial ");
        }
    }


    public T AttachComponent<T>() where T : XComponent, new()
    {
        CheckCondtion();
        uint uid = XCommon.singleton.XHash(typeof(T).Name);
        if (components.ContainsKey(uid))
        {
            return components[uid] as T;
        }
        else
        {
            T com = new T();
            com.OnInitial(this);
            components.Add(uid, com);
            return com;
        }
    }

    public bool DetachComponent<T>() where T : XComponent, new()
    {
        return DetachComponent(typeof(T).Name);
    }


    public T GetComponent<T>() where T : XComponent
    {
        uint uid = XCommon.singleton.XHash(typeof(T).Name);
        if (components != null && components.ContainsKey(uid)) return components[uid] as T;
        return null;
    }

    //for native or lua interface
    public object GetComponent(string name)
    {
        uint uid = XCommon.singleton.XHash(name);
        if (components != null && components.ContainsKey(uid)) return components[uid];
        return null;
    }

    //for native or lua interface
    public bool DetachComponent(string name)
    {
        uint uid = XCommon.singleton.XHash(name);
        if (components != null && components.ContainsKey(uid))
        {
            components[uid].OnUninit();
            components[uid] = null;
            components.Remove(uid);
            return true;
        }
        return false;
    }


    public void DetachAllComponents()
    {
        if (components != null)
        {
            var e = components.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Value.OnUninit();
            }
        }
        components.Clear();
    }

    protected void UpdateComponents(float delta)
    {
        if (components != null)
        {
            var e = components.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Value.Update(delta);
            }
        }
    }

}
