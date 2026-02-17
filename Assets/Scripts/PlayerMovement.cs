using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float forwardSpeed = 5f;
    public float sideSpeed = 15f;
    public float roadWidth = 4.5f;
    public int score = 0;

    public GameObject gameOverUI;
    public GameObject finishedUI;

    private Rigidbody rb;
    private bool isGameStopped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (finishedUI != null) finishedUI.SetActive(false);
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
            TriggerEndGame(gameOverUI);
        }
    }
    
    private void OnTriggerEnter(Collider other)
{
    if (isGameStopped) return;

    Debug.Log("Triggered with: " + other.gameObject.name + " | Tag: " + other.tag);

    if (other.CompareTag("Collectible"))
    {
        score += 1;
        Debug.Log("Score: " + score);
        Destroy(other.gameObject);
        return;
    }

    if (other.CompareTag("Finish"))
    {
        TriggerEndGame(finishedUI);
        return;
    }
}


    private void TriggerEndGame(GameObject uiToShow)
    {
        if (isGameStopped) return;

        isGameStopped = true;

        if (uiToShow != null)
        {
            uiToShow.SetActive(true);
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Time.timeScale = 0f;
    }
}