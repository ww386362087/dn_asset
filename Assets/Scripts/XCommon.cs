using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;

public abstract class XBaseSingleton
{
    public abstract bool Init();
    public abstract void Uninit();
}

public abstract class XSingleton<T> : XBaseSingleton where T : new()
{
    protected XSingleton()
    {
        if (null != _instance) { throw new Exception(_instance.ToString() + @" can not be created again."); }
    }

    private static readonly T _instance = new T();

    public static T singleton { get { return _instance; } }

    public override bool Init() { return true; }
    public override void Uninit() { }
}


public class XCommon : XSingleton<XCommon>
{
    public XCommon()
    {
        _idx = 5;
    }

    public readonly float FrameStep = (1 / 30.0f);
    private static readonly float _eps = 0.0001f;

    public static float XEps
    {
        get { return _eps; }
    }


    private int _idx = 0;

    private int _new_id = 0;

    public int New_id { get { return ++_new_id; } }

    public long UniqueToken
    {
        get { return DateTime.Now.Ticks + New_id; }
    }

    public StringBuilder shareSB = new StringBuilder();

    public static List<Renderer> tmpRender = new List<Renderer>();
    public static List<ParticleSystem> tmpParticle = new List<ParticleSystem>();
    public static List<SkinnedMeshRenderer> tmpSkinRender = new List<SkinnedMeshRenderer>();
    public static List<MeshRenderer> tmpMeshRender = new List<MeshRenderer>();
    public uint XHash(string str)
    {
        if (str == null) return 0;

        uint hash = 0;
        for (int i = 0; i < str.Length; i++)
        {
            hash = (hash << _idx) + hash + str[i];
        }

        return hash;
    }
    public uint XHashLower(string str)
    {
        if (str == null) return 0;

        uint hash = 0;
        for (int i = 0; i < str.Length; i++)
        {
            char c = char.ToLower(str[i]);
            hash = (hash << _idx) + hash + c;
        }

        return hash;
    }
    public uint XHash(StringBuilder str)
    {
        if (str == null) return 0;

        uint hash = 0;
        for (int i = 0; i < str.Length; i++)
        {
            hash = (hash << _idx) + hash + str[i];
        }

        return hash;
    }

    public bool IsEqual(float a, float b)
    {
#if (PRECISION_USED)
            //return Mathf.Approximately(Mathf.Abs(a - b), _eps);
            return Mathf.Abs(a - b) < _eps;
#else
        return a == b;
#endif
    }

}
