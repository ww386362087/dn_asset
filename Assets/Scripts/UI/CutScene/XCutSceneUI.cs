using UnityEngine;
using UnityEngine.UI;

public class XCutSceneUI : UIDlg<XCutSceneUI, XCutSceneBehaviour>
{

    public override string fileName
    {
        get { return "UI/CutSceneUI"; }
    }

    public override DlgType type
    {
        get { return DlgType.Top; }
    }

    protected override void Regist()
    {
        base.Regist();
        RegistClick(uiBehaviour.skip.gameObject, OnSkipClick);
    }

    public void Initial()
    {
        SetVisible(true);
        SetText("");
        int far = UIManager._far_far_away;
        SetIntroPos(far, far);
    }

    public void SetText(string text)
    {
        uiBehaviour.text.text = text;
    }
    
    public void SetIntroText(bool enabled, string name)
    {
        if (!IsVisible()) return;
        if (enabled)
        {
            uiBehaviour.tname.text = name;
            if (!uiBehaviour.anim.isPlaying)
                uiBehaviour.anim.Play();
        }
    }
    
    public void SetIntroPos(float x, float y)
    {
        if (uiBehaviour.intro != null)
            uiBehaviour.intro.localPosition = new Vector3(x, y, 0);
    }


    private void OnSkipClick(GameObject go)
    {
        XScene.singleton.DetachCutScene();
    }

}
