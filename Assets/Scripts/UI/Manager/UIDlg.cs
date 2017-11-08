using UnityEngine;

public enum DlgType
{
    Stack,   //入栈的 
    Top,     //最前的 提示框 跑马灯等
    Surface,  //贴在栈顶ui表面 放在栈顶 但可以被其他UI覆盖 如聊天 需要自己管理
    Fixed     //固定层级的 depth只由prefab上的设定决定
}

public abstract class UIDlg<TDlg, TBehaviour> : UIEventPacher, IUIDlg
    where TBehaviour : DlgBehaviourBase
    where TDlg :  IUIDlg, new()
{

    private static TDlg s_instance = default(TDlg);

    private static object s_objLock = new object();


    private DlgBehaviourBase _uibehaviour;
    private bool _show = false;
    private bool _load = false;
    private uint _id = 0;

    public abstract string fileName { get; }

    public uint id
    {
        get
        {
            if (_id == 0)
            {
                _id = XCommon.singleton.XHash(fileName);
            }
            return _id;
        }
    }

    public static TDlg singleton
    {
        get
        {
            if (null == s_instance)
            {
                lock (s_objLock)
                {
                    if (null == s_instance)
                    {
                        s_instance = new TDlg();
                    }
                }
            }
            return s_instance;
        }
    }

    public bool IsVisible()
    {
        return _show;
    }

    public bool IsLoaded()
    {
        return _load;
    }

    public TBehaviour uiBehaviour
    {
        get { return _uibehaviour as TBehaviour; }
    }

    public DlgBehaviourBase innerBehaviour
    {
        get { return _uibehaviour; }
    }

    public bool pushStack
    {
        get { return type == DlgType.Stack; }
    }

    public virtual DlgType type
    {
        get { return DlgType.Stack; }
    }
    
    public virtual bool isCutscene { get { return false; } }

    public virtual bool shareCanvas
    { get { return false; } }


    public virtual void OnLoad()
    {
        _load = true;
        _show = true;
       
    }

    public virtual void OnShow()
    {
        Regist();
        _show = true;
    }
    
    public virtual void OnHide()
    {
        UnRegist();
        _show = false;
    }
    
    public virtual void OnDestroy()
    {
        UnRegist();
        _load = false;
        _show = false;
    }
    
    public void SetVisible(bool visble)
    {
        if (visble)
        {
            UIManager.singleton.LoadDlg(this as IUIDlg);
        }
        else
        {
            UIManager.singleton.Hide(this as IUIDlg);
        }
    }
    
    
    public void SetBehaviour(GameObject _go)
    {
        _uibehaviour = _go.AddComponent<TBehaviour>();
        _go.SetActive(true);
        OnLoad();
    }
    
}

