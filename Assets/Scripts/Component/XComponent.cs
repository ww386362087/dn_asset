using UnityEngine;
using System.Collections;

public class XComponent : XObject
{

    public XObject obj = null;

    public virtual uint ID { get { return XCommon.singleton.XHash(this.GetType().Name); } }

    public virtual void OnInitial(XObject _obj)
    {
        base.Initilize();
        obj = _obj;
    }

    public virtual void OnUninit()
    {
        base.Unload();
    }



}
