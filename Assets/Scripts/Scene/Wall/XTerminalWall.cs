using UnityEngine;
using System.Collections;

public class XTerminalWall : XWall
{
    public string exString;
    private bool reported = false;

    protected override void OnTriggered()
    {
        if (!reported && exString != null && exString.Length > 0)
        {
            _interface.SetExternalString(exString, true);
            reported = true;
        }
    }
}
