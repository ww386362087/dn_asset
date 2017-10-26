using AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Game")]
[TaskDescription("寻路去锁定目标")]
public class NavToTarget : Action
{
    public SharedGameObject mAIArgTarget;
    public SharedGameObject mAIArgNavTarget;
    public SharedVector3 mAIArgNavPos;

    public override TaskStatus OnUpdate()
    {
        if (mAIArgTarget.GetValue() == null)
        {
            if (mAIArgNavTarget.GetValue() == null)
            {
                if (mAIArgNavPos.Value == Vector3.zero)
                    return TaskStatus.Failure;
                else
                {
                    if (XAIUtil.ActionNav(transform, mAIArgNavPos.Value))
                        return TaskStatus.Success;
                    else
                        return TaskStatus.Failure;
                }
            }
            else
            {
                if (XAIUtil.NavToTarget(transform, mAIArgNavTarget.Value))
                    return TaskStatus.Success;
                else
                    return TaskStatus.Failure;
            }
        }
        else
        {
            if (XAIUtil.NavToTarget(transform, mAIArgTarget.Value))
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}

[TaskCategory("Game")]
[TaskDescription("方向对准目标")]
public class RotateToTarget : Action
{
    public SharedGameObject mAIArgTarget;

    public override TaskStatus OnUpdate()
    {
        if (XAIUtil.RotateToTarget(transform,mAIArgTarget.Value))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

[TaskCategory("Game")]
public class DetectEnimyInSight : Action
{
    public override TaskStatus OnUpdate()
    {
        if (XAIUtil.DetectEnemyInSight(transform))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}