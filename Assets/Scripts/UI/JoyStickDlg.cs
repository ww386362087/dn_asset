using System.Collections.Generic;
using UnityEngine;

public class JoyStickDlg : UIDlg<JoyStickDlg, JoyyouStickBahaviour>
{

    
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
            uiBehaviour.transform.localPosition = screenpos;
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


    private void SetOffsetPos(Vector3 pos)
    {
        uiBehaviour.m_sprSir.transform.localPosition = pos;
    }

}
