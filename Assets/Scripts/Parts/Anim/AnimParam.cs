public class AnimParam
{
    public static string ToJump = "ToJump";
    public static string ToStand = "ToStand";
    public static string ToFall = "ToFall";
    public static string ToSkill = "ToSkill";
    public static string ToCharge = "ToCharge";
    public static string ToFreeze = "ToFreeze";
    public static string ToBeHit = "ToBeHit";
    public static string ToDeath = "ToDeath";
    public static string ToMove = "ToMove";
    public static string ToArtSkill = "ToArtSkill";
    public static string ToJA_0_1 = "ToJA_0_1";
    public static string ToJA_0_2 = "ToJA_0_2";
    public static string ToJA_1_0 = "ToJA_1_0";
    public static string ToJA_2_0 = "ToJA_2_0";
    public static string ToJA_3_0 = "ToJA_3_0";
    public static string ToJA_4_0 = "ToJA_4_0";
    public static string ToJA_1_1 = "ToJA_1_1";
    public static string ToJA_1_2 = "ToJA_1_2";
    public static string ToJA_2_1 = "ToJA_2_1";
    public static string ToJA_2_2 = "ToJA_2_2";
    public static string ToJA_3_1 = "ToJA_3_1";
    public static string ToJA_3_2 = "ToJA_3_2";
    public static string ToJA_4_1 = "ToJA_4_1";
    public static string ToJA_4_2 = "ToJA_4_2";
    public static string ToBeHit_Landing = "ToBeHit_Landing";
    public static string ToBeHit_Hard = "ToBeHit_Hard";
    public static string ToBeHit_GetUp = "ToBeHit_GetUp";
    public static string ToPhase = "ToPhase";
    public static string ToPhase1 = "ToPhase1";
    public static string ToPhase2 = "ToPhase2";
    public static string ToPhase3 = "ToPhase3";
    public static string ToPhase4 = "ToPhase4";
    public static string ToPhase5 = "ToPhase5";
    public static string ToPhase6 = "ToPhase6";
    public static string ToPhase7 = "ToPhase7";
    public static string ToPhase8 = "ToPhase8";
    public static string ToPhase9 = "ToPhase9";
    public static string ToJA_QTE = "ToJA_QTE";
    public static string ToMultipleDirAttack = "ToMultipleDirAttack";


    private string V;

    public AnimParam(string aa) { V = aa; }

    public static implicit operator string(AnimParam id)
    {
        return id.ToString();
    }

    public static implicit operator AnimParam(string id)
    {
        return new AnimParam(id);
    }

    public override string ToString()
    {
        return V;
    }


}
