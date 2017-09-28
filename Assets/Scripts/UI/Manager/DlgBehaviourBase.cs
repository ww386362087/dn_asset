using UnityEngine;

public class DlgBehaviourBase : MonoBehaviour
{

    public int ChildCnt = 0;
    public RectTransform rect;
    protected Transform[] _objs;

    void Awake()
    {
        rect = transform.GetComponent<RectTransform>();
        _objs = transform.GetComponentsInChildren<Transform>(true);
        ChildCnt = _objs.Length;
        OnInitial();
    }

    public virtual void OnInitial()
    {
        _objs = null;
        ChildCnt = 0;
    }


    public Transform GetUIObj(string strName)
    {
        for (int i = 0; i < ChildCnt; i++)
        {
            if (_objs[i].name.Equals(strName))
            {
                return _objs[i];
            }
        }
        return null;
    }

    public int SortDepth(int start)
    {
        int val = start;
        for (int i = 0; i < ChildCnt; i++)
        {
            int sort = _objs[i].transform.GetSiblingIndex();
            sort += start;
            val = Mathf.Max(val, sort);
            _objs[i].transform.SetSiblingIndex(sort);
        }
        return val;
    }


}
