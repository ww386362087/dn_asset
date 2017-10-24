using UnityEngine;

public class XAttributes : XComponent
{
    private uint _id = 0;
    private string _prefab_name = null;
    private string _name = null;
    private Vector3 _appear_pos = Vector3.zero;
    private Quaternion _appear_qua = Quaternion.identity;
    private uint _presentID = 2;
    private bool _is_dead = false;

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

    public bool IsDead
    {
        get { return _is_dead; }
        set { _is_dead = value; }
    }

    public double GetAttr(XAttributeDefine def)
    {
        return 0;
    }

}
