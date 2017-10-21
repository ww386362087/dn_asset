using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IsInSight : Conditional
{
    public float viewLength;
    public SharedTransform target;

    public override TaskStatus OnUpdate()
    {

        return TaskStatus.Success;
    }
}
