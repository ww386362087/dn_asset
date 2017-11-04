
public class Clip
{
    public static string Idle = "Idle";
    public static string Death = "Death";
    public static string Fall = "Fall";
    public static string Freezed = "Freezed";
    public static string Walk = "Walk";
    public static string Run = "Run";
    public static string RunLeft = "RunLeft";
    public static string RunRight = "RunRight";
    public static string PresentStraight = "PresentStraight";
    public static string HitLanding = "HitLanding";
    public static string GetUp = "GetUp";
    public static string HardStraight = "HardStraight";
    public static string Art = "Art";
    public static string Forward = "Forward";
    public static string RightForward = "RightForward";
    public static string Right = "Right";
    public static string RightBack = "RightBack";
    public static string Back = "Back";
    public static string LeftBack = "LeftBack";
    public static string Left = "Left";
    public static string LeftForward = "LeftForward";
    public static string A = "A";
    public static string AA = "AA";
    public static string AAA = "AAA";
    public static string AAAA = "AAAA";
    public static string AAAAA = "AAAAA";
    public static string AB = "AB";
    public static string QTE = "QTE";
    public static string Phase0 = "Phase0";
    public static string Phase1 = "Phase1";
    public static string Phase2 = "Phase2";
    public static string Phase3 = "Phase3";
    public static string Phase4 = "Phase4";
    public static string Phase5 = "Phase5";
    public static string Phase6 = "Phase6";
    public static string Phase7 = "Phase7";
    public static string Phase8 = "Phase8";
    public static string Phase9 = "Phase9";


    private string V;

    public Clip(string aa) { V = aa; }

    public static implicit operator string(Clip id)
    {
        return id.ToString();
    }

    public static implicit operator Clip(string id)
    {
        return new Clip(id);
    }

    public override string ToString()
    {
        return V;
    }

}

