using AOT;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public delegate void CppDelegate(byte type, IntPtr p);
public delegate void NativeEntityDelegate(uint entityid, byte command, uint arg);
public delegate void NativeEntitySyncInfoDelegate(uint entity, byte command, ref VectorArr vec);
public delegate void NativeComptDelegate(uint entity, byte command, string arg);

public class NativeDef
{

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iInitCallbackCommand(CppDelegate cb);

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iInitial(string stream, string persist);


#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern int iAdd(int x, int y);


#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iJson(String file);


#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern int iSub(IntPtr x, IntPtr y);


#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iPatch(string oldf, string diff, string newf);

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iStartCore();

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iStopCore();

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iTickCore(float delta);

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iQuitCore();

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iGoInfo(string name, byte command, ref VectorArr vec);

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


    public static void Init()
    {
        iInitCallbackCommand(OnInitCallback);
        iInitEntityCall(OnEntityCallback);
        iInitCompnentCall(OnComponentCallback);
        iInitEntitySyncCall(OnEntitySync);
        iInitial(Application.streamingAssetsPath + "/", Application.persistentDataPath + "/");
    }

    [MonoPInvokeCallback(typeof(CppDelegate))]
    static void OnInitCallback(byte t, IntPtr ptr)
    {
        string command = Marshal.PtrToStringAnsi(ptr);
        switch (t)
        {
            case ASCII.L:
                XDebug.CLog(command);
                break;
            case ASCII.W:
                XDebug.CWarn(command);
                break;
            case ASCII.E:
                XDebug.CError(command);
                break;
            case ASCII.G:
                XDebug.CLog("load object: " + command + " len: " + command.Length);
                GameObject go = XResources.Load<GameObject>(command, AssetType.Prefab);
                go.name = command;
                break;
            case ASCII.U:
                XDebug.CLog("unload: " + command);
                break;
            default:
                XDebug.LogError(t + " is not parse symbol: " + command);
                break;
        }
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
    static void OnEntitySync(uint entityid, byte command, ref VectorArr vec)
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

