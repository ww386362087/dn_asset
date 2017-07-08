using UnityEngine;
using System.Collections;

public class XAudioComponent : XComponent
{
    readonly uint uuID = XCommon.singleton.XHash("XAudioComponent");

    public override uint ID { get { return uuID; } }


    public override void OnInitial(XEntity _entity)
    {
        base.OnInitial(_entity);
    }

    public override void OnUninit()
    {
        base.OnUninit();
    }
}
