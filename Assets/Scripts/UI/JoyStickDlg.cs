using System.Collections.Generic;
using UnityEngine;

public class JoyStickDlg : UIDlg<JoyStickDlg, JoyyouStickBahaviour>
{

    private Vector3 dir_pos = Vector3.zero;

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
        get { return "UI/JoyStick"; }
    }


    public override void OnShow()
    {
        base.OnShow();
    }

    public void Show(bool show, Vector2 screenpos)
    {
        
        if (show)
        {
            if (!IsVisible()) SetVisible(true);
            uiBehaviour.rect.anchoredPosition = screenpos;
        }
        else
        {
            Hide();
        }
    }


    public void Hide()
    {
        if (!IsLoaded()) return;
        uiBehaviour.transform.localPosition = UIManager.Far_Far_Away;
    }


    private void SetMainPos(Vector3 pos)
    {
        uiBehaviour.m_sprSir.transform.localPosition = pos;
    }


    public void SetOffsetPos(float radius,float angle)
    {
        float max_radius = GetMaxRadius();
        float r = (radius > max_radius) ? max_radius : radius;
        angle = angle / 180 * Mathf.PI;
        dir_pos.x = Mathf.Cos(angle) * r;
        dir_pos.y = -Mathf.Sin(angle) * r;
        uiBehaviour.m_sprSir.rectTransform.anchoredPosition =  dir_pos;
    }


    private float GetMaxRadius()
    {
        if(IsVisible())
        {
            float w1 = uiBehaviour.m_sprBg.preferredWidth / 2;
            float w2 = uiBehaviour.m_sprSir.preferredWidth / 2;
            return w1 + w2;
        }
        return 100;
    }
}
