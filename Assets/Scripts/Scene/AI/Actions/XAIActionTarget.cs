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
        if (XAIUtil.FindTargetByDistance(transform.gameObject, mAIArgDistance.Value, mAIArgAngle))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class DoSelectNearest : Action
{
    public override TaskStatus OnUpdate()
    {
        if (XAIUtil.DoSelectNearest(transform))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class DoSelectFarthest : Action
{
    public override TaskStatus OnUpdate()
    {
        if (XAIUtil.DoSelectFarthest(transform))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class DoSelectRandomTarget : Action
{
    public override TaskStatus OnUpdate()
    {
        if (XAIUtil.DoSelectRandomTarget(transform))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class CalDistance : Action
{
    public SharedTransform mAIArgObject;
    public SharedFloat mAIArgDistance;
    public SharedVector3 mAIArgDestPoint;


    public override TaskStatus OnUpdate()
    {
        if (mAIArgObject.Value != null)
        {
            mAIArgDistance.Value = (transform.position - mAIArgObject.Value.position).magnitude;
        }
        else
        {
            mAIArgDistance.Value = (transform.position - mAIArgDestPoint.Value).magnitude;
        }
        return TaskStatus.Success;
    }
}


public class SelectMoveTargetById : Action
{
    public SharedTransform mAIArgMoveTarget;
    public int mAIArgObjectId;

    public override TaskStatus OnUpdate()
    {
        Transform moveTarget = XAIUtil.SelectMoveTargetById(transform, mAIArgObjectId);

        if (moveTarget == null)
            return TaskStatus.Failure;
        else
        {
            mAIArgMoveTarget.Value = moveTarget;
            return TaskStatus.Success;
        }
    }
}
