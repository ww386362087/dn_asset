
namespace AI
{
    public class AITreeArg
    {
        public static string HeartRate = "heartrate";

        public static string NavTarget = "navtarget";

        public static string TARGET = "target";

        public static string MASTER = "master";

        public static string IsOppoCastingSkill = "is_oppo_casting_skill";

        public static string IsHurtOppo = "is_hurt_oppo";

        public static string TargetDistance = "target_distance";

        public static string MasterDistance = "master_distance";

        public static string IsFixedInCD = "is_fixed_in_cd";

        public static string NormalAttackProb = "normal_attack_prob";

        public static string EnterFightRange = "enter_fight_range";

        public static string FightTogetherDis = "fight_together_dis";

        public static string MaxHP = "max_hp";

        public static string CurrHP = "current_hp";

        public static string MaxSupperArmor = "max_super_armor";

        public static string CurrSuperArmor = "current_super_armor";

        public static string EntityType = "entity_type";

        public static string TargetRot = "target_rotation";

        public static string AttackRange = "attack_range";

        public static string MinKeepRange = "min_keep_range";

        public static string IsCastingSkill = "is_casting_skill";

        public static string IsFighting = "is_fighting";

        public static string IsQteState = "is_qte_state";
        
        public static string BornPos = "bornpos";

        public static string SkillID = "skillid";

        private string V;

        public AITreeArg(string aa) { V = aa; }

        public static implicit operator string(AITreeArg id)
        {
            return id.ToString();
        }

        public static implicit operator AITreeArg(string id)
        {
            return new AITreeArg(id);
        }

        public override string ToString()
        {
            return V;
        }


        public override bool Equals(object obj)
        {
            return V.Equals(obj.ToString());
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
