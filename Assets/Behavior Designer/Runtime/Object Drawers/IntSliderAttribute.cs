using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.ObjectDrawers
{
    public class IntSliderAttribute : ObjectDrawerAttribute
    {
        public int min;
        public int max;

        public IntSliderAttribute(int mi, int ma)
        {
            min = mi;
            max = ma;
        }
    }
}