using UnityEngine;

public class ChaserLogic : MonoBehaviour
{
    public Transform target;
    public float minSpeed = 7.5f;
    public float targetDistance = 15f; 
    public float k = 1.5f;            

    void Update()
    {
        if (target == null) return;

        float currentDist = target.position.z - transform.position.z;
        float currentSpeed = minSpeed;

        if (currentDist > targetDistance)
        {
            float extraSpeed = (currentDist - targetDistance) * k;
            currentSpeed += extraSpeed;
        }

        transform.position += Vector3.forward * currentSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // --- 关键修改点 ---
            // 寻找场景中的 GameManager 并调用 EndGame 方法
            GameManager gm = Object.FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.EndGame();
            }
            else
            {
                Debug.LogError("场景中没找到 GameManager！请检查是否挂载了脚本。");
            }
        }
    }
}