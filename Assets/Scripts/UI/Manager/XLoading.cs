using UnityEngine;
using UnityEngine.UI;

public sealed class XLoading  
{

    private static Image loadimg { get { return UIManager.singleton.LoadImage; } }

    private static Text loadtxt { get { return UIManager.singleton.LoadText; } }



    public static void SetText(string txt)
    {
        if (loadtxt != null)
        {
            loadtxt.text = txt;
        }
    }


    public static void SetImage(string name)
    {
        if(loadimg!=null)
        {
            Sprite spr = XResourceMgr.Load<Sprite>(name, AssetType.PNG);
            loadimg.sprite = spr;
        }
    }

    public static void OnLoadFinish(bool finish)
    {
        XAutoFade.MakeBlack(true);
        Show(false);
        XAutoFade.FadeIn(1);
    }


    public static void Show(bool show)
    {
        Color c = loadimg.color;
        c.a = show ? 1 : 0;
        loadimg.color = c;
        c = loadtxt.color;
        c.a = show ? 1 : 0;
        loadtxt.color = c;
        if (!show)
        {
            Object.Destroy(loadimg.sprite);
            loadimg.sprite = null;
        }
    }
    

}
