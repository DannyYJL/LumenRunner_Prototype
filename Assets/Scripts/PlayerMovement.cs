using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("移动设置")]
    public float forwardSpeed = 5f;    // 前进速度
    public float sideSpeed = 15f;      // 左右移动灵敏度
    public float roadWidth = 4.5f;    // 道路半宽（根据你的路面宽度调整）

    [Header("UI 引用")]
    public GameObject gameOverUI;     // 在 Inspector 中拖入 Game_Over 物体
    public GameObject finishedUI;      // 在 Inspector 中拖入 Finished 物体

    private Rigidbody rb;
    private bool isGameStopped = false; // 游戏状态位

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 游戏开始时隐藏所有 UI
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (finishedUI != null) finishedUI.SetActive(false);

        // 确保游戏时间正常运行
        Time.timeScale = 1f;
    }

    void FixedUpdate()
    {
        // 如果游戏已经结束，不再处理移动
        if (isGameStopped) return;

        // 1. 处理自动前进
        Vector3 forwardMove = transform.forward * forwardSpeed * Time.fixedDeltaTime;

        // 2. 处理玩家左右输入
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 sideMove = transform.right * horizontalInput * sideSpeed * Time.fixedDeltaTime;

        // 3. 计算新位置并限制在道路宽度内
        Vector3 newPosition = rb.position + forwardMove + sideMove;
        newPosition.x = Mathf.Clamp(newPosition.x, -roadWidth, roadWidth);

        // 4. 应用移动
        rb.MovePosition(newPosition);
    }

    // 检测撞到障碍物（实体碰撞）
    private void OnCollisionEnter(Collision collision)
    {
        // 检查碰撞物标签是否为 Obstacles
        if (collision.gameObject.CompareTag("Obstacles"))
        {
            TriggerEndGame(gameOverUI);
            Debug.Log("Game Over! 撞到了障碍物。");
        }
    }

    // 检测到达终点（穿过触发器）
    private void OnTriggerEnter(Collider other)
    {
        // 检查触发物标签是否为 Finish
        if (other.CompareTag("Finish"))
        {
            TriggerEndGame(finishedUI);
            Debug.Log("Level Finished! 顺利通关。");
        }
    }

    // 统一的游戏结束处理逻辑
    private void TriggerEndGame(GameObject uiToShow)
    {
        if (isGameStopped) return; // 防止重复触发

        isGameStopped = true;

        // 1. 显示对应的 UI 文本
        if (uiToShow != null)
        {
            uiToShow.SetActive(true);
        }

        // 2. 停止物理运动
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 3. 停止时间流逝
        Time.timeScale = 0f;
    }
}