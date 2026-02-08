using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject finishedUI;
    private bool gameHasEnded = false;

    public void EndGame()
    {
        if (gameHasEnded == false)
        {
            gameHasEnded = true;
            gameOverUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }


    public void CompleteLevel()
    {
        if (gameHasEnded == false)
        {
            gameHasEnded = true;
            finishedUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}