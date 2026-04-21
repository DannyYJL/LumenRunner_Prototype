using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public enum DeathCause
{
    Fissure,
    MovingObstacle,
    StaticObstacle,
    Laser,
    Chaser,
    Other
}

public class AnalyticsUploader : MonoBehaviour
{
    public static AnalyticsUploader Instance { get; private set; }

    [Header("Apps Script Web App URL")]
    public string endpoint =
        "https://script.google.com/macros/s/AKfycbw9M_oug82hPArKXtQEsRy-_8_MtHyMKrYEYPW0fpbpMgmxpzgnRnqPGNPKd1t_VXqM/exec";

    [Header("Must match API_TOKEN in Apps Script")]
    public string token = "888";

    private int runId = 0;

    private int attemptsInCurrentLevel = 1;
    private string currentLevelName = "";
    private DeathCause? lastDeathCause = null;

    public DeathCause? LastDeathCause => lastDeathCause;

    [Header("Menu Scene Name")]
    public string mainMenuSceneName = "MainMenu";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != mainMenuSceneName)
        {
            currentLevelName = GetAnalyticsLevelName(scene.name);
            Debug.Log($"[Analytics] Current level set to: {currentLevelName} (scene: {scene.name})");
        }
    }

    private string GetAnalyticsLevelName(string sceneName)
    {
        switch (sceneName)
        {
            case "SampleScene":
                return "Level_1";

            case "Level_2":
                return "Level_2";

            case "Tutorial_Level":
                return "Level_Tutorial";

            default:
                return sceneName;
        }
    }

    public void StartNewRun()
    {
        runId++;
        Debug.Log("Starting new run: " + runId);
    }

    public void StartLevelSession(string levelName)
    {
        currentLevelName = levelName;
        attemptsInCurrentLevel = 1;
        runId++;
        lastDeathCause = null;

        Debug.Log($"[Analytics] StartLevelSession level={currentLevelName}, attempts=1, runId={runId}");
    }

    public void StartNextAttemptInSameLevel()
    {
        attemptsInCurrentLevel++;
        runId++;
        lastDeathCause = null;

        Debug.Log($"[Analytics] StartNextAttemptInSameLevel level={currentLevelName}, attempts={attemptsInCurrentLevel}, runId={runId}");
    }

    public void LogDeath(DeathCause cause)
    {
        string timestamp = DateTime.UtcNow.ToString("o");
        string levelNameForLog = string.IsNullOrEmpty(currentLevelName)
            ? SceneManager.GetActiveScene().name
            : currentLevelName;
        lastDeathCause = cause;

        Debug.LogWarning($"[Death] level={levelNameForLog} cause={cause}");
        StartCoroutine(PostDeath(runId, timestamp, cause.ToString()));
    }

    public void LogAttemptsBeforeCompletion()
    {
        string timestamp = DateTime.UtcNow.ToString("o");
        StartCoroutine(PostAttemptsBeforeCompletion(timestamp, currentLevelName, attemptsInCurrentLevel));
    }

    IEnumerator PostDeath(int runId, string timestamp, string reason)
    {
        WWWForm form = new WWWForm();
        form.AddField("token", token);
        form.AddField("metric_type", "death");
        form.AddField("run_id", runId);
        form.AddField("timestamp", timestamp);
        form.AddField("death_reason", reason);

        using var req = UnityWebRequest.Post(endpoint, form);
        yield return req.SendWebRequest();

        Debug.Log($"[Analytics][Death] reason={reason} code={req.responseCode} result={req.result} err={req.error}");
        Debug.Log($"[Analytics][Death] reason={reason} resp={req.downloadHandler.text}");
    }

    IEnumerator PostAttemptsBeforeCompletion(string timestamp, string levelName, int attempts)
    {
        WWWForm form = new WWWForm();
        form.AddField("token", token);
        form.AddField("metric_type", "attempts_before_completion");
        form.AddField("timestamp", timestamp);
        form.AddField("level_name", levelName);
        form.AddField("attempts_before_completion", attempts);

        using var req = UnityWebRequest.Post(endpoint, form);
        yield return req.SendWebRequest();

        Debug.Log($"[Analytics][Attempts] code={req.responseCode} result={req.result} err={req.error}");
        Debug.Log($"[Analytics][Attempts] resp={req.downloadHandler.text}");
    }

    public void LogSurvivalTimePerAttempt(float survivalTimeSeconds, string endType)
    {
        string timestamp = DateTime.UtcNow.ToString("o");
        StartCoroutine(PostSurvivalTimePerAttempt(
            timestamp,
            currentLevelName,
            attemptsInCurrentLevel,
            survivalTimeSeconds,
            endType
        ));
    }

    IEnumerator PostSurvivalTimePerAttempt(
        string timestamp,
        string levelName,
        int attemptNumber,
        float survivalTimeSeconds,
        string endType)
    {
        WWWForm form = new WWWForm();
        form.AddField("token", token);
        form.AddField("metric_type", "survival_time_per_attempt");
        form.AddField("timestamp", timestamp);
        form.AddField("level_name", levelName);
        form.AddField("attempt_number", attemptNumber);
        form.AddField("survival_time_seconds", survivalTimeSeconds.ToString("F3"));
        form.AddField("end_type", endType);

        using var req = UnityWebRequest.Post(endpoint, form);
        yield return req.SendWebRequest();

        Debug.Log($"[Analytics][Survival] code={req.responseCode} result={req.result} err={req.error}");
        Debug.Log($"[Analytics][Survival] resp={req.downloadHandler.text}");
    }

    public void LogDeathDistanceAlongZ(float distanceAlongZ)
    {
        string timestamp = DateTime.UtcNow.ToString("o");
        string causeText = lastDeathCause?.ToString() ?? DeathCause.Other.ToString();
        Debug.LogWarning($"[Death] cause={causeText} distanceAlongZ={distanceAlongZ:F3}");
        StartCoroutine(PostDeathDistanceAlongZ(timestamp, currentLevelName, distanceAlongZ));
    }

    IEnumerator PostDeathDistanceAlongZ(string timestamp, string levelName, float distanceAlongZ)
    {
        WWWForm form = new WWWForm();
        form.AddField("token", token);
        form.AddField("metric_type", "death_location");
        form.AddField("timestamp", timestamp);
        form.AddField("level_name", levelName);
        form.AddField("distance_along_z", distanceAlongZ.ToString("F3"));

        using var req = UnityWebRequest.Post(endpoint, form);
        yield return req.SendWebRequest();

        string causeText = lastDeathCause?.ToString() ?? DeathCause.Other.ToString();
        Debug.Log($"[Analytics][DeathLocation] cause={causeText} code={req.responseCode} result={req.result} err={req.error}");
        Debug.Log($"[Analytics][DeathLocation] cause={causeText} resp={req.downloadHandler.text}");
    }
}
