using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed Settings")]
    public float minSpeed = 5f;
    public float maxSpeed = 30f;
    public float acceleration = 15f;
    private float currentForwardSpeed;

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

    public int Score => score;
    public float ElapsedTime => elapsedTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Time.timeScale = 1f;
        currentForwardSpeed = minSpeed;
        elapsedTime = 0f;
    }

    void Update()
    {
        if (isGameStopped) return;

        elapsedTime += Time.deltaTime;

        if (Input.GetKey(KeyCode.W))
        {
            currentForwardSpeed += acceleration * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            currentForwardSpeed -= acceleration * Time.deltaTime;
        }

        currentForwardSpeed = Mathf.Clamp(currentForwardSpeed, minSpeed, maxSpeed);
    }

    void FixedUpdate()
    {
        if (isGameStopped) return;

        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 v = rb.linearVelocity;
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

            if (collision.gameObject.CompareTag("Moving Obstacle"))
                FindObjectOfType<AnalyticsUploader>()?.LogDeath(DeathCause.MovingObstacle);
            if (collision.gameObject.CompareTag("Fall"))
                FindObjectOfType<AnalyticsUploader>()?.LogDeath(DeathCause.Fissure);
            if (collision.gameObject.CompareTag("Obstacles"))
                FindObjectOfType<AnalyticsUploader>()?.LogDeath(DeathCause.Other);

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
}
