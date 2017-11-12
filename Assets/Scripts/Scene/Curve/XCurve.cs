using UnityEngine;

public class XCurve : MonoBehaviour , IXCurve
{
    public float Max_Value = 0;
    public float Land_Value = 0;
    public AnimationCurve Curve = new AnimationCurve();

	public int length { get {return Curve.length;} }

	public float Evaluate(float time)
	{
        return time < Land_Value ? Curve.Evaluate(time) : 0;
	}

	public float GetValue(int index)
	{
		return Curve [index].value;
	}

	public float GetTime(int index)
	{
		return Curve [index].time;
	}

    public float GetMaxValue()
    {
        return Max_Value;
    }

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
