#if TEST

using UnityEngine;

public class TestScene : ITest
{
   
    const int sceneid = 401;
    XEntity npc = null;

    public void Start()
    {
        XScene.singleton.Enter(sceneid);
    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 120, 40), "Nav"))
        {
            XPlayer player = XEntityMgr.singleton.Player;
            if (npc == null)
            {
                var hashset = XEntityMgr.singleton._hash_entitys;
                var e = hashset.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.IsNpc)
                    {
                        npc = e.Current;
                        break;
                    }
                }
            }
            player.Navigate(npc.Position);
        }
        if (GUI.Button(new Rect(20, 80, 120, 40), "Path"))
        {
            XEntityMgr.singleton.Player.DrawNavPath();
        }
    }


    public void Update() { }

    public void LateUpdate()
    {
    }
}



#endif