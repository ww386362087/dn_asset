using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.ObjectDrawers
{
    public class FloatSliderAttribute : ObjectDrawerAttribute
    {
        public float min;
        public float max;

        public FloatSliderAttribute(float mi, float ma)
        {
            min = mi;
            max = ma;
        }
    }
}