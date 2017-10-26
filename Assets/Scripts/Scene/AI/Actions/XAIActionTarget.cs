using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using AI;
using UnityEngine;

[TaskCategory("Game")]
[TaskDescription("选择目标")]
public class FindTargetByDistance : Action
{
    public SharedFloat mAIArgDistance;
    public float mAIArgAngle;

    public override TaskStatus OnUpdate()
    {
        if (XAIUtil.FindTargetByDistance(transform, mAIArgDistance.Value, mAIArgAngle))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

[TaskCategory("Game")]
[TaskDescription("锁定最近的目标")]
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

[TaskCategory("Game")]
[TaskDescription("锁定最远的目标")]
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

[TaskCategory("Game")]
[TaskDescription("随机锁定目标")]
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

[TaskCategory("Game")]
[TaskDescription("给树里参数计算距离")]
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

[TaskCategory("Game")]
[TaskDescription("根据statistic id锁定目标")]
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
