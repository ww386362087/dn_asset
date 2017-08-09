using UnityEngine;
using UnityEngine.SceneManagement;

public interface ITest
{

    void Start();


    void OnGUI();


    void Update();

}


public class TestManager
{

    static ITest test;

    public static ITest Get()
    {
        if (test != null)
        {
            return test;
        }
        else
        {
            string name = SceneManager.GetActiveScene().name;
            Debug.Log("scene name: "+name);
            switch (name)
            {
                case "cpp":
                    test = new TestCPP();
                    break;
                case "asset":
                    test = new TestAB();
                    break;
                case "DN":
                    test = new TestScene();
                    break;
                default:
                    test = new TestAB();
                    break;
            }
            return test;
        }
    }


}