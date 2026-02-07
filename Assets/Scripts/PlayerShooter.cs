using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("必须设置")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Camera mainCamera;

    [Header("参数调整")]
    public float shootForce = 50f;
    
    // 【新增】射击间隔时间（秒）
    public float fireRate = 2f; 

    // 【新增】记录下一次允许开火的时间点
    private float nextFireTime = 0f;

    void Update()
    {
        // 1. 检测鼠标点击
        // 2. 并且检测：当前时间 (Time.time) 是否已经超过了 下一次允许开火的时间 (nextFireTime)
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Shoot();
            
            // 【关键】开火后，设置下一次允许开火的时间 = 当前时间 + 间隔时间
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
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
        
        // 确保子弹还是用 LightBullet 脚本里的生命周期
        // (不需要改 LightBullet 脚本，它只需要负责销毁自己)
        
        currentBullet.GetComponent<Rigidbody>().linearVelocity = direction.normalized * shootForce; // Unity 6 用 linearVelocity，旧版用 velocity
    }
}