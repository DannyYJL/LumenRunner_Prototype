using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed Settings")]
    public float baseSpeed = 5f;      // 原速
    public float slowSpeed = 2f;      // 按 S 时的最低速度
    public float maxSpeed = 30f;      // 按 W 时的最高速度
    public float accelerateRate = 15f; // W/S 改变速度的速度
    public float recoverRate = 10f;    // 松开后恢复原速的速度

    private float currentForwardSpeed;
    private float targetForwardSpeed;

    [Header("Lateral Movement")]
    public float sideSpeed = 15f;
    public float roadWidth = 4.5f;

    [Header("Jump Pad Settings")]
    public float jumpPadUpwardSpeed = 15f;

    [Header("Game State")]
    public int score = 0;
    public GameManager gameManager;

    private Rigidbody rb;
    private bool isGameStopped = false;
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

        // 设置目标速度
        if (Input.GetKey(KeyCode.W))
        {
            targetForwardSpeed = maxSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            targetForwardSpeed = slowSpeed;
        }
        else
        {
            targetForwardSpeed = baseSpeed;
        }

        // 平滑靠近目标速度
        float rate = (targetForwardSpeed == baseSpeed) ? recoverRate : accelerateRate;
        currentForwardSpeed = Mathf.MoveTowards(
            currentForwardSpeed,
            targetForwardSpeed,
            rate * Time.deltaTime
        );
    }

    void FixedUpdate()
    {
        if (isGameStopped) return;

        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 v = rb.linearVelocity;   // 如果你的 Unity 版本不支持 linearVelocity，改成 velocity
        v.z = currentForwardSpeed;
        v.x = horizontalInput * sideSpeed;
        rb.linearVelocity = v;

        Vector3 p = rb.position;
        p.x = Mathf.Clamp(p.x, -roadWidth, roadWidth);
        rb.position = p;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isGameStopped) return;

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

            if (collision.gameObject.CompareTag("Moving Obstacle")){
                FindObjectOfType<AnalyticsUploader>()?.LogDeath(DeathCause.MovingObstacle);
                AnalyticsUploader.Instance?.LogDeathDistanceAlongZ(GetDistanceAlongZ());
            }
                
            if (collision.gameObject.CompareTag("Fall")){
                FindObjectOfType<AnalyticsUploader>()?.LogDeath(DeathCause.Fissure);
                AnalyticsUploader.Instance?.LogDeathDistanceAlongZ(GetDistanceAlongZ());
            }
                
            if (collision.gameObject.CompareTag("Obstacles")){
                FindObjectOfType<AnalyticsUploader>()?.LogDeath(DeathCause.Other);
                AnalyticsUploader.Instance?.LogDeathDistanceAlongZ(GetDistanceAlongZ());
            }

            if (gameManager != null)
            {
                gameManager.EndGame();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGameStopped) return;

        if (other.CompareTag("Finish"))
        {
            StopMovement();

            AnalyticsUploader.Instance?.LogAttemptsBeforeCompletion();

            if (gameManager != null)
            {
                gameManager.CompleteLevel();
            }
        }
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

    private void LaunchUpward()
    {
        Vector3 v = rb.linearVelocity;
        v.y = jumpPadUpwardSpeed;
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