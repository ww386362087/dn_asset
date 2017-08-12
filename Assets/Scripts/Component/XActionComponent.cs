using UnityEngine;

public class XActionComponent : XComponent
{

    XAnimComponent ani;

    public override void OnInitial(XEntity _entity)
    {
        base.OnInitial(_entity);
        ani = entity.GetComponent<XAnimComponent>();
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
        XJoyStickDirectionEvent move = e as XJoyStickDirectionEvent;
        ani.SetTrigger("ToMove");
        Vector3 mov = entity.speed * move.Direction;
        mov.y = 0;
        entity.ApplyMove(mov);
    }



}
