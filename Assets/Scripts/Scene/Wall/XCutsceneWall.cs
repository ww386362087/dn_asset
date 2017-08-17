using UnityEngine;

public class XCutsceneWall : XWall
{
    public string CutScene;
    private bool _played = false;

    protected override void OnTriggered()
    {
        if (_played) return;

        _interface.PlayCutScene(CutScene);
        _played = true;
    }
}
