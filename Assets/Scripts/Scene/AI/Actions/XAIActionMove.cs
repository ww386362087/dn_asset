using AI;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class NavToTarget : Action
{
    public SharedTransform mAIArgTarget;
    public SharedTransform mAIArgNavTarget;
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
                    if (XAIGeneralMgr.singleton.ActionNav(transform, mAIArgNavPos.Value))
                        return TaskStatus.Success;
                    else
                        return TaskStatus.Failure;
                }

            }
            else
            {
                if (XAIGeneralMgr.singleton.NavToTarget(transform, mAIArgNavTarget.Value.gameObject))
                    return TaskStatus.Success;
                else
                    return TaskStatus.Failure;
            }
        }
        else
        {
            if (XAIGeneralMgr.singleton.NavToTarget(transform, mAIArgTarget.Value.gameObject))
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}


public class RotateToTarget : Action
{
    public override TaskStatus OnUpdate()
    {
        if (XAIGeneralMgr.singleton.RotateToTarget(transform))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}


public class DetectEnimyInSight : Action
{
    public override TaskStatus OnUpdate()
    {
        if (XAIGeneralMgr.singleton.DetectEnemyInSight(transform))
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}