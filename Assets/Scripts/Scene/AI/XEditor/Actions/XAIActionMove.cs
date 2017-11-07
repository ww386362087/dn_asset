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
    [TaskDescription("方向对准目标 允许偏移")]
    public class RotateToTarget : Action
    {
        public SharedGameObject mAIArgTarget;
        public SharedFloat mAIArgAngle;

        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.RotateToTargetUpdate(e, mAIArgTarget.Value, mAIArgAngle.Value);
        }
    }

    [TaskCategory("Game")]
    [TaskDescription("自身偏移")]
    public class RotateSelf : Action
    {
        public float mAIArgMax;
        public float mAIArgMin;

        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.RotateSelfUpdate(e, mAIArgMax,mAIArgMin);
        }
    }


    [TaskCategory("Game")]
    [TaskDescription("向正前方位移")]
    public class MoveForward : Action
    {
        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.MoveForwardUpdate(e);
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