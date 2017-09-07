using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventListener : EventTrigger
{

    public delegate void VoidDelegate(GameObject go);
    public delegate void VectorDelegate(GameObject go, Vector2 delta);
    public VoidDelegate onClick;
    public VoidDelegate onDoubleClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    public VectorDelegate onDrag;


    private float lastClickTime = 0;

    static public UIEventListener Get(GameObject go)
    {
        UIEventListener listener = go.GetComponent<UIEventListener>();
        if (listener == null)
        {
            listener = go.AddComponent<UIEventListener>();
        }
        return listener;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickTime - lastClickTime < 0.3f)
        {
            if (onDoubleClick != null) onDoubleClick(gameObject);
            lastClickTime = 0;
        }
        else
        {
            if (onClick != null) onClick(gameObject);
            lastClickTime = eventData.clickTime;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null) onDown(gameObject);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter(gameObject);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null) onExit(gameObject);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null) onUp(gameObject);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(gameObject);
    }

    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null) onUpdateSelect(gameObject);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null) onDrag(gameObject, eventData.delta);
    }

}
