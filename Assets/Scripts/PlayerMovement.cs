using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("移动设置")]
    public float forwardSpeed = 8f;   // 自动向前的速度
    public float sideSpeed = 15f;     // 左右移动的灵敏度
    public float roadWidth = 9f;      // 路面半宽（根据你20宽的路，设为9比较安全）

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // 物理移动建议放在 FixedUpdate 中
    void FixedUpdate()
    {
        // 1. 自动向前移动
        Vector3 forwardMove = transform.forward * forwardSpeed * Time.fixedDeltaTime;

        // 2. 左右移动 (支持 A/D 键和 左右方向键)
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 sideMove = transform.right * horizontalInput * sideSpeed * Time.fixedDeltaTime;

        // 计算新位置
        Vector3 newPosition = rb.position + forwardMove + sideMove;

        // 3. 限制玩家不能跑出路面
        newPosition.x = Mathf.Clamp(newPosition.x, -roadWidth, roadWidth);

        // 使用 MovePosition 保证物理碰撞生效，防止重叠/穿模
        rb.MovePosition(newPosition);
    }
}