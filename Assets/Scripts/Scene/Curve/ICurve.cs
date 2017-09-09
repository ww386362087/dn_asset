public interface IXCurve
{
    int length { get; }

    float Evaluate(float time);
    float GetLandValue();
    float GetMaxValue();
    float GetTime(int index);
    float GetValue(int index);
}
