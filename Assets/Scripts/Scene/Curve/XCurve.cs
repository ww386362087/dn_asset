using UnityEngine;

public class XCurve : MonoBehaviour, IXCurve
{
    public float Max_Value = 0;
    public float Land_Value = 0;
    public AnimationCurve Curve = new AnimationCurve();

    public int length { get { return Curve.length; } }

    public float Evaluate(float time)
    {
        return Curve.Evaluate(time);
    }
    public float GetValue(int index)
    {
        return Curve[index].value;
    }

    public float GetTime(int index)
    {
        return Curve[index].time;
    }

    /// <summary>
    /// 曲线最大值
    /// </summary>
    public float GetMaxValue()
    {
        return Max_Value;
    }

    /// <summary>
    /// 返回的是着陆时间
    /// </summary>
    public float GetLandValue()
    {
        return Land_Value;
    }

    public bool Deprecated
    {
        get;
        set;
    }
}
