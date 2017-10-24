using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using AI;

public class FindTargetByDistance : Action
{
    public SharedFloat mAIArgDistance;
    public float mAIArgAngle;

    public override TaskStatus OnUpdate()
    {
        if (XAIGeneralMgr.singleton.FindTargetByDistance(transform.gameObject, mAIArgDistance.Value, mAIArgAngle))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class DoSelectNearest : Action
{
    public override TaskStatus OnUpdate()
    {
        if (XAIGeneralMgr.singleton.DoSelectNearest(transform.gameObject))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}
