#if TEST

using UnityEngine;
using UnityEngine.SceneManagement;

public interface ITest
{

    void Start();


    void OnGUI();


    void Update();


    void LateUpdate();

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
           // Debug.Log("scene name: "+name);
            switch (name)
            {
                case "Cpp":
                    test = new TestCPP();
                    break;
                case "Fashion":
                    test = new TestAB();
                    break;
                case "World":
                    test = new TestScene();
                    break;
                case "CutScene":
                    test = new TestCutScene();
                    break;
                default:
                    test = new TestScene();
                    break;
            }
            return test;
        }
    }


}


#endif