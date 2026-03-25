using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // 这里的名字必须和你的 Scene 文件名一模一样！
    public void LoadTutorial() { SceneManager.LoadScene("Tutorial_Level"); }
    public void LoadSample() { SceneManager.LoadScene("SampleScene"); }
    public void LoadLevel2() { SceneManager.LoadScene("Level_2"); }
}