using UnityEngine;

public class SkillframeBehaviour : DlgBehaviourBase
{
    public override void OnInitial()
    {
        base.OnInitial();
        m_btn0 = transform.Find("btn0");
    }

    public Transform m_btn0;

}
