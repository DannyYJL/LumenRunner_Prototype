using UnityEngine;

public class LightBullet : MonoBehaviour
{
    // 子弹存活时间，默认3秒
    public float lifeTime = 5f;

    void Start()
    {
        // 出生3秒后自动销毁
        Destroy(gameObject, lifeTime);
    }
}