using UnityEngine;

public class XTransferWall : XWall
{
    public enum transfer_type
    {
        current_scene,
        other_scene,
    }

    public transfer_type targetScene;
    public int sceneID;
    public GameObject targetPos;

    protected override void OnTriggered()
    {
        if (targetScene == transfer_type.current_scene)
        {
            if (targetPos != null)
            {
                _interface.TransferToSceneLocation(targetPos.transform.position, targetPos.transform.forward);
            }
        }
        else if (targetScene == transfer_type.other_scene)
        {
            if (sceneID > 0)
            {
                _interface.TransferToNewScene((uint)sceneID);
            }
        }
    }
}
