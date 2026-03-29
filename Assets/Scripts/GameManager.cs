using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gameOverTextUI;
    public GameObject restartButtonUI;
    public GameObject finishedUI;

    [Header("Settings")]
    public float delayBeforeRestart = 2.0f;
    public string levelSelectionSceneName = "MainMenu";
    public float delayBeforeReturnToLevelSelection = 2.0f;
    private bool gameHasEnded = false;
    private bool canRestart = false;
    private TextMeshProUGUI runSummaryText;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        EnsureRunSummaryText();
        SetRunSummaryVisible(false);
    }

    void Update()
    {
        if (!canRestart)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            RestartGame();
        }
    }

    public void EndGame()
    {
        if (!gameHasEnded)
        {
            gameHasEnded = true;
            canRestart = false;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            StartCoroutine(ShowRestartSequence());
        }
    }

    private IEnumerator ShowRestartSequence()
    {
        if (gameOverTextUI != null)
        {
            gameOverTextUI.SetActive(true);
        }

        if (restartButtonUI != null)
        {
            restartButtonUI.SetActive(false);
        }

        SetRunSummaryVisible(false);
        yield return new WaitForSecondsRealtime(delayBeforeRestart);

        if (gameOverTextUI != null)
        {
            gameOverTextUI.SetActive(false);
        }

        ShowRunSummary("RUN OVER");

        if (restartButtonUI != null)
        {
            restartButtonUI.SetActive(true);
        }

        canRestart = true;
    }

    public void CompleteLevel()
    {
        if (!gameHasEnded)
        {
            gameHasEnded = true;
            canRestart = false;
            if (finishedUI != null)
            {
                finishedUI.SetActive(true);
            }

            ShowRunSummary("LEVEL CLEAR");
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            StartCoroutine(ReturnToLevelSelection());
        }
    }

    public void RestartGame()
    {
        if (!gameHasEnded)
        {
            return;
        }

        canRestart = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        AnalyticsUploader.Instance?.StartNewRun();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void EnsureRunSummaryText()
    {
        if (runSummaryText != null)
        {
            return;
        }

        TextMeshProUGUI template = null;
        if (finishedUI != null)
        {
            template = finishedUI.GetComponent<TextMeshProUGUI>();
        }

        if (template == null && gameOverTextUI != null)
        {
            template = gameOverTextUI.GetComponent<TextMeshProUGUI>();
        }

        if (template == null)
        {
            return;
        }

        runSummaryText = Instantiate(template, template.transform.parent);
        runSummaryText.name = "RuntimeRunSummaryText";
        runSummaryText.alignment = TextAlignmentOptions.Center;
        runSummaryText.raycastTarget = false;
        runSummaryText.fontSize = Mathf.Max(24f, template.fontSize * 0.7f);

        RectTransform templateRect = template.rectTransform;
        RectTransform summaryRect = runSummaryText.rectTransform;
        summaryRect.anchorMin = templateRect.anchorMin;
        summaryRect.anchorMax = templateRect.anchorMax;
        summaryRect.pivot = templateRect.pivot;
        summaryRect.sizeDelta = new Vector2(Mathf.Max(templateRect.sizeDelta.x, 420f), 90f);
        summaryRect.anchoredPosition = templateRect.anchoredPosition + new Vector2(0f, -70f);
        runSummaryText.text = string.Empty;
    }

    private void ShowRunSummary(string title)
    {
        EnsureRunSummaryText();
        if (runSummaryText == null)
        {
            return;
        }

        string scoreValue = playerMovement != null ? playerMovement.Score.ToString() : "0";
        string timeValue = playerMovement != null ? playerMovement.GetFormattedElapsedTime() : "00:00.0";
        runSummaryText.text = title + "\nSCORE: " + scoreValue + "\nTIME: " + timeValue;
        SetRunSummaryVisible(true);
    }

    private void SetRunSummaryVisible(bool isVisible)
    {
        if (runSummaryText != null)
        {
            runSummaryText.gameObject.SetActive(isVisible);
        }
    }

    private IEnumerator ReturnToLevelSelection()
    {
        yield return new WaitForSecondsRealtime(delayBeforeReturnToLevelSelection);

        Time.timeScale = 1f;
        SceneManager.LoadScene(levelSelectionSceneName);
    }
}
