using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using AI;

[TaskCategory("Game")]
[TaskDescription("选择目标")]
public class FindTargetByDistance : Action
{
    public SharedFloat mAIArgDistance;
    public float mAIArgAngle;

    public override TaskStatus OnUpdate()
    {
        XEntity e = AITreeImpleted.Transform2Entity(transform);
        return (TaskStatus)AITreeImpleted.FindTargetByDistanceUpdate(e, mAIArgDistance.Value, mAIArgAngle);
    }
}

[TaskCategory("Game")]
[TaskDescription("锁定最近的目标")]
public class DoSelectNearest : Action
{
    public override TaskStatus OnUpdate()
    {
        XEntity e = AITreeImpleted.Transform2Entity(transform);
        return (TaskStatus)AITreeImpleted.DoSelectNearestUpdate(e);
    }
}

[TaskCategory("Game")]
[TaskDescription("锁定最远的目标")]
public class DoSelectFarthest : Action
{
    public override TaskStatus OnUpdate()
    {
        XEntity e = AITreeImpleted.Transform2Entity(transform);
        return (TaskStatus)AITreeImpleted.DoSelectFarthestUpdate(e);
    }
}

[TaskCategory("Game")]
[TaskDescription("随机锁定目标")]
public class DoSelectRandomTarget : Action
{
    public override TaskStatus OnUpdate()
    {
        XEntity e = AITreeImpleted.Transform2Entity(transform);
        return (TaskStatus)AITreeImpleted.DoSelectRandomTargetUpdate(e);
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
        XEntity e = AITreeImpleted.Transform2Entity(transform);
        return (TaskStatus)AITreeImpleted.CalDistanceUpdate(e,mAIArgObject.Value,mAIArgDistance.Value,mAIArgDestPoint.Value);
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
        XEntity e = AITreeImpleted.Transform2Entity(transform);
        return (TaskStatus)AITreeImpleted.SelectMoveTargetByIdUpdate(e, mAIArgMoveTarget.Value, mAIArgObjectId);
    }
}
