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
    public float jumpPadUpwardSpeed = 15f;   // 向上弹射速度

    [Header("Game State")]
    public int score = 0;
    public GameManager gameManager;

    private Rigidbody rb;
    private bool isGameStopped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Time.timeScale = 1f;
        currentForwardSpeed = minSpeed;
    }

    void Update()
    {
        if (isGameStopped) return;

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

        // 先处理 jump pad
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

        if (other.CompareTag("Collectible"))
        {
            score += 1;
            Destroy(other.gameObject);
            return;
        }

        if (other.CompareTag("Finish"))
        {
            StopMovement();

            if (gameManager != null)
            {
                gameManager.CompleteLevel();
            }
            return;
        }
    }

    private void LaunchUpward()
    {
        Vector3 v = rb.linearVelocity;

        // 保持当前 x 和 z，只重设 y 为向上速度
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