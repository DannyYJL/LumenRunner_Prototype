using UnityEngine;

public class SimplePendulum : MonoBehaviour
{
    [Header("摆动设置")]
    public float angleLimit = 45f; // 摆动角度（左右各45度）
    public float speed = 2.0f;     // 摆动速度

    private Quaternion startRotation;

    void Start()
    {
        // 记录初始旋转角度
        startRotation = transform.rotation;
    }

    void Update()
    {
        // 使用正弦函数 Mathf.Sin 产生 -1 到 1 的周期变化
        float angle = Mathf.Sin(Time.time * speed) * angleLimit;

        // 应用旋转
        transform.rotation = startRotation * Quaternion.Euler(0, 0, angle);
    }
}