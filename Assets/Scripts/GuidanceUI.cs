using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // 必须保留

[RequireComponent(typeof(TextMeshProUGUI))]
public class GuidanceUI : MonoBehaviour
{
    public float fadeDuration = 1.0f;
    private TextMeshProUGUI guidanceText;
    private bool hasStartedFading = false;

    void Start()
    {
        guidanceText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // 使用新输入系统的检测方式：检测键盘是否有任何键按下
        if (!hasStartedFading && Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            hasStartedFading = true;
            StartCoroutine(FadeOutRoutine());
        }
    }

    private IEnumerator FadeOutRoutine()
    {
        if (guidanceText == null) yield break;

        Color originalColor = guidanceText.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeDuration);
            guidanceText.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);
            yield return null;
        }

        guidanceText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        gameObject.SetActive(false);
    }
}