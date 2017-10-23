using Level;
using System.Collections;
using XTable;

public class XSceneLoader
{
    private float _progress = 0;
    private float _sub_progress = 0;
    
    private System.Action _loaded = null;

    public void Load(System.Action loadComplete)
    {
        _loaded = loadComplete;
        GameEnine.entrance.StartCoroutine(InnerLoad());
    }

    private IEnumerator InnerLoad()
    {
        IEnumerator ietr = PreLoadMonster();
        while (ietr.MoveNext())
        {
            _progress = 0.05f + _sub_progress * 0.75f;
            yield return null;
        }
        ietr = PreLoadNPC();
        while (ietr.MoveNext())
        {
            _progress = 0.80f + _sub_progress * 0.10f;
            yield return null;
        }
        if (_loaded != null) _loaded();
    }

    private IEnumerator PreLoadMonster()
    {
        XLevelSpawnInfo spawner = XLevelSpawnMgr.singleton.currSpawn;
        if (spawner != null)
        {
            _sub_progress = 0;
            float per = 1f / spawner.preloadInfo.Count;
            foreach (var item in spawner.preloadInfo)
            {
                var entity = XTableMgr.GetTable<XEntityStatistics>().GetByID(item.Key);
                if (entity == null) continue;
                var pres = XTableMgr.GetTable<XEntityPresentation>().GetItemID(entity.PresentID);
                if (pres == null) continue;
                XResources.CreateInAdvance("Prefabs/" + pres.Prefab, item.Value);
                _sub_progress += per;
                yield return null;
            }
        }
    }


    private IEnumerator PreLoadNPC()
    {
        yield break;
    }

    private void CreatePlayer()
    {
        XEntityMgr.singleton.CreatePlayer();
        XPlayer player = XEntityMgr.singleton.Player;
        player.EnableCC(true);
    }


    private void DisplayProgress()
    {
        XDebug.Log(string.Format("当前加载进度：{0}/100 ", _progress));
    }

}
