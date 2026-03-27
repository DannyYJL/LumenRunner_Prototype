using UnityEngine;

public class CubeCollectible : MonoBehaviour
{
    public float rotateSpeed = 90f;
    public int value = 1;
    public string playerTag = "Player";

    void Update()
    {
        // rotate cube
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        // Shared pickup prefabs can carry a specialized collectible script too.
        // In those cases, let the specialized script own the score award.
        if (GetComponent<AmmoBoostCollectible>() != null ||
            GetComponent<MapFlashCollectible>() != null ||
            GetComponent<BlockBreakCollectible>() != null ||
            GetComponent<AllBrightPowerUp>() != null)
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
            playerMovement.AddScore(value);
        }

        Destroy(gameObject);
    }
}
