using System.Text;
using UnityEngine;
using XTable;

public class XNPC : XEntity
{
    protected override EnitityType _eEntity_Type
    {
        get { return EnitityType.Entity_Npc; }
    }

    protected XNpcList.RowData row = null;

    public static int NpcLayer = LayerMask.NameToLayer("Npc");

    private Transform _head = null;

    public override void OnInitial()
    {
        base.OnInitial();
        _layer = LayerMask.NameToLayer("Npc");
        FindHead();
        
        XAnimComponent anim = AttachComponent<XAnimComponent>();
        anim.OverrideAnim("Idle", _present.AttackIdle);
        anim.SetTrigger("ToStand");
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
            //otherwise try to remove '/Bip001 Spine1'
            _head_path.Replace("/Bip001 Spine1" + subffix, "/");
            _head = EntityTransfer.Find(_head_path.ToString(0, _head_path.Length - 1));
        }
    }


    public override void Update(float delta)
    {
        base.Update(delta);
        if (_head != null)
        {
            _head.transform.localEulerAngles = EntityTransfer.forward;
        }
    }

}
