using UnityEngine;

[ExecuteInEditMode]
public class XCameraWall : XWall
{
    public XCameraWall Associated = null;
    public XCurve Curve = null;

    public bool BeginWith = false;
    public float Angle = 0;
    public bool VerticalOnly = false;
    public float VerticalShiftAngle = 0;


    protected override void OnTriggered()
    {
        float sector = Vector3.Angle(transform.forward, Associated.transform.forward);
        Vector3 rotate = transform.rotation.eulerAngles;
        rotate.y = 0;
        Vector3 dir = Quaternion.AngleAxis(rotate.z, transform.forward) * XCommon.singleton.HorizontalRotateVetor3(transform.forward, 90, true);

        if (_forward_collision)
        {
            if (!VerticalOnly)
            {
                if (BeginWith)
                {
                    _interface.CameraWallEnter(Curve.Curve, transform.parent.transform.position, dir, sector, Angle, Associated.Angle, BeginWith);
                }
                else
                    _interface.CameraWallExit(Angle);
            }

            _interface.CameraWallVertical(VerticalShiftAngle);
        }
        else
        {
            if (!VerticalOnly)
            {
                if (BeginWith)
                    _interface.CameraWallExit(Angle);
                else
                {
                    _interface.CameraWallEnter(Curve.Curve, transform.parent.transform.position, dir, sector, Angle, Associated.Angle, BeginWith);
                }
            }

            _interface.CameraWallVertical(-VerticalShiftAngle);
        }
    }
}
