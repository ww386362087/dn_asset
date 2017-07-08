using UnityEngine;
using System.Collections;


/// <summary>
/// 处理对象生命周期、事件相关
/// </summary>
public class XObject 
{

    public bool Deprecated { get; set; }


    protected void Initilize()
    {
        Deprecated = false;
    }

    /// <summary>
    /// 登录进入
    /// </summary>
    protected virtual void OnAttachToHost()
    {
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    protected virtual void OnDetachFromHost()
    {
    }

    /// <summary>
    /// 注册事件
    /// </summary>
    protected virtual void EventSubscribe()
    {
    }


}
