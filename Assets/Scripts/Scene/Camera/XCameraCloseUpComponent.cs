using UnityEngine;


class XCameraCloseUpComponent : XComponent
{
    private XCamera _camera = null;

    private float _pre_x = 0;
    private float _pre_y = 0;


    public override void OnInitial(XObject _obj)
    {
        base.OnInitial(_obj);
        _camera = _obj as XCamera;
    }


    public override void OnUninit()
    {
        base.OnUninit();
        _camera = null;
    }

    protected override void EventSubscribe()
    {
        base.EventSubscribe();
        RegisterEvent(XEventDefine.XEvent_Camera_CloseUp, OnCloseUp);
        RegisterEvent(XEventDefine.XEvent_Camera_CloseUpEnd, OnCloseUpEnd);
    }
    

    private void OnCloseUp(XEventArgs e)
    {
         XDebug.Log(_camera.CameraTrans);

        XCameraCloseUpEvent ev = e as XCameraCloseUpEvent;
        XEntity target = ev.Target;

        _pre_x = _camera.Root_R_X;
        _pre_y = _camera.Root_R_Y;

        Vector3 base_v = XCommon.singleton.Horizontal(target.Position - XEntityMgr.singleton.Player.Position);
        Vector3 rot_v = XCommon.singleton.HorizontalRotateVetor3(base_v, -45);
        XCameraActionEvent arg = new XCameraActionEvent();
        arg.To_Rot_X = _camera.Root_R_X;
        arg.To_Rot_Y = rot_v.y;
        XEventMgr.singleton.FireEvent(arg);
    }


    private void OnCloseUpEnd(XEventArgs e)
    {
        XCameraActionEvent arg = new XCameraActionEvent();
        arg.To_Rot_X = _pre_x;
        arg.To_Rot_Y = _pre_y;
        XEventMgr.singleton.FireEvent(arg);
    }


}
