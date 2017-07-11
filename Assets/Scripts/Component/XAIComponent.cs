using UnityEngine;
using System.Collections;

public class XAIComponent : XComponent
{
    private bool _is_fighting = false;
    private XRole _opponent = null;
    private float _ai_tick = 1.0f;  //AI心跳间隔 

    public bool IsFighting { get { return _is_fighting; } set { _is_fighting = value; } }
    public XRole Opponent { get { return _opponent; } set { _opponent = value; } }
   

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
        Debug.Log("ai move:" +move.Speed);
    }
}
