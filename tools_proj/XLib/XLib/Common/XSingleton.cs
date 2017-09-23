using System;

public abstract class XBaseSingleton
{
    public abstract bool Init();

    public abstract void Uninit();
}

public abstract class XSingleton<T> : XBaseSingleton where T : new()
{
    protected XSingleton()
    {
        if (null != _instance)
        {
            throw new Exception(_instance.ToString() + @" can not be created again.");
        }
    }

    private static readonly T _instance = new T();

    public static T singleton { get { return _instance; } }

    public override bool Init() { return true; }

    public override void Uninit() { }
}