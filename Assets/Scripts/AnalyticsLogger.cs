using System;
using System.IO;
using UnityEngine;

public enum DeathCause
{
    Fissure,
    MovingObstacle,
    Laser,
    Other
}

public class AnalyticsLogger : MonoBehaviour
{
    public static AnalyticsLogger Instance { get; private set; }

    private string filePath;
    private int runId = 0;

    private void Awake()
    {
        // 单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 将 CSV 存储在 Unity 项目根目录
        filePath = Path.Combine(
            Directory.GetParent(Application.dataPath).FullName,
            "death_log.csv"
        );

        Debug.Log("Analytics file path: " + filePath);

        InitializeFile();
        StartNewRun(); // 第一次启动视为 Run 1
    }

    private void InitializeFile()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "timestamp,runId,cause\n");
            Debug.Log("Created new analytics CSV file.");
        }
    }

    public void StartNewRun()
    {
        runId++;
        Debug.Log("Starting new run: " + runId);
    }

    public void LogDeath(DeathCause cause)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogWarning("Analytics file path not set.");
            return;
        }

        string timestamp = DateTime.UtcNow.ToString("o"); // ISO 8601格式
        string line = $"{timestamp},{runId},{cause}\n";

        File.AppendAllText(filePath, line);

        Debug.Log($"[Analytics] Logged death: {cause} (Run {runId})");
    }

    // 可选：清空日志（调试用）
    public void ClearLog()
    {
        File.WriteAllText(filePath, "timestamp,runId,cause\n");
        Debug.Log("Analytics log cleared.");
    }

    public string GetFilePath()
    {
        return filePath;
    }
}