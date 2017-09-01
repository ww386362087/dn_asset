
public abstract class XDocument : XObject
{
    protected virtual string _name { get { return GetType().Name; } }

    public virtual uint ID { get { return XCommon.singleton.XHash(_name); } }

    public virtual void OnInitial() { }

    public virtual void OnUninitial() { }

    public abstract void OnReconnected();
    
    public virtual void OnEnterSceneFinally() { }

    public virtual void OnEnterScene() { }
    
    public virtual void OnLeaveScene() { }

    public virtual void OnAttachToHost() { }

    public virtual void OnDeatchToHost() { }

    public virtual void OnUpdate() { }
}
