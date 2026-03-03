using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public enum DeathCause
{
    Fissure,
    MovingObstacle,
    Laser,
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

    void Awake()
    {
        // 单例模式（和你原来一样）
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartNewRun(); // 启动即为 Run 1
    }

    public void StartNewRun()
    {
        runId++;
        Debug.Log("Starting new run: " + runId);
    }

    public void LogDeath(DeathCause cause)
    {
        string timestamp = DateTime.UtcNow.ToString("o");
        StartCoroutine(PostDeath(runId, timestamp, cause.ToString()));
    }

    IEnumerator PostDeath(int runId, string timestamp, string reason)
    {
        WWWForm form = new WWWForm();
        form.AddField("token", token);
        form.AddField("run_id", runId);
        form.AddField("timestamp", timestamp);
        form.AddField("death_reason", reason);

        using var req = UnityWebRequest.Post(endpoint, form);
        yield return req.SendWebRequest();

        Debug.Log($"[Analytics] code={req.responseCode} result={req.result} err={req.error}");
        Debug.Log($"[Analytics] resp={req.downloadHandler.text}");
    }

    [Serializable]
    class DeathEvent
    {
        public string token;
        public int run_id;
        public string timestamp;
        public string death_reason;
    }
}