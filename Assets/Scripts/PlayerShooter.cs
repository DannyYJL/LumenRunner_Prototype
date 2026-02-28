using UnityEngine;
using System.Collections;
using TMPro; // Required for TextMeshPro UI

public class PlayerShooter : MonoBehaviour
{
    private float baseFireRate;
    private float baseReloadTime;
    private Coroutine boostCoroutine;

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
    public TextMeshProUGUI ammoText;      // Drag your Ammo Text here
    public GameObject reloadingPrompt;  // Drag your "Reloading" UI object here

    void Start()
    {
        currentAmmo = maxAmmo;
        baseFireRate = fireRate;
        baseReloadTime = reloadTime;

        // Initialize UI
        if (reloadingPrompt != null) reloadingPrompt.SetActive(false);
        UpdateAmmoUI();
    }

    void Update()
    {
        if (isReloading) return;

        // Auto reload when empty
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        // Manual reload
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
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
        currentAmmo--;
        UpdateAmmoUI();

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - firePoint.position;
        GameObject currentBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        currentBullet.GetComponent<Rigidbody>().linearVelocity = direction.normalized * shootForce;
    }

    IEnumerator Reload()
    {
        isReloading = true;

        if (reloadingPrompt != null) reloadingPrompt.SetActive(true);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        
        if (reloadingPrompt != null) reloadingPrompt.SetActive(false);
        
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
            ammoText.text = currentAmmo + " / " + maxAmmo;
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
        reloadTime = Mathf.Max(0.05f, baseReloadTime * reloadTimeMultiplier);

        yield return new WaitForSeconds(duration);

        ResetBoost();
        boostCoroutine = null;
    }

    private void ResetBoost()
    {
        fireRate = baseFireRate;
        reloadTime = baseReloadTime;
    }
}