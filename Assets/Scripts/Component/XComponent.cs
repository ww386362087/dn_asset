
/// <summary>
/// 组件component 也可以挂载组件
/// 子子孙孙无穷尽也 
/// </summary>
public class XComponent : XObject
{

    public XObject xobj = null;

    public virtual uint ID { get { return XCommon.singleton.XHash(this.GetType().Name); } }

    public virtual void OnInitial(XObject _obj)
    {
        base.Initilize();
        xobj = _obj;
    }

    public virtual void OnUninit()
    {
        xobj = null;
        base.Unload();
    }


    public bool IsRoleComponent()
    {
        return xobj is XRole;
    }

    public bool IsEntityComponent()
    {
        return xobj is XEntity;
    }


    public bool IsCameraComponent()
    {
        return xobj is XCamera;
    }

}
