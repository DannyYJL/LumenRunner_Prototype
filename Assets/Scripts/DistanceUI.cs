using UnityEngine;
using TMPro; // 必须引用
using UnityEngine.UI;

public class DistanceUI : MonoBehaviour
{
    public Transform player;      // 拖入 Player
    public Transform chaser;      // 拖入 Chaser
    public TextMeshProUGUI distanceText; // 拖入刚才创建的 Text
    public Slider distanceSlider;        // 如果有进度条，拖入这里
    
    [Header("Settings")]
    public float dangerDistance = 5f;    // 低于这个距离文字变红

    void Update()
    {
        if (player == null || chaser == null) return;

        // 1. 计算实时距离（只看 Z 轴前进方向）
        float distance = player.position.z - chaser.position.z - 1.2f;

        // 2. 更新文字显示 (取整数，保留一位小数可用 "F1")
        distanceText.text = "DISTANCE: " + distance.ToString("F1") + "m";

        // 3. 危险反馈：距离过近时变红
        if (distance < dangerDistance)
        {
            distanceText.color = Color.red;
            // 甚至可以让文字抖动（简易效果）
            distanceText.transform.localScale = Vector3.one * (1f + Mathf.PingPong(Time.time * 5, 0.2f));
        }
        else
        {
            distanceText.color = Color.white;
            distanceText.transform.localScale = Vector3.one;
        }

        // 4. 更新进度条（假设 20m 为满值）
        if (distanceSlider != null)
        {
            distanceSlider.value = Mathf.Clamp(distance / 20f, 0f, 1f);
        }
    }
}