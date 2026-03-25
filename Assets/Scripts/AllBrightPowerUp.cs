using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AllBrightPowerUp : MonoBehaviour
{
    public float rotateSpeed = 100f;
    public float duration = 5f;
    public float brightExposure = 3.0f;

    private Volume globalVolume;
    private ColorAdjustments colorAdjustments;
    private float normalExposure;
    private bool isActivated = false;

    void Start()
    {
        globalVolume = GameObject.FindObjectOfType<Volume>();
        if (globalVolume != null && globalVolume.profile.TryGet(out colorAdjustments))
        {
            normalExposure = colorAdjustments.postExposure.value;
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;
            StartCoroutine(FlashEnvironment());
        }
    }

    IEnumerator FlashEnvironment()
    {
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>()) mr.enabled = false;
        if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;

        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = brightExposure;
            Camera.main.backgroundColor = Color.white;
        }

        yield return new WaitForSeconds(duration);

        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = normalExposure;
            Camera.main.backgroundColor = Color.black;
        }

        Destroy(gameObject);
    }
}