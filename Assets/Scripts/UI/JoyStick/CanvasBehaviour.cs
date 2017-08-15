using UnityEngine.UI;

public class CanvasBehaviour : DlgBehaviourBase
{

    public override void OnInitial()
    {
        base.OnInitial();

        m_sprBg = transform.FindChild("spr").GetComponent<Image>();
    }

    
    public Image m_sprBg;
}
