using UnityEngine;

public class TwoColumnArrowFlow : MonoBehaviour
{
    [Header("Assign arrow ROOT objects from bottom to top")]
    public GameObject[] leftColumn;
    public GameObject[] rightColumn;

    [Header("Timing")]
    public float stepDuration = 0.3f;   // 每个箭头亮多久
    public float rightColumnDelay = 0f; // 右列延迟，0表示两列同步

    [Header("Emission Settings")]
    public float offIntensity = 0f;
    public float onIntensity = 8f;

    private void Update()
    {
        AnimateColumn(leftColumn, 0f);
        AnimateColumn(rightColumn, rightColumnDelay);
    }

    void AnimateColumn(GameObject[] column, float delay)
    {
        if (column == null || column.Length == 0 || stepDuration <= 0f) return;

        float shiftedTime = Time.time - delay;
        if (shiftedTime < 0f)
        {
            SetAllArrowsIntensity(column, offIntensity);
            return;
        }

        int activeIndex = Mathf.FloorToInt(shiftedTime / stepDuration) % column.Length;
        if (activeIndex < 0) activeIndex += column.Length;

        for (int i = 0; i < column.Length; i++)
        {
            float targetIntensity = (i == activeIndex) ? onIntensity : offIntensity;
            SetArrowIntensity(column[i], targetIntensity);
        }
    }

    void SetAllArrowsIntensity(GameObject[] column, float intensity)
    {
        foreach (GameObject arrow in column)
        {
            SetArrowIntensity(arrow, intensity);
        }
    }

    void SetArrowIntensity(GameObject arrowRoot, float intensity)
    {
        if (arrowRoot == null) return;

        Renderer[] renderers = arrowRoot.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            Material mat = r.material;

            Color baseEmission = mat.GetColor("_EmissionColor");
            Color normalizedBase = baseEmission.maxColorComponent > 0f
                ? baseEmission / baseEmission.maxColorComponent
                : Color.white;

            mat.SetColor("_EmissionColor", normalizedBase * intensity);
        }
    }
}