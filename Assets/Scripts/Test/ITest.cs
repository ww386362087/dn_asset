#if TEST

using UnityEngine.SceneManagement;

public interface ITest
{

    void Start();


    void OnGUI();


    void Update();


    void LateUpdate();


    void OnQuit();

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
           //  XDebug.Log("scene name: "+name);
            switch (name.ToLower())
            {
                case "cpp":
                    test = new TestCPP();
                    break;
                case "fashion":
                    test = new TestFashion();
                    break;
                case "world":
                    test = new TestWorld();
                    break;
                case "cutscene":
                    test = new TestCutScene();
                    break;
                case "ai":
                    test = new TestAI();
                    break;
                default:
                    test = new Test();
                    break;
            }
            return test;
        }
    }


}


#endif