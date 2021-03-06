﻿using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


namespace AI
{
    [TaskCategory("Game")]
    [TaskDescription("释放技能")]
    public class XAIActionSkill : Action
    {
        public string mAIArgSkillScript;
        public SharedGameObject mAIArgTarget;


        public override TaskStatus OnUpdate()
        {
            XEntity e = AITreeImpleted.Transform2Entity(transform);
            return (TaskStatus)AITreeImpleted.XAIActionSkillUpdate(e, mAIArgSkillScript, mAIArgTarget.Value);
        }
    }

}