using UnityEngine;
using UnityEngine.UI;

public class XCutSceneUI : XSingleton<XCutSceneUI>
{

    private Text m_text;
    private Text m_skip;
    private Text m_name;
    private Transform m_intro;
    private Animation m_anim;
    public GameObject _objUI;

    public override bool Init()
    {
        _objUI = XResourceMgr.Load<GameObject>("UI/CutSceneUI", AssetType.Prefab);
        _objUI.transform.SetParent(UIManager.singleton.UiCamera.transform);
        Canvas cans = _objUI.GetComponent<Canvas>();
        if (cans != null) cans.worldCamera = UIManager.singleton.UiCamera;
        _objUI.transform.localPosition = Vector3.zero;
        _objUI.transform.localRotation = Quaternion.identity;
        _objUI.transform.localScale = Vector3.one;

        m_intro = _objUI.transform.Find("Intro");
        m_anim = _objUI.GetComponent<Animation>();
        m_text = _objUI.transform.FindChild("DownBG/Text").GetComponent<Text>();
        m_name = _objUI.transform.FindChild("Intro/Name").GetComponent<Text>();

        m_text.text = "";
        return base.Init();
    }


    public void SetText(string text)
    {
        m_text.text = text;
    }

    public void SetVisible(bool visible)
    {
        _objUI.SetActive(visible);
    }

    public void SetIntroText(bool enabled, string name)
    {
        if (!_objUI.activeInHierarchy) return;
        if (enabled)
        {
            m_name.text = name;
            if (!m_anim.isPlaying)
                m_anim.Play();
        }
    }
    
    public void SetIntroPos(float x, float y)
    {
        if (m_intro != null)
            m_intro.localPosition = new Vector3(x, y, 0);
    }

}
