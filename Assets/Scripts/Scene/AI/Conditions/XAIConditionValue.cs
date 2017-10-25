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
            uint id = uint.Parse(transform.name);
            XEntity entity = XEntityMgr.singleton.GetEntity(id);
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;
            double hp = entity.Attributes.GetAttr(XAttributeDefine.XAttr_CurrentHP_Basic);
            if (hp >= mAIArgMinHP && hp <= mAIArgMaxHP)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }

    [TaskCategory("Game")]
    public class ValueMP : Conditional
    {
        public int mAIArgMaxMP;
        public int mAIArgMinMP;

        public override TaskStatus OnUpdate()
        {
            uint id = uint.Parse(transform.name);
            XEntity entity = XEntityMgr.singleton.GetEntity(id);
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;
            double hp = entity.Attributes.GetAttr(XAttributeDefine.XAttr_CurrentMP_Basic);
            if (hp >= mAIArgMinMP && hp <= mAIArgMaxMP)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }

    [TaskCategory("Game")]
    public class ValueTarget : Conditional
    {
        public SharedTransform mAIArgTarget;

        public override TaskStatus OnUpdate()
        {
            if (XEntity.Valide(mAIArgTarget.Value))
                return TaskStatus.Success;
            else
            {
                uint id = uint.Parse(transform.name);
                XEntity entity = XEntityMgr.singleton.GetEntity(id);
                if (XEntity.Valide(entity))
                {
                    entity.GetComponent<XAIComponent>().SetTarget(null);
                }
                return TaskStatus.Failure;
            }
        }
    }

    [TaskCategory("Game")]
    public class ValueDistance : Conditional
    {
        public SharedTransform mAIArgTarget;
        public SharedFloat mAIArgMaxDistance;

        public override TaskStatus OnUpdate()
        {
            if (mAIArgTarget.Value == null)
                return TaskStatus.Failure;

            if ((transform.position - mAIArgTarget.Value.position).magnitude <= mAIArgMaxDistance.Value)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }

    [TaskCategory("Game")]
    public class IsOppoCastingSkill : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            uint id = uint.Parse(transform.name);
            XEntity entity = XEntityMgr.singleton.GetEntity(id);
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;
            if (entity.GetComponent<XAIComponent>().IsOppoCastingSkill)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }

    [TaskCategory("Game")]
    public class IsHurtOppo : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            uint id = uint.Parse(transform.name);
            XEntity entity = XEntityMgr.singleton.GetEntity(id);
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;
            if (entity.GetComponent<XAIComponent>().IsHurtOppo)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }

    [TaskCategory("Game")]
    public class IsFixedInCd : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            uint id = uint.Parse(transform.name);
            XEntity entity = XEntityMgr.singleton.GetEntity(id);
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;
            if (entity.GetComponent<XAIComponent>().IsFixedInCd)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}

[TaskCategory("Game")]
public class IsCastingSkill : Conditional
{
    public override TaskStatus OnUpdate()
    {
        uint id = uint.Parse(transform.name);
        XEntity entity = XEntityMgr.singleton.GetEntity(id);
        if (!XEntity.Valide(entity)) return TaskStatus.Failure;
        if (entity.GetComponent<XAIComponent>().IsCastingSkill)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}