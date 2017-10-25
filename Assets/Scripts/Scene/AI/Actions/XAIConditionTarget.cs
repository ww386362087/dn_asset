using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using AI;
using UnityEngine;


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


public class SelectMoveTargetById : Action
{
    public SharedTransform mAIArgMoveTarget;
    public int mAIArgObjectId;

    public override TaskStatus OnUpdate()
    {
        Transform moveTarget = XAIGeneralMgr.singleton.SelectMoveTargetById(transform, mAIArgObjectId);

        if (moveTarget == null)
            return TaskStatus.Failure;
        else
        {
            mAIArgMoveTarget.Value = moveTarget;
            return TaskStatus.Success;
        }
    }
}
