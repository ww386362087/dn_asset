
internal class XStateMgr
{
    public static bool IsMoveState(XStateDefine state)
    {
        return state == XStateDefine.XState_Move;
    }

    public static bool IsUnBattleState(XStateDefine state)
    {
        return state == XStateDefine.XState_Idle ||
               IsMoveState(state);
    }

    public static bool IsAirState(XStateDefine state)
    {
        return state == XStateDefine.XState_Fall ||
               state == XStateDefine.XState_Jump;
    }

    public static bool IsStunState(XStateDefine state)
    {
        return state == XStateDefine.XState_Freeze;
    }

    public static bool IsUnControlledState(XStateDefine state)
    {
        return IsStunState(state) ||
               state == XStateDefine.XState_BeHit ||
               state == XStateDefine.XState_Death;
    }

}
