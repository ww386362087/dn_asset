using AI;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Game")]
[TaskDescription("当前状态是否为Idle")]
public class StatusIdle : Conditional
{
    public override TaskStatus OnUpdate()
    {
        XEntity e = AITreeImpleted.Transform2Entity(transform);
        return AITreeImpleted.StatusIdleUpdate(e);
    }
}

[TaskCategory("Game")]
[TaskDescription("当前状态是否为Move")]
public class StatusMove : Conditional
{
    public override TaskStatus OnUpdate()
    {
        XEntity e = AITreeImpleted.Transform2Entity(transform);
        return AITreeImpleted.StatusMoveUpdate(e);
    }
}

[TaskCategory("Game")]
[TaskDescription("当前状态是否为受击")]
public class StatusBehit : Conditional
{
    public override TaskStatus OnUpdate()
    {
        XEntity e = AITreeImpleted.Transform2Entity(transform);
        return AITreeImpleted.StatusBehitUpdate(e);
    }
}

[TaskCategory("Game")]
[TaskDescription("当前状态是否为死亡")]
public class StatusDeath : Conditional
{
    public override TaskStatus OnUpdate()
    {
        XEntity e = AITreeImpleted.Transform2Entity(transform);
        return AITreeImpleted.StatusDeathUpdate(e);
    }
}

[TaskCategory("Game")]
[TaskDescription("当前状态是否为锁定")]
public class StatusFreeze : Conditional
{
    public override TaskStatus OnUpdate()
    {
        XEntity e = AITreeImpleted.Transform2Entity(transform);
        return AITreeImpleted.StatusFreezeUpdate(e);
    }
}

[TaskCategory("Game")]
[TaskDescription("当前状态是否为cast技能")]
public class StatusSkill : Conditional
{
    public override TaskStatus OnUpdate()
    {
        XEntity e = AITreeImpleted.Transform2Entity(transform);
        return AITreeImpleted.StatusSkillUpdate(e);
    }
}

