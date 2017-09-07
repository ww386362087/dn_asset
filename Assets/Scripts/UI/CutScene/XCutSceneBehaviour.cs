using UnityEngine;
using UnityEngine.UI;

public class XCutSceneBehaviour : DlgBehaviourBase
{
    public override void OnInitial()
    {
        base.OnInitial();
        intro = transform.Find("Intro");
        anim = GetComponent<Animation>();
        text = transform.FindChild("DownBG/Text").GetComponent<Text>();
        tname = transform.FindChild("Intro/Name").GetComponent<Text>();
        skip = transform.FindChild("UpBG/Skip");
    }


    public Text text;
    public Transform skip;
    public Text tname;
    public Transform intro;
    public Animation anim;

}
