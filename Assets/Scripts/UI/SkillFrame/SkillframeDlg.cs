using UnityEngine;


public class SkillframeDlg : UIDlg<SkillframeDlg, SkillframeBehaviour>
{

    private XSkillComponent m_skill;
    const string skillid = "player_archer/Player_archer_attack_duoduansheji";


    public override DlgType type
    {
        get { return DlgType.Fixed; }
    }

    public override bool shareCanvas
    {
        get { return true; }
    }


    public override string fileName
    {
        get { return "UI/SkillFrame"; }
    }


    protected override void Regist()
    {
        base.Regist();
        RegistClick(uiBehaviour.m_btn0.gameObject, OnAttack);
    }

    public override void OnShow()
    {
        base.OnShow();
        uiBehaviour.rect.anchoredPosition = Vector3.zero;
    }


    public void OnAttack(GameObject go)
    {
        XPlayer player = XEntityMgr.singleton.Player;
        if (player != null)
        {
            m_skill = player.GetComponent<XSkillComponent>();
        }
        if (m_skill != null)
        {
            m_skill.CastSkill(skillid);
        }
    }

}
