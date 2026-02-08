using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Camera mainCamera;

    public float shootForce = 50f;
    
    public float fireRate = 2f; 

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Shoot();
            
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
        
        
        currentBullet.GetComponent<Rigidbody>().linearVelocity = direction.normalized * shootForce; // Unity 6 用 linearVelocity，旧版用 velocity
    }
}