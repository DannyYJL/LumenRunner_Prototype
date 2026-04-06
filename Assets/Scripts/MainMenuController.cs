using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void LoadTutorial()
    {
        AnalyticsUploader.Instance?.StartLevelSession("Level_Tutorial");
        SceneManager.LoadScene("Tutorial_Level");
    }

    public void LoadSample()
    {
        AnalyticsUploader.Instance?.StartLevelSession("Level_1");
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadLevel2()
    {
        AnalyticsUploader.Instance?.StartLevelSession("Level_2");
        SceneManager.LoadScene("Level_2");
    }
}