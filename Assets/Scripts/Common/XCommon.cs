using UnityEngine;
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


public class XCommon : XSingleton<XCommon>
{
    public XCommon()
    {
        _idx = 5;
    }

    public const float FrameStep = (1 / 30.0f);
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

    public static List<Renderer> tmpRender = new List<Renderer>();

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


    /// <summary>
    /// 叉积
    /// </summary>
    private float CrossProduct(float x1, float z1, float x2, float z2)
    {
        return x1 * z2 - x2 * z1;
    }


    /// <summary>
    /// 线段是否相交 叉积运算
    /// </summary>
    public bool IsLineSegmentCross(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2)
    {
        //fast detect
        if (Mathf.Min(p1.x, p2.x) <= Mathf.Max(q1.x, q2.x) &&
            Mathf.Min(q1.x, q2.x) <= Mathf.Max(p1.x, p2.x) &&
            Mathf.Min(p1.z, p2.z) <= Mathf.Max(q1.z, q2.z) &&
            Mathf.Min(q1.z, q2.z) <= Mathf.Max(p1.z, p2.z))
        {
            //( p1 - q1 ) * ( q2 - q1 )
            float p1xq = CrossProduct(p1.x - q1.x, p1.z - q1.z,
                                       q2.x - q1.x, q2.z - q1.z);
            //( p2 - q1 ) * ( q2 - q1 )
            float p2xq = CrossProduct(p2.x - q1.x, p2.z - q1.z,
                                       q2.x - q1.x, q2.z - q1.z);

            //( q1 - p1 ) * ( p2 - p1 )
            float q1xp = CrossProduct(q1.x - p1.x, q1.z - p1.z,
                                       p2.x - p1.x, p2.z - p1.z);
            //( q2 - p1 ) * ( p2 - p1 )
            float q2xp = CrossProduct(q2.x - p1.x, q2.z - p1.z,
                                       p2.x - p1.x, p2.z - p1.z);

            return ((p1xq * p2xq <= 0) && (q1xp * q2xp <= 0));
        }

        return false;
    }

    public Vector3 Horizontal(Vector3 v)
    {
        v.y = 0;
        return v.normalized;
    }


    public void Horizontal(ref Vector3 v)
    {
        v.y = 0;
        v.Normalize();
    }

    public Vector2 HorizontalRotateVetor2(Vector2 v, float degree, bool normalized = true)
    {
        degree = -degree;

        float rad = degree * Mathf.Deg2Rad;
        float sinA = Mathf.Sin(rad);
        float cosA = Mathf.Cos(rad);

        float x = v.x * cosA - v.y * sinA;
        float y = v.x * sinA + v.y * cosA;

        v.x = x;
        v.y = y;
        return normalized ? v.normalized : v;
    }

    public Vector3 HorizontalRotateVetor3(Vector3 v, float degree, bool normalized = true)
    {
        degree = -degree;

        float rad = degree * Mathf.Deg2Rad;
        float sinA = Mathf.Sin(rad);
        float cosA = Mathf.Cos(rad);

        float x = v.x * cosA - v.z * sinA;
        float z = v.x * sinA + v.z * cosA;

        v.x = x;
        v.z = z;
        return normalized ? v.normalized : v;
    }


    /// <summary>
    /// 顺时针旋转
    /// </summary>
    public bool Clockwise(Vector3 fiduciary, Vector3 relativity)
    {
        float r = fiduciary.z * relativity.x - fiduciary.x * relativity.z;
        return r > 0;
    }


    /// <summary>
    /// 顺时针旋转 
    /// </summary>
    public bool Clockwise(Vector2 fiduciary, Vector2 relativity)
    {
        float r = fiduciary.y * relativity.x - fiduciary.x * relativity.y;
        return r > 0;
    }

    public Vector3 FloatToAngle(float angle)
    {
        return Quaternion.Euler(0, angle, 0) * Vector3.forward;
    }

    public float AngleToFloat(Vector3 dir)
    {
        float face = Vector3.Angle(Vector3.forward, dir);
        return XCommon.singleton.Clockwise(Vector3.forward, dir) ? face : -face;
    }

    public Transform FindChildRecursively(Transform t, string name)
    {
        if (t.name == name)
            return t;
        for (int i = 0; i < t.childCount; i++)
        {
            Transform ct = FindChildRecursively(t.GetChild(i), name);
            if (ct != null)
                return ct;
        }
        return null;
    }
}
