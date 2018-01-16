技能系统：


点击一些常用的技能的按钮触发Art Skill
	不同的按钮触发的都是Animator->Art(State) Art分别会不同的Animation overide掉

点击普攻按钮（可以连续点击）触发JA Skill
	动画表现是Animator->Attack Layer-> Attack -> (JA01, JA02, JA10, JA20 ....) 连续点击在这几个状态之间切换

组合技能Combined Skill
	动画表现是Aniamtor->Attack Layer->Combined ->(Phase0, Phase1, Phase2, Phase3 ....)


小怪、Boss、NPC只有Art Skill 
只有Player才会有JA Skill, 当然也有Art Skill


技能相关配置说明：
1. Charge	受此技能人物实现向前冲
2. Manipulate	受此技能对手实现位移
3. MobUnits	此技能生成参照物
4. Fx 特效相关
5. Result和HitEffect 打击点相关配置
6. Warning 警告圈相关配置

