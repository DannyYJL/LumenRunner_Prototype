using UnityEngine;

public class AmmoBoostCollectible : MonoBehaviour
{
    public int scoreValue = 50;

    [Header("Ammo Refill")]
    public int refillAmount = 2;

    [Header("Temporary Boost")]
    [Tooltip("fireRate multiplier: 0.5 = shoot twice as fast, 1 = no change")]
    public float fireRateMultiplier = 0.6f;

    [Tooltip("reloadTime multiplier: 0.5 = reload twice as fast, 1 = no change")]
    public float reloadTimeMultiplier = 0.7f;

    public float duration = 5f;

    public float rotateSpeed = 90f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerShooter shooter = other.GetComponent<PlayerShooter>();
        if (shooter == null)
        {
            shooter = other.GetComponentInParent<PlayerShooter>();
        }

        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            playerMovement = other.GetComponentInParent<PlayerMovement>();
        }

        if (shooter != null)
        {
            // instant refill
            if (refillAmount > 0)
                shooter.RefillAmmo(refillAmount);

            // temporary boost
            if (duration > 0f)
                shooter.ApplyBoost(fireRateMultiplier, reloadTimeMultiplier, duration);
        }

        if (playerMovement != null)
        {
            playerMovement.AddScore(scoreValue);
        }

        Destroy(gameObject);
    }
}
