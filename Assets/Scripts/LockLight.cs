using UnityEngine;

public class LockLightY : MonoBehaviour
{
    public Transform player; // 拖入 Player 物体
    public float yOffset = 0.5f; // 光源距离地面的固定高度

    void LateUpdate()
    {
        if (player == null) return;

        // 保持光源当前的 X 和 Z（随 Chaser 移动）
        // 但将 Y 轴强制设置为玩家当前的 Y 轴高度 + 偏移量
        Vector3 newPosition = transform.position;
        newPosition.y = player.position.y + yOffset;
        
        transform.position = newPosition;
    }
}