using UnityEngine;
using XTable;

public class XAttributes : XComponent
{
    private uint _id = 0;
    private string _prefab_name = null;
    private string _name = null;
    private Vector3 _appear_pos = Vector3.zero;
    private Quaternion _appear_qua = Quaternion.identity;
    private uint _presentID = 2;
    private bool _is_dead = false;

    public uint id
    {
        get { return _id; }
        set { _id = value; }
    }

    public string Prefab
    {
        get { return _prefab_name; }
        set { _prefab_name = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }


    public Vector3 AppearPostion
    {
        get { return _appear_pos; }
        set { _appear_pos = value; }
    }


    public Quaternion AppearQuaternion
    {
        get { return _appear_qua; }
        set { _appear_qua = value; }
    }


    public uint PresentID
    {
        get { return _presentID; }
        set { _presentID = value; }
    }

    public bool IsDead
    {
        get { return _is_dead; }
        set { _is_dead = value; }
    }
    public bool Blocked { get; set; }
    public bool IsWander { get; set; }
    public bool IsFixedInCD { get; set; }
    public bool Outline { get; set; }
    public int UseMyMesh { get; set; }
    public int SummonGroup { get; set; }
    public bool EndShow { get; set; }
    public bool GeneralCutScene { get; set; }
    public bool SameBillBoardByMaster { get; set; }
    public float NormalAttackProb { get; set; }
    public float EnterFightRange { get; set; }
    public float FloatingMax { get; set; }
    public float FloatingMin { get; set; }
    public float AIStartTime { get; set; }
    public float AIActionGap { get; set; }
    public string AiBehavior { get; set; }
    public float FightTogetherDis { get; set; }
    public int AiHit { get; set; }
    public double GetAttr(XAttributeDefine def)
    {
        return 0;
    }

    // 从本地读出来的配置数据初始化
    public void InitAttribute(XEntityStatistics.RowData data)
    {
        NormalAttackProb = data.AttackProb;
        EnterFightRange = data.Sight;
        Blocked = data.Block;
        AIStartTime = data.AIStartTime;
        AIActionGap = data.AIActionGap;
        IsFixedInCD = data.IsFixedInCD;
        Outline = data.Highlight;
        FloatingMin = data.FloatHeight != null && data.FloatHeight.Length > 0 ? data.FloatHeight[0] : 0;
        FloatingMax = data.FloatHeight != null && data.FloatHeight.Length > 0 ? data.FloatHeight[1] : 0;
        EndShow = data.EndShow;
        GeneralCutScene = data.UsingGeneralCutscene;
        FightTogetherDis = data.FightTogetherDis;
        AiBehavior = data.AiBehavior;
        AiHit = data.aihit;
    }

}
