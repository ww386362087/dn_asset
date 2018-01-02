
public enum UpdateState
{
    NONE,  //不调用
    TIMER, //每秒一次
    FRAME, //每帧调用
    DOUBLE,//每两帧调用
}

/// <summary>
/// 组件component 也可以挂载组件
/// 子子孙孙无穷尽也 
/// </summary>
public class XComponent : XObject
{
    
    /// <summary>
    /// 被挂载的对象
    /// </summary>
    public XObject xobj = null;

    public virtual uint ID { get { return XCommon.singleton.XHash(GetType().Name); } }

    protected virtual UpdateState state { get { return UpdateState.NONE; } }

    private bool _double = false;
    private float _time = 0;
    
    public virtual void OnInitial(XObject _obj)
    {
        base.Initilize();
        xobj = _obj;
        _double = false;
        _time = 0;
    }

    public virtual void OnUninit()
    {
        xobj = null;
        _double = false;
        _time = 0;
        base.Unload();
    }

    public void Update(float delta)
    {
        _time += delta;
        switch (state)
        {
            case UpdateState.FRAME:
                OnUpdate(delta);
                break;
            case UpdateState.DOUBLE:
                if (_double) OnUpdate(delta);
                _double = !_double;
                break;
            case UpdateState.TIMER:
                if (_time >= 1f)
                {
                    OnUpdate(delta);
                    _time = 0;
                }
                break;
            default:
                break;
        }
    }

    public virtual void OnUpdate(float delta)
    {
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
