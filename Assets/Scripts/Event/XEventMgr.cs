
public delegate void XEventHandler(XEventArgs e);

public struct EventHandler
{
    public XEventDefine eventDefine;
    public XEventHandler handler;
}

public class XEventMgr : XSingleton<XEventMgr>
{
    
    public bool FireEvent(XEventArgs args)
    {
        return DispatchEvent(args);
    }

    private bool DispatchEvent(XEventArgs args)
    {
        bool bHandled = false;

        if (!(args.Firer == null || (args.Firer.Deprecated)))
            bHandled = args.Firer.DispatchEvent(args);

        if (!args.ManualRecycle)
            args.Recycle();

        return bHandled;
    }

}

