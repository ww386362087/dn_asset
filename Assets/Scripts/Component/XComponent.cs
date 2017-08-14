
/// <summary>
/// 组件component 也可以挂载组件
/// 子子孙孙无穷尽也 
/// </summary>
public class XComponent : XObject
{

    public XObject obj = null;

    public virtual uint ID { get { return XCommon.singleton.XHash(this.GetType().Name); } }

    public virtual void OnInitial(XObject _obj)
    {
        base.Initilize();
        obj = _obj;
    }

    public virtual void OnUninit()
    {
        obj = null;
        base.Unload();
    }


    public bool IsRoleComponent()
    {
        return obj is XRole;
    }

    public bool IsEntityComponent()
    {
        return obj is XEntity;
    }


    public bool IsCameraComponent()
    {
        return obj is XCamera;
    }

}
