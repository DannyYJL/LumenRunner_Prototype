using UnityEngine;
using System.Collections; // 必须引用这个才能使用协程 Coroutine

public class PlayerShooter : MonoBehaviour
{
    private float baseFireRate;
    private float baseReloadTime;

    private Coroutine boostCoroutine;

    [Header("基础射击")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Camera mainCamera;
    public float shootForce = 50f;
    public float fireRate = 0.5f; // 缩短了射速，让手感更连贯
    private float nextFireTime = 0f;

    [Header("弹药管理")]
    public int maxAmmo = 5;          // 弹夹容量
    public float reloadTime = 2.5f;   // 换弹需要的秒数
    private int currentAmmo;          // 当前剩余弹药
    private bool isReloading = false; // 是否正在换弹

    void Start()
    {
        currentAmmo = maxAmmo; // 初始化时装满子弹
        baseFireRate = fireRate;
        baseReloadTime = reloadTime;
    }

    void Update()
    {
        // 如果正在换弹，则直接返回，不执行后面的射击逻辑
        if (isReloading) return;

        // 自动换弹：如果没子弹了，自动开始换弹
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        // 手动换弹：玩家按下 R 键且弹药不满时换弹
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
            return;
        }

        // 射击逻辑
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        currentAmmo--; // 每射出一发，子弹减一
        Debug.Log("剩余弹药: " + currentAmmo);

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
        
        // 保持你代码里的 linearVelocity 习惯
        currentBullet.GetComponent<Rigidbody>().linearVelocity = direction.normalized * shootForce;
    }

    // 换弹的协程（类似一个独立的倒计时器）
    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("正在换弹...");

        // 这里可以播放换弹动画或声音
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("换弹完成！");
    }

    public void RefillAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        Debug.Log("弹药补给！当前弹药: " + currentAmmo);
    }

    public void ApplyBoost(float fireRateMultiplier, float reloadTimeMultiplier, float duration)
    {
        // Stop old boost and reset first (so boosts don't stack weirdly)
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
            ResetBoost();
        }

        boostCoroutine = StartCoroutine(BoostRoutine(fireRateMultiplier, reloadTimeMultiplier, duration));
    }

    private IEnumerator BoostRoutine(float fireRateMultiplier, float reloadTimeMultiplier, float duration)
    {
        // fireRate faster
        fireRate = Mathf.Max(0.01f, baseFireRate * fireRateMultiplier);

        // reloadTime faster
        reloadTime = Mathf.Max(0.05f, baseReloadTime * reloadTimeMultiplier);

        Debug.Log($"Boost starts！fireRate={fireRate}, reloadTime={reloadTime}");

        yield return new WaitForSeconds(duration);

        ResetBoost();
        boostCoroutine = null;

        Debug.Log("Boost over, back to normal");
    }

    private void ResetBoost()
    {
        fireRate = baseFireRate;
        reloadTime = baseReloadTime;
    }

}