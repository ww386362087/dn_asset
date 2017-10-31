using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace AI
{
    [TaskCategory("Game")]
    [TaskDescription("寻路去锁定目标")]
    public class NavToTarget : Action
    {
        public SharedGameObject mAIArgTarget;
        public SharedGameObject mAIArgNavTarget;
        public SharedVector3 mAIArgNavPos;

        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.NavToTargetUpdate(e, mAIArgTarget.Value, mAIArgNavTarget.Value, mAIArgNavPos.Value);
        }
    }

    [TaskCategory("Game")]
    [TaskDescription("停止寻路")]
    public class StopNavMove : Action
    {
        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.StopNavMoveUpdate(e);
        }
    }

    [TaskCategory("Game")]
    [TaskDescription("方向对准目标")]
    public class RotateToTarget : Action
    {
        public SharedGameObject mAIArgTarget;

        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.RotateToTargetUpdate(e, mAIArgTarget.Value);
        }
    }

    [TaskCategory("Game")]
    public class DetectEnemyInSight : Action
    {
        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.DetectEnemyInSightUpdate(e);
        }
    }
}