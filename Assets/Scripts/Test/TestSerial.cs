using UnityEngine;

[System.Serializable]
public class Clas
{
    public int a;

    public string str = "";

}


public class TestSerial : MonoBehaviour
{

    [SerializeField]
    public Clas _a ;

    public Clas a
    {
        get
        {
            if (_a == null)
                _a = new Clas();
            return _a;
        }
        set { _a = value; }
    }

    // Use this for initialization
    void Start()
    {
        XDebug.LogGreen("a value is: ", a.a, " str:", a.str);
    }

    // Update is called once per frame
    void Update()
    {

    }

}
