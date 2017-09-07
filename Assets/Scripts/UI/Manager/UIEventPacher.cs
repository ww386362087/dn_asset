using UnityEngine;
using System.Collections.Generic;

public class UIEventPacher
{

    List<UIEventListener.VoidDelegate> void_list;

    List<UIEventListener.VectorDelegate> vector_list;


    protected virtual void Regist()
    {
    }

    protected void UnRegist()
    {
        if (void_list != null)
        {
            for (int i = 0, max = void_list.Count; i < max; i++)
            {
                void_list[i] = null;
            }
            void_list.Clear();
        }
        if (vector_list != null)
        {
            for (int i = 0, max = vector_list.Count; i < max; i++)
            {
                vector_list[i] = null;
            }
            vector_list.Clear();
        }
    }


    private void Attach(ref UIEventListener.VoidDelegate dele)
    {
        if (void_list == null)
            void_list = new List<UIEventListener.VoidDelegate>();

        void_list.Add(dele);
    }


    private void Attach(ref UIEventListener.VectorDelegate dele)
    {
        if (vector_list == null)
            vector_list = new List<UIEventListener.VectorDelegate>();

        vector_list.Add(dele);
    }

    protected void RegistClick(GameObject go, UIEventListener.VoidDelegate cb)
    {
        UIEventListener.Get(go).onClick = cb;
        Attach(ref UIEventListener.Get(go).onClick);
    }

    protected void RegistSelect(GameObject go, UIEventListener.VoidDelegate cb)
    {
        UIEventListener.Get(go).onSelect = cb;
        Attach(ref UIEventListener.Get(go).onSelect);
    }

    protected void RegistDrag(GameObject go, UIEventListener.VectorDelegate cb)
    {
        UIEventListener.Get(go).onDrag = cb;
        Attach(ref UIEventListener.Get(go).onDrag);
    }

    protected void RegistDoubleClick(GameObject go, UIEventListener.VoidDelegate cb)
    {
        UIEventListener.Get(go).onDoubleClick = cb;
        Attach(ref UIEventListener.Get(go).onDoubleClick);
    }

}



