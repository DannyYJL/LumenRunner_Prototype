using UnityEngine;
using TMPro; // 如果你使用的是 TextMeshPro

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;  // 将 GameOverText 拖到这里
    public GameObject finishedUI;  // 将 FinishedText 拖到这里
    private bool gameHasEnded = false;

    // 游戏结束逻辑
    public void EndGame()
    {
        if (gameHasEnded == false)
        {
            gameHasEnded = true;
            gameOverUI.SetActive(true); // 显示 Game Over 字样
            Time.timeScale = 0f;        // 停止游戏时间
        }
    }

    // 通关逻辑
    public void CompleteLevel()
    {
        if (gameHasEnded == false)
        {
            gameHasEnded = true;
            finishedUI.SetActive(true); // 显示 Finished 字样
            Time.timeScale = 0f;        // 停止游戏时间
        }
    }
}