using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class JoyyouStickBahaviour : DlgBehaviourBase
{

    public override void OnInitial()
    {
        base.OnInitial();

        m_sprBg = transform.FindChild("bg").GetComponent<Image>();
        m_sprSir = transform.FindChild("bg/sir").GetComponent<Image>();
    }


    public Image m_sprSir;
    public Image m_sprBg;
}
