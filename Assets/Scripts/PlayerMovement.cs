using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("移动设置")]
    public float forwardSpeed = 3f;
    public float sideSpeed = 15f;
    public float roadWidth = 9f;

    private Rigidbody rb;
    private bool isDead = false; // 1. 增加死亡标志位

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 2. 如果已经死亡，直接跳过移动逻辑
        if (isDead) return;

        Vector3 forwardMove = transform.forward * forwardSpeed * Time.fixedDeltaTime;
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 sideMove = transform.right * horizontalInput * sideSpeed * Time.fixedDeltaTime;

        Vector3 newPosition = rb.position + forwardMove + sideMove;
        newPosition.x = Mathf.Clamp(newPosition.x, -roadWidth, roadWidth);

        rb.MovePosition(newPosition);
    }

    // 3. 增加碰撞检测逻辑
    private void OnCollisionEnter(Collision collision)
    {
        // 注意：这里的字符串必须和你 Inspector 里设置的 Tag 完全一致（区分大小写和复数）
        if (collision.gameObject.CompareTag("Obstacles"))
        {
            isDead = true;
            Debug.Log("Game Over! 你撞到了障碍物。");

            // 停止整个游戏的时间流逝
            Time.timeScale = 0;

            // 如果你想让玩家撞击后立刻停下不反弹，可以把速度清零
            rb.linearVelocity = Vector3.zero;
        }
    }
}