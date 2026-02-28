using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed Settings")]
    public float minSpeed = 5f;          // Minimum speed (cannot be 0)
    public float maxSpeed = 30f;         // Maximum speed
    public float acceleration = 15f;     // How fast it speeds up/slows down
    private float currentForwardSpeed;   // Current dynamic speed

    [Header("Lateral Movement")]
    public float sideSpeed = 15f;
    public float roadWidth = 4.5f;

    [Header("Game State")]
    public int score = 0;
    public GameManager gameManager;

    private Rigidbody rb;
    private bool isGameStopped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Time.timeScale = 1f;
        
        // Start the game at the minimum speed
        currentForwardSpeed = minSpeed;
    }

    void Update()
    {
        if (isGameStopped) return;

        // Accelerate with W
        if (Input.GetKey(KeyCode.W))
        {
            currentForwardSpeed += acceleration * Time.deltaTime;
        }
        // Decelerate with S
        else if (Input.GetKey(KeyCode.S))
        {
            currentForwardSpeed -= acceleration * Time.deltaTime;
        }

        // Clamp speed so it stays between minSpeed and maxSpeed
        currentForwardSpeed = Mathf.Clamp(currentForwardSpeed, minSpeed, maxSpeed);
    }

    void FixedUpdate()
    {
        if (isGameStopped) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        
        Vector3 v = rb.linearVelocity;
        v.z = currentForwardSpeed; // Use the dynamic speed controlled by W/S
        v.x = horizontalInput * sideSpeed;
        rb.linearVelocity = v;
        
        Vector3 p = rb.position;
        p.x = Mathf.Clamp(p.x, -roadWidth, roadWidth);
        rb.position = p;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacles"))
        {
            if (isGameStopped) return;
            StopMovement();

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

    // Helper method to stop the player physics
    private void StopMovement()
    {
        isGameStopped = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}