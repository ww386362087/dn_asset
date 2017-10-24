using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace AI
{
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

}