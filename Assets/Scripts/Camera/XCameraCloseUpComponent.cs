using UnityEngine;


class XCameraCloseUpComponent : XComponent
{
    private XCamera _camera = null;


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
        Debug.Log(_camera.CameraTrans);
    }


    private void OnCloseUpEnd(XEventArgs e)
    {

        Debug.Log("OnCloseUpEnd");
    }


}
