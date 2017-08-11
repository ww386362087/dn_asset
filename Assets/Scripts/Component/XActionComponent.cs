using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XActionComponent : XComponent
{

    public override void OnInitial(XEntity _entity)
    {
        base.OnInitial(_entity);
    }

    public override void OnUninit()
    {
        base.OnUninit();
    }

    protected override void EventSubscribe()
    {
        base.EventSubscribe();
        RegisterEvent(XEventDefine.XEvent_Move, OnMove);
    }


    private void OnMove(XEventArgs e)
    {
        XMoveEventArgs move = e as XMoveEventArgs;
        Debug.Log("ai move:" + move.Speed);
    }
}
