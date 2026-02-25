using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float forwardSpeed = 5f;
    public float sideSpeed = 15f;
    public float roadWidth = 4.5f;
    public int score = 0;

    public GameManager gameManager;

    private Rigidbody rb;
    private bool isGameStopped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Time.timeScale = 1f;
    }

    void FixedUpdate()
    {
        if (isGameStopped) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        
        Vector3 v = rb.linearVelocity;
        v.z = forwardSpeed;
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
            isGameStopped = true;
            
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

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
            isGameStopped = true;
            
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (gameManager != null)
            {
                gameManager.CompleteLevel();
            }
            return;
        }
    }
}