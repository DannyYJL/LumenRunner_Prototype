using UnityEngine;

public class LightBullet : MonoBehaviour
{
    public float lifeTime = 5f;
    public string obstacleTag = "Obstacles";
    public string movingObstacleTag = "Moving Obstacle";
    public int obstacleBreakScore = 50;

    private bool canBreakObstacle;
    private PlayerMovement scoreOwner;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetBlockBreaking(bool enabled)
    {
        canBreakObstacle = enabled;
    }

    public void SetScoreOwner(PlayerMovement owner)
    {
        scoreOwner = owner;
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
        if (scoreOwner != null)
        {
            scoreOwner.AddScore(obstacleBreakScore);
        }
        Destroy(breakTarget);
        Destroy(gameObject);
    }

    private GameObject ResolveBreakTarget(Collider hitCollider)
    {
        if (HasBreakableTag(hitCollider.gameObject))
        {
            return hitCollider.gameObject;
        }

        if (hitCollider.attachedRigidbody != null && HasBreakableTag(hitCollider.attachedRigidbody.gameObject))
        {
            return hitCollider.attachedRigidbody.gameObject;
        }

        Transform current = hitCollider.transform.parent;
        while (current != null)
        {
            if (HasBreakableTag(current.gameObject) && IsBreakableObject(current))
            {
                return current.gameObject;
            }

            current = current.parent;
        }

        return null;
    }

    private bool HasBreakableTag(GameObject target)
    {
        return target.CompareTag(obstacleTag) || target.CompareTag(movingObstacleTag);
    }

    private bool IsBreakableObject(Transform target)
    {
        return target.GetComponent<Collider>() != null
            || target.GetComponent<Rigidbody>() != null
            || target.GetComponent<Renderer>() != null;
    }
}
