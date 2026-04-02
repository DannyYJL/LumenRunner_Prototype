using UnityEngine;
using TMPro; // Required for TextMeshPro
using UnityEngine.UI;

public class DistanceUI : MonoBehaviour
{
    public Transform player;      // Assign Player transform in Inspector
    public Transform chaser;      // Assign Chaser transform in Inspector
    public TextMeshProUGUI distanceText; // Assign UI Text element
    public Slider distanceSlider;        // Optional: assign progress bar
    
    [Header("Settings")]
    public float dangerDistance = 5f;    // Distance threshold for danger state

    void Update()
    {
        if (player == null || chaser == null) return;

        // 1. Compute distance along the forward (Z) axis
        float distance = player.position.z - chaser.position.z - 1.2f;

        // 2. Update UI text (use "F1" for one decimal place)
        distanceText.text = "CHASER DISTANCE: " + distance.ToString("F1") + "m";

        // 3. Danger feedback: turn red and pulse when too close
        if (distance < dangerDistance)
        {
            distanceText.color = Color.red;

            // Simple pulse effect
            distanceText.transform.localScale = Vector3.one * 
                (1f + Mathf.PingPong(Time.time * 5, 0.2f));
        }
        else
        {
            distanceText.color = Color.white;
            distanceText.transform.localScale = Vector3.one;
        }

        // 4. Update progress bar (assume 20m = full)
        if (distanceSlider != null)
        {
            distanceSlider.value = Mathf.Clamp(distance / 20f, 0f, 1f);
        }
    }
}