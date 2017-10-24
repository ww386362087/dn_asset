using System.Text;
using UnityEngine;
using XTable;

public class XNPC : XEntity
{
    protected XNpcList.RowData row = null;

    public static int NpcLayer = LayerMask.NameToLayer("Npc");
    private int _uGazing = 0;
    private Transform _head = null;
    private Vector3 _head_rotate = Vector3.forward;
    private XEntity _target = null;

    public override void OnInitial()
    {
        base.OnInitial();
        _layer = LayerMask.NameToLayer("Npc");
        _eEntity_Type |= EntityType.Npc;
        _target = XEntityMgr.singleton.Player;
        _head_rotate = EntityTransfer.forward;
        _uGazing = XTableMgr.GetTable<XNpcList>().GetByUID((int)_attr.id).Gazing;
        FindHead();
        XAnimComponent anim = AttachComponent<XAnimComponent>();
        anim.OverrideAnim("NPC_sidel_idle", _present.AnimLocation + _present.Idle);
        EnableShadow(true);
    }



    private void FindHead()
    {
        StringBuilder _head_path =
             new StringBuilder("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head", 128);

        string subffix = _present.BoneSuffix + "/";
        _head_path.Replace("/", subffix);
        _head_path.Append(subffix);

        _head = EntityTransfer.Find(_head_path.ToString(0, _head_path.Length - 1));

        if (_head == null)
        {
            _head_path.Replace("/Bip001 Spine1" + subffix, "/");
            _head = EntityTransfer.Find(_head_path.ToString(0, _head_path.Length - 1));
        }
    }

    public override void OnLateUpdate()
    {
        base.OnLateUpdate();
        if (_head != null && _target != null)
        {
            Vector3 v = _target.Position - Position;
            float d = v.magnitude;
            XCommon.singleton.Horizontal(ref v);

            if (d < 10)
            {
                Vector3 forward = EntityObject.transform.forward;
                if (Vector3.Angle(v, forward) > _uGazing)
                {
                    v = XCommon.singleton.HorizontalRotateVetor3(forward, XCommon.singleton.Clockwise(forward, v) ? _uGazing : -_uGazing);
                }
            }
            else
            {
                v = EntityTransfer.forward;
            }

            _head_rotate += (v - _head_rotate) * Mathf.Min(1.0f, 3 * Time.deltaTime);
            float y = XCommon.singleton.AngleToFloat(_head_rotate);
            y -= Rotation.eulerAngles.y;

            Vector3 rotate = _head.localEulerAngles;

            rotate.x += (-y);
            rotate.x %= 360;
            if (rotate.x > 180) rotate.x -= 360;
            if (rotate.x < -180) rotate.x += 360;

            if (Mathf.Abs(rotate.x) > _uGazing)
            {
                rotate.x = rotate.x > 0 ? _uGazing : -_uGazing;
            }
            _head.localEulerAngles = rotate;

        }
    }

    private void EnableShadow(bool able)
    {
        if (skin == null)
        {
            skin = EntityTransfer.GetComponentInChildren<SkinnedMeshRenderer>();
            skin.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }

}
