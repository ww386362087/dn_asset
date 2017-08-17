using UnityEngine;

public class XSpawnWall : XWall
{
    public string exString;
    public etrigger_type TriggerType;

    public enum etrigger_type
    {
        once,
        always
    }

    protected override void OnTriggered()
    {
        if (exString != null && exString.Length > 0)
        {
            _interface.SetExternalString(exString, (TriggerType == etrigger_type.once));
        }
    }
}
