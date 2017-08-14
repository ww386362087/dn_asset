using UnityEngine;
using System.Collections;

public class XAIComponent : XComponent
{
    private bool _is_fighting = false;
    private XRole _opponent = null;
   // private float _ai_tick = 1.0f;  //AI心跳间隔 

    public bool IsFighting { get { return _is_fighting; } set { _is_fighting = value; } }
    public XRole Opponent { get { return _opponent; } set { _opponent = value; } }
   

    public override void OnInitial(XObject _obj)
    {
        base.OnInitial(_obj);
    }
    
    public override void OnUninit()
    {
        base.OnUninit();
    }


    
    
}
