using UnityEngine;

public class AllBrightPowerUp : MonoBehaviour
{
    public float rotateSpeed = 90f;
    public float duration = 3f;
    public float exposureBoost = 0.45f;
    public float ambientIntensityMultiplier = 1.2f;
    public float lightIntensityMultiplier = 1.15f;
    public Color flashLightTint = new Color(1f, 0.96f, 0.84f);
    public int scoreValue = 100;

    private bool isActivated;

    void Update()
    {
        if (!isActivated)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActivated || !other.CompareTag("Player"))
        {
            return;
        }

        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            playerMovement = other.GetComponentInParent<PlayerMovement>();
        }

        if (playerMovement != null)
        {
            playerMovement.AddScore(scoreValue);
        }

        isActivated = true;
        MapFlashCollectible.TriggerFlash(
            duration,
            exposureBoost,
            ambientIntensityMultiplier,
            lightIntensityMultiplier,
            flashLightTint);
        Destroy(gameObject);
    }
}
