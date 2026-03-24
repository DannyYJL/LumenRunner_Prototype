using UnityEngine;

public class LightBullet : MonoBehaviour
{
    public float lifeTime = 5f;
    public string breakableTag = "Obstacles";

    private bool canBreakObstacle;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetBlockBreaking(bool enabled)
    {
        canBreakObstacle = enabled;
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryBreakObstacle(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryBreakObstacle(other);
    }

    private void TryBreakObstacle(Collider hitCollider)
    {
        if (!canBreakObstacle || hitCollider == null)
        {
            return;
        }

        GameObject breakTarget = ResolveBreakTarget(hitCollider);
        if (breakTarget == null)
        {
            return;
        }

        canBreakObstacle = false;
        Destroy(breakTarget);
        Destroy(gameObject);
    }

    private GameObject ResolveBreakTarget(Collider hitCollider)
    {
        if (hitCollider.CompareTag(breakableTag))
        {
            return hitCollider.gameObject;
        }

        if (hitCollider.attachedRigidbody != null && hitCollider.attachedRigidbody.CompareTag(breakableTag))
        {
            return hitCollider.attachedRigidbody.gameObject;
        }

        Transform root = hitCollider.transform.root;
        if (root != null && root.CompareTag(breakableTag))
        {
            return root.gameObject;
        }

        return null;
    }
}
