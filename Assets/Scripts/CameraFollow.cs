using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // 在 Inspector 中把 Player 拖到这里
    public Vector3 offset = new Vector3(0, 5, -10); // 相机与玩家的相对距离
    public float smoothSpeed = 0.125f; // 平滑系数，越小越平滑

    // 注意这里改成了 LateUpdate
    void LateUpdate()
    {
        if (target != null)
        {
            // 1. 计算目标应该在的位置
            Vector3 desiredPosition = target.position + offset;

            // 2. 使用 Lerp 让相机平滑地“飘”向目标位置，而不是瞬间移动
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // 3. 更新相机坐标
            transform.position = smoothedPosition;

            // 4. 让相机始终盯着玩家看
            transform.LookAt(target);
        }
    }
}