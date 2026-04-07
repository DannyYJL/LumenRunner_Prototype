using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed Settings")]
    public float baseSpeed = 5f;
    public float slowSpeed = 2f;
    public float maxSpeed = 18f;      // 建议最高速不要太夸张，18 比较平衡
    public float accelerateRate = 15f;
    public float recoverRate = 10f;

    private float currentForwardSpeed;
    private float targetForwardSpeed;

    [Header("Lateral Movement")]
    public float sideSpeed = 15f;
    public float roadWidth = 4.5f;

    [Header("Jump Pad Settings (Calculated for Apex)")]
    public float jumpPadUpwardSpeed = 13f;   // 垂直初速度 (Vy)
    public float jumpPadForwardSpeed = 7.7f; // 水平锁定速度 (Vz)

    [Header("Game State")]
    public int score = 0;
    public GameManager gameManager;

    private Rigidbody rb;
    private bool isGameStopped = false;
    private bool isJumping = false; // 关键：跳跃状态锁
    private float elapsedTime = 0f;
    private float attemptStartZ;

    public int Score => score;
    public float ElapsedTime => elapsedTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Time.timeScale = 1f;
        currentForwardSpeed = baseSpeed;
        targetForwardSpeed = baseSpeed;
        elapsedTime = 0f;
        attemptStartZ = transform.position.z;
    }

    void Update()
    {
        if (isGameStopped) return;
        elapsedTime += Time.deltaTime;

        // 只有不在跳跃中，才允许玩家通过 W/S 控制速度
        if (!isJumping)
        {
            if (Input.GetKey(KeyCode.W)) targetForwardSpeed = maxSpeed;
            else if (Input.GetKey(KeyCode.S)) targetForwardSpeed = slowSpeed;
            else targetForwardSpeed = baseSpeed;

            float rate = (targetForwardSpeed == baseSpeed) ? recoverRate : accelerateRate;
            currentForwardSpeed = Mathf.MoveTowards(currentForwardSpeed, targetForwardSpeed, rate * Time.deltaTime);
        }
        // 如果在跳跃中，currentForwardSpeed 会保持起跳时 LaunchUpward 赋予的 7.7
    }

    void FixedUpdate()
    {
        if (isGameStopped) return;
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 v = rb.linearVelocity;
        v.z = currentForwardSpeed;
        v.x = horizontalInput * sideSpeed; // 依然允许左右微调，防止跳歪
        rb.linearVelocity = v;

        Vector3 p = rb.position;
        p.x = Mathf.Clamp(p.x, -roadWidth, roadWidth);
        rb.position = p;
    }

    public void AddScore(int points)
    {
        score += Mathf.Max(0, points);
    }

    public string GetFormattedElapsedTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        float seconds = elapsedTime % 60f;
        return minutes.ToString("00") + ":" + seconds.ToString("00.0");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isGameStopped) return;

        // 落地检测：碰到路面或障碍物，解锁速度控制
        if (collision.gameObject.CompareTag("Road") || collision.gameObject.CompareTag("Obstacles"))
        {
            if (isJumping)
            {
                isJumping = false;
                // 落地一瞬间，让目标速度平滑恢复
                targetForwardSpeed = baseSpeed;
            }
        }

        if (collision.gameObject.CompareTag("JumpPad"))
        {
            LaunchUpward();
            return;
        }

        if (collision.gameObject.CompareTag("Moving Obstacle") ||
            collision.gameObject.CompareTag("Fall") ||
            collision.gameObject.CompareTag("Obstacles"))
        {
            StopMovement();

            if (collision.gameObject.CompareTag("Moving Obstacle"))
                FindObjectOfType<AnalyticsUploader>()?.LogDeath(DeathCause.MovingObstacle);
            else if (collision.gameObject.CompareTag("Fall"))
                FindObjectOfType<AnalyticsUploader>()?.LogDeath(DeathCause.Fissure);
            else
                FindObjectOfType<AnalyticsUploader>()?.LogDeath(DeathCause.Other);

            AnalyticsUploader.Instance?.LogDeathDistanceAlongZ(GetDistanceAlongZ());

            if (gameManager != null) gameManager.EndGame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGameStopped) return;
        if (other.CompareTag("Finish"))
        {
            StopMovement();
            AnalyticsUploader.Instance?.LogAttemptsBeforeCompletion();
            if (gameManager != null) gameManager.CompleteLevel();
        }
    }

    private void LaunchUpward()
    {
        isJumping = true; // 锁定状态

        // 强制修正当前的所有前进速度参数
        currentForwardSpeed = jumpPadForwardSpeed;
        targetForwardSpeed = jumpPadForwardSpeed;

        Vector3 v = rb.linearVelocity;
        v.y = jumpPadUpwardSpeed;   // 13
        v.z = jumpPadForwardSpeed;  // 7.7
        rb.linearVelocity = v;
    }

    private void StopMovement()
    {
        isGameStopped = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public float GetDistanceAlongZ()
    {
        return transform.position.z - attemptStartZ;
    }
}