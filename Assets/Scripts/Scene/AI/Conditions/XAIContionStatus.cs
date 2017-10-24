using BehaviorDesigner.Runtime.Tasks;

public class StatusIdle : Conditional
{
    public override TaskStatus OnUpdate()
    {
        uint id = uint.Parse(transform.name);
        XEntity entity = XEntityMgr.singleton.GetEntity(id);
        if (!XEntity.Valide(entity)) return TaskStatus.Failure;

        if (entity.CurState == XStateDefine.XState_Idle)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class StatusMove : Conditional
{
    public override TaskStatus OnUpdate()
    {
        uint id = uint.Parse(transform.name);
        XEntity entity = XEntityMgr.singleton.GetEntity(id);
        if (!XEntity.Valide(entity)) return TaskStatus.Failure;
        if (entity.CurState == XStateDefine.XState_Move)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}


public class StatusBehit : Conditional
{
    public override TaskStatus OnUpdate()
    {
        uint id = uint.Parse(transform.name);
        XEntity entity = XEntityMgr.singleton.GetEntity(id);
        if (!XEntity.Valide(entity)) return TaskStatus.Failure;
        if (entity.CurState == XStateDefine.XState_BeHit)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}


public class StatusDeath : Conditional
{
    public override TaskStatus OnUpdate()
    {
        uint id = uint.Parse(transform.name);
        XEntity entity = XEntityMgr.singleton.GetEntity(id);
        if (!XEntity.Valide(entity)) return TaskStatus.Failure;
        if (entity.CurState == XStateDefine.XState_Death)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

public class StatusFreeze : Conditional
{
    public override TaskStatus OnUpdate()
    {
        uint id = uint.Parse(transform.name);
        XEntity entity = XEntityMgr.singleton.GetEntity(id);
        if (!XEntity.Valide(entity)) return TaskStatus.Failure;
        if (entity.CurState == XStateDefine.XState_Freeze)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}


public class StatusSkill : Conditional
{
    public override TaskStatus OnUpdate()
    {
        uint id = uint.Parse(transform.name);
        XEntity entity = XEntityMgr.singleton.GetEntity(id);
        if (!XEntity.Valide(entity)) return TaskStatus.Failure;
        if (entity.CurState == XStateDefine.XState_Skill)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}

