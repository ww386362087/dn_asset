using AOT;
using System.Runtime.InteropServices;

public class NativeEntity
{

#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iInitEntityCall(NativeEntityDelegate cb);


#if UNITY_IPHONE || UNITY_XBOX360
	[DllImport("__Internal")]
#else
    [DllImport("GameCore")]
#endif
    public static extern void iInitCompnentCall(NativeComptDelegate cb);

    
    public delegate void NativeEntityDelegate(uint entityid, string method, string arg);
    public delegate void NativeComptDelegate(uint entity, string component, string method, string arg);
    

    [MonoPInvokeCallback(typeof(NativeEntityDelegate))]
    static void OnEntityCallback(uint entityid,string method,string arg)
    {

    }

    [MonoPInvokeCallback(typeof(NativeComptDelegate))]
    static void OnComponentCallback(uint entityid,string compt,string method,string arg)
    {

    }

    public static void InitNative()
    {
        iInitEntityCall(OnEntityCallback);
        iInitCompnentCall(OnComponentCallback);
    }
}

