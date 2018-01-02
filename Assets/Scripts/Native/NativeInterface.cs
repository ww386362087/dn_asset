using AOT;
using System.Runtime.InteropServices;
using UnityEngine;

class NativeInterface
{

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    static extern void iInitEntityCall(NativeEntityDelegate cb);

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    static extern void iInitCompnentCall(NativeComptDelegate cb);

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    static extern void iInitEntitySyncCall(NativeEntitySyncInfoDelegate cb);


    public delegate void NativeEntityDelegate(uint entityid, byte command, uint arg);
    public delegate void NativeEntitySyncInfoDelegate(uint entity, byte command,ref VectorArr vec);
    public delegate void NativeComptDelegate(uint entity, byte command, string arg);


    public static void InitNative()
    {
        iInitEntityCall(OnEntityCallback);
        iInitCompnentCall(OnComponentCallback);
        iInitEntitySyncCall(OnEntitySync);
    }


    [MonoPInvokeCallback(typeof(NativeEntityDelegate))]
    static void OnEntityCallback(uint entityid, byte command, uint arg)
    {
        XDebug.Log("entity " + entityid, " arg: ", arg, " command: ", command);
        switch (command)
        {
            case ASCII.E:
                NativeEntityMgr.singleton.Add<NativeEntity>(entityid, arg);
                break;
            case ASCII.R:
                NativeEntityMgr.singleton.Add<NativeRole>(entityid, arg);
                break;
            case ASCII.U:
                NativeEntityMgr.singleton.Remv(entityid);
                break;
        }
    }

    [MonoPInvokeCallback(typeof(NativeComptDelegate))]
    static void OnEntitySync(uint entityid, byte command,ref VectorArr vec)
    {
        XDebug.Log("entityid: ", entityid, " arg:", vec.ToVector());
        NativeEntity entity = NativeEntityMgr.singleton.Get(entityid);
        switch (command)
        {
            case ASCII.p:
                entity.transfrom.position = vec.ToVector();
                break;
            case ASCII.s:
                entity.transfrom.localScale = vec.ToVector();
                break;
            case ASCII.r:
                entity.transfrom.rotation = Quaternion.Euler(vec.ToVector());
                break;
            case ASCII.f:
                entity.transfrom.forward = vec.ToVector();
                break;
        }
    }

    [MonoPInvokeCallback(typeof(NativeComptDelegate))]
    static void OnComponentCallback(uint entityid, byte command, string arg)
    {
        NativeEntity entity = NativeEntityMgr.singleton.Get(entityid);
        switch (command)
        {
            case ASCII.C:
                {
                    NativeEquipComponent ne = entity.GetComponent<NativeEquipComponent>();
                    ne.ChangeHairColor(Color.red);
                }
                break;
            case ASCII.W:
                {
                    NativeEquipComponent ne = entity.GetComponent<NativeEquipComponent>();
                    ne.AttachWeapon(arg);
                }
                break;
        }
    }

 

}
