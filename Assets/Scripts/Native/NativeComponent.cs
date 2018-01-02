public class NativeComponent
{
    private float _time = 0;
    private bool _double = false;
    protected NativeEntity entity;

    protected virtual UpdateState state
    {
        get { return UpdateState.NONE; }
    }

    public virtual uint ID
    {
        get { return XCommon.singleton.XHash(GetType().Name); }
    }
    

    public virtual void OnInitial(NativeEntity enty)
    {
        entity = enty;
    }

    public virtual void OnUninit()
    {
    }

    protected virtual void OnUpdate(float delta) { }


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



}
