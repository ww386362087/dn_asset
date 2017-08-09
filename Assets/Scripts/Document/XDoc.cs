using UnityEngine;
using System;
using System.Diagnostics;

public abstract class XDoc : XObject
{

    protected virtual string _name { get { return GetType().Name; } }

    public virtual uint ID { get { return XCommon.singleton.XHash(_name); } }

 
    public void Init() { OnInitial(); }

    public void UnInit() { OnInitial(); }

    public void EnterScene() { OnEnterScene(); }

    public void EnterSceneFinally() { OnEnterSceneFinally(); }

    public void LeaveScene() { OnLeaveScene(); }

    public void AttachHost() { OnAttachToHost(); }

    public void DeatchHost() { OnDeatchToHost(); }

    public void Reconnect() { OnReconnected(); }

    public void Update() { OnUpdate(); }

    protected virtual void OnInitial() { }

    protected virtual void OnUninitial() { }

    protected abstract void OnReconnected();
    
    protected virtual void OnEnterSceneFinally() { }

    protected virtual void OnEnterScene() { }


    protected virtual void OnLeaveScene() { }

    protected virtual void OnAttachToHost() { }

    protected virtual void OnDeatchToHost() { }

    protected virtual void OnUpdate() { }
}
