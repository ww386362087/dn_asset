using BehaviorDesigner.Runtime.Tasks;

namespace AI
{
    [TaskCategory("Game")]
    [TaskDescription("当前状态是否为Idle")]
    public class StatusIdle : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.StatusIdleUpdate(e);
        }
    }

    [TaskCategory("Game")]
    [TaskDescription("当前状态是否为Move")]
    public class StatusMove : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.StatusMoveUpdate(e);
        }
    }

    [TaskCategory("Game")]
    [TaskDescription("当前状态是否为受击")]
    public class StatusBehit : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.StatusBehitUpdate(e);
        }
    }

    [TaskCategory("Game")]
    [TaskDescription("当前状态是否为死亡")]
    public class StatusDeath : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.StatusDeathUpdate(e);
        }
    }

    [TaskCategory("Game")]
    [TaskDescription("当前状态是否为锁定")]
    public class StatusFreeze : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.StatusFreezeUpdate(e);
        }
    }

    [TaskCategory("Game")]
    [TaskDescription("当前状态是否为cast技能")]
    public class StatusSkill : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.StatusSkillUpdate(e);
        }
    }

    [TaskCategory("Game")]
    [TaskDescription("随机值 触发概率")]
    public class RandomCompare : Conditional
    {
        public int mAIArgProb;

        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.RandomCompareUpdate(e, mAIArgProb);
        }
    }

}