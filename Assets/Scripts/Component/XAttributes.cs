using UnityEngine;

public class XAttributes : XComponent
{
    private uint _id = 0;
    private string _prefab_name = null;
    private string _name = null;
    private EnitityType _type = EnitityType.Entity_None;
    private Vector3 _appear_pos = Vector3.zero;
    private Quaternion _appear_qua = Quaternion.identity;
    private uint _presentID = 2;

    public uint id
    {
        get { return _id; }
        set { _id = value; }
    }

    public string Prefab
    {
        get { return _prefab_name; }
        set { _prefab_name = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public EnitityType Type
    {
        get { return _type; }
        set { _type = value; }
    }

    public Vector3 AppearPostion
    {
        get { return _appear_pos; }
        set { _appear_pos = value; }
    }


    public Quaternion AppearQuaternion
    {
        get { return _appear_qua; }
        set { _appear_qua = value; }
    }


    public uint PresentID
    {
        get { return _presentID; }
        set { _presentID = value; }
    }


    public double GetAttr(XAttributeDefine def)
    {
        return 0;
    }

}
