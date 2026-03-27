using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerShooter : MonoBehaviour
{
    private float baseFireRate;
    private float baseReloadTime;
    private Coroutine boostCoroutine;
    private Coroutine subtitleCoroutine;
    private TextMeshProUGUI subtitleText;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI timerText;
    private bool hasEnhancedShot;
    private PlayerMovement playerMovement;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Camera mainCamera;
    public float shootForce = 50f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Header("Ammo Settings")]
    public int maxAmmo = 5;
    public float reloadTime = 2.5f;
    private int currentAmmo;
    private bool isReloading = false;

    [Header("UI References")]
    public TextMeshProUGUI ammoText;

    [Header("Pickup Subtitle")]
    public string enhancedAmmoSubtitle = "AMMO ENHANCED";
    public float subtitleDuration = 1.5f;

    void Start()
    {
        currentAmmo = maxAmmo;
        baseFireRate = fireRate;
        baseReloadTime = Mathf.Max(0.1f, reloadTime);
        playerMovement = GetComponent<PlayerMovement>();
        UpdateAmmoUI();
        EnsureScoreboardTexts();
        UpdateScoreboardUI();
    }

    void Update()
    {
        UpdateScoreboardUI();

        if (isReloading) return;

        if (currentAmmo <= 0 || (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        bool useEnhancedShot = hasEnhancedShot;
        hasEnhancedShot = false;

        currentAmmo--;
        UpdateAmmoUI();
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 targetPoint = Physics.Raycast(ray, out hit) ? hit.point : ray.GetPoint(100);
        Vector3 direction = targetPoint - firePoint.position;
        GameObject currentBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        LightBullet lightBullet = currentBullet.GetComponent<LightBullet>();
        if (lightBullet != null)
        {
            lightBullet.SetBlockBreaking(useEnhancedShot);
            lightBullet.SetScoreOwner(playerMovement);
        }
        currentBullet.GetComponent<Rigidbody>().linearVelocity = direction.normalized * shootForce;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        UpdateAmmoUI();

        yield return new WaitForSecondsRealtime(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoUI();
    }

    public void RefillAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = isReloading ? "RELOADING..." : currentAmmo + " / " + maxAmmo;
        }
    }

    public void ApplyBoost(float fireRateMultiplier, float reloadTimeMultiplier, float duration)
    {
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
            ResetBoost();
        }
        boostCoroutine = StartCoroutine(BoostRoutine(fireRateMultiplier, reloadTimeMultiplier, duration));
    }

    private IEnumerator BoostRoutine(float fireRateMultiplier, float reloadTimeMultiplier, float duration)
    {
        fireRate = Mathf.Max(0.01f, baseFireRate * fireRateMultiplier);
        reloadTime = Mathf.Max(0.1f, baseReloadTime * reloadTimeMultiplier);
        yield return new WaitForSecondsRealtime(duration);
        ResetBoost();
        boostCoroutine = null;
    }

    private void ResetBoost()
    {
        fireRate = baseFireRate;
        reloadTime = baseReloadTime;
    }

    public void EnableNextShotBlockBreak(string subtitleOverride = null)
    {
        hasEnhancedShot = true;
        ShowSubtitle(string.IsNullOrWhiteSpace(subtitleOverride) ? enhancedAmmoSubtitle : subtitleOverride);
    }

    private void ShowSubtitle(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        EnsureSubtitleText();
        if (subtitleText == null)
        {
            return;
        }

        subtitleText.text = message;

        if (subtitleCoroutine != null)
        {
            StopCoroutine(subtitleCoroutine);
        }

        subtitleCoroutine = StartCoroutine(SubtitleRoutine());
    }

    private void EnsureSubtitleText()
    {
        if (subtitleText != null || ammoText == null)
        {
            return;
        }

        subtitleText = Instantiate(ammoText, ammoText.transform.parent);
        subtitleText.name = "RuntimeSubtitleText";
        subtitleText.alignment = TextAlignmentOptions.Center;
        subtitleText.fontSize = Mathf.Max(24f, ammoText.fontSize * 0.7f);
        subtitleText.raycastTarget = false;
        subtitleText.text = string.Empty;

        RectTransform ammoRect = ammoText.rectTransform;
        RectTransform subtitleRect = subtitleText.rectTransform;
        subtitleRect.anchorMin = ammoRect.anchorMin;
        subtitleRect.anchorMax = ammoRect.anchorMax;
        subtitleRect.pivot = ammoRect.pivot;
        subtitleRect.sizeDelta = new Vector2(Mathf.Max(ammoRect.sizeDelta.x, 360f), ammoRect.sizeDelta.y);
        subtitleRect.anchoredPosition = ammoRect.anchoredPosition + new Vector2(0f, 40f);

        Color subtitleColor = subtitleText.color;
        subtitleText.color = new Color(subtitleColor.r, subtitleColor.g, subtitleColor.b, 0f);
    }

    private IEnumerator SubtitleRoutine()
    {
        Color baseColor = subtitleText.color;
        subtitleText.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);

        yield return new WaitForSecondsRealtime(subtitleDuration);

        float fadeTime = 0.25f;
        float elapsed = 0f;

        while (elapsed < fadeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            subtitleText.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }

        subtitleText.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        subtitleCoroutine = null;
    }

    private void EnsureScoreboardTexts()
    {
        if (ammoText == null || scoreText != null || timerText != null)
        {
            return;
        }

        scoreText = Instantiate(ammoText, ammoText.transform.parent);
        scoreText.name = "RuntimeScoreText";
        scoreText.alignment = TextAlignmentOptions.Left;
        scoreText.raycastTarget = false;

        timerText = Instantiate(ammoText, ammoText.transform.parent);
        timerText.name = "RuntimeTimerText";
        timerText.alignment = TextAlignmentOptions.Left;
        timerText.raycastTarget = false;

        ConfigureHudText(scoreText.rectTransform, new Vector2(20f, -20f));
        ConfigureHudText(timerText.rectTransform, new Vector2(20f, -52f));
    }

    private void ConfigureHudText(RectTransform textRect, Vector2 anchoredPosition)
    {
        textRect.anchorMin = new Vector2(0f, 1f);
        textRect.anchorMax = new Vector2(0f, 1f);
        textRect.pivot = new Vector2(0f, 1f);
        textRect.anchoredPosition = anchoredPosition;
        textRect.sizeDelta = new Vector2(320f, textRect.sizeDelta.y);
    }

    private void UpdateScoreboardUI()
    {
        if (playerMovement == null || scoreText == null || timerText == null)
        {
            return;
        }

        scoreText.text = "SCORE: " + playerMovement.Score;
        timerText.text = "TIME: " + playerMovement.GetFormattedElapsedTime();
    }
}
