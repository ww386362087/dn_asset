using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace AI
{
    [TaskCategory("Game")]
    public class ValueHP : Conditional
    {
        public int mAIArgMaxHP;
        public int mAIArgMinHP;

        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.ValueHPUpdate(e, mAIArgMaxHP, mAIArgMinHP);
        }
    }

    [TaskCategory("Game")]
    public class ValueMP : Conditional
    {
        public int mAIArgMaxMP;
        public int mAIArgMinMP;

        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.ValueMPUpdate(e, mAIArgMaxMP, mAIArgMinMP);
        }
    }

    [TaskCategory("Game")]
    public class ValueTarget : Conditional
    {
        public SharedTransform mAIArgTarget;

        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.ValueTargetUpdate(e, mAIArgTarget.Value);
        }
    }

    [TaskCategory("Game")]
    [TaskDescription("距离目标小于固定值")]
    public class ValueDistance : Conditional
    {
        public SharedGameObject mAIArgTarget;
        public SharedFloat mAIArgMaxDistance;

        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.ValueDistanceUpdate(e, mAIArgTarget.Value, mAIArgMaxDistance.Value);
        }
    }

    [TaskCategory("Game")]
    public class IsOppoCastingSkill : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.IsOppoCastingSkillUpdate(e);
        }
    }

    [TaskCategory("Game")]
    public class IsHurtOppo : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.IsHurtOppoUpdate(e);
        }
    }

    [TaskCategory("Game")]
    public class IsFixedInCd : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.IsFixedInCdUpdate(e);
        }
    }


    [TaskCategory("Game")]
    public class IsCastingSkill : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.IsCastingSkillUpdate(e);
        }
    }

}