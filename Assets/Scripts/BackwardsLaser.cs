using UnityEngine;
using System.Collections;

public class BackwardsLaser : MonoBehaviour 
{
    [Header("Required Settings")]
    public float laserRange = 1000f;
    public LayerMask hitLayers;
    public string playerTag = "Player";
    
    [Header("Time Settings")]
    public float activeTime = 2.0f;
    public float inactiveTime = 7.0f;
    public float startDelay = 0f;
    public float extendDuration = 0.4f;
    
    [Header("Direction Settings")]
    public bool shootBackwards = true;

    private LineRenderer lineRenderer;
    private bool isLaserActive = false;
    private float activationStartTime;
    
    private GameManager gameManager;

    private bool hasKilledPlayer = false;


    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; 
        lineRenderer.enabled = false;
        
        gameManager = FindObjectOfType<GameManager>();
        
        StartCoroutine(LaserRoutine());
    }

    void Update()
    {
        if (isLaserActive)
        {
            FireLaser();
        }
    }

    IEnumerator LaserRoutine()
    {
        if (startDelay > 0) yield return new WaitForSeconds(startDelay);

        while (true)
        {
            isLaserActive = true;
            activationStartTime = Time.time;
            lineRenderer.enabled = true;
            yield return new WaitForSeconds(activeTime);

            isLaserActive = false;
            lineRenderer.enabled = false;
            yield return new WaitForSeconds(inactiveTime);
        }
    }

    void FireLaser()
    {
        Vector3 direction = shootBackwards ? -transform.forward : transform.forward;
        Vector3 startPoint = transform.position;
        lineRenderer.SetPosition(0, startPoint);

        RaycastHit hit;
        Vector3 fullEndPoint = startPoint + (direction * laserRange);
        bool hitPlayer = false;

        if (Physics.Raycast(startPoint, direction, out hit, laserRange, hitLayers))
        {
            fullEndPoint = hit.point;
            hitPlayer = hit.collider.CompareTag(playerTag);
        }

        float extendProgress = extendDuration <= 0f
            ? 1f
            : Mathf.Clamp01((Time.time - activationStartTime) / extendDuration);

        Vector3 currentEndPoint = Vector3.Lerp(startPoint, fullEndPoint, extendProgress);
        lineRenderer.SetPosition(1, currentEndPoint);

        if (hitPlayer && Vector3.Distance(startPoint, currentEndPoint) >= Vector3.Distance(startPoint, hit.point))
        {
            KillPlayer();
        }
    }

    void KillPlayer()
    {
        if (hasKilledPlayer) return; 
        hasKilledPlayer = true;
        
        Debug.Log("Player hit by laser. Triggering Game Over!");
        
        FindObjectOfType<AnalyticsUploader>()?.LogDeath(DeathCause.Laser);

        if (gameManager != null)
        {
            gameManager.EndGame();
        }
        else
        {
            Debug.LogWarning("GameManager not found! Please ensure there is a GameObject with the GameManager script in the scene.");
        }
    }
}
