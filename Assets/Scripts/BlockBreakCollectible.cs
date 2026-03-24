using UnityEngine;

public class BlockBreakCollectible : MonoBehaviour
{
    [Header("Enhanced Shot")]
    public string subtitleText = "AMMO ENHANCED";
    public float rotateSpeed = 90f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerShooter shooter = other.GetComponent<PlayerShooter>();
        if (shooter == null && other.attachedRigidbody != null)
        {
            shooter = other.attachedRigidbody.GetComponent<PlayerShooter>();
        }
        if (shooter == null)
        {
            shooter = other.GetComponentInParent<PlayerShooter>();
        }
        if (shooter == null)
        {
            shooter = other.GetComponentInChildren<PlayerShooter>();
        }

        if (shooter != null)
        {
            shooter.EnableNextShotBlockBreak(subtitleText);
        }

        Destroy(gameObject);
    }
}
