using UnityEngine;


public class PlayerAction : IXPlayerAction
{
    public void CameraWallEnter(AnimationCurve curve, Vector3 intersection, Vector3 cornerdir, float sector, float inDegree, float outDegree, bool positive)
    {

    }
    public void CameraWallExit(float angle)
    {

    }

    public void CameraWallVertical(float angle)
    {

    }

    public void SetExternalString(string exString, bool bOnce)
    {

    }


    public void TransferToSceneLocation(Vector3 pos, Vector3 forward)
    {

    }


    public void TransferToNewScene(uint sceneID)
    {

    }

    public void PlayCutScene(string cutscene)
    {

    }

    public void GotoBattle()
    {

    }

    public void GotoTerritoryBattle(int index)
    {

    }

    public void GotoNest()
    {

    }

    public void GotoFishing(int seatIndex, bool bFishing)
    {

    }

    public bool IsValid { get { return true; } }

    public Vector3 PlayerPosition(bool notplayertrigger)
    {
        if (XEntityMgr.singleton.Player != null)
            return XEntityMgr.singleton.Player.EntityObject.transform.position;
        return Vector3.zero;
    }

    public Vector3 PlayerLastPosition(bool notplayertrigger)
    {
        if (XEntityMgr.singleton.Player != null)
            return XEntityMgr.singleton.Player.EntityObject.transform.position;
        return Vector3.zero;
    }

    public void RefreshPosition()
    {

    }
}