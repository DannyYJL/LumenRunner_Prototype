using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MapFlashCollectible : MonoBehaviour
{
    [Header("Pickup")]
    public string playerTag = "Player";
    public float rotateSpeed = 90f;
    public int scoreValue = 100;

    [Header("Flash Settings")]
    public float duration = 3f;
    public float exposureBoost = 0.45f;
    public float ambientIntensityMultiplier = 1.2f;
    public float lightIntensityMultiplier = 1.15f;
    public Color flashLightTint = new Color(1f, 0.96f, 0.84f);

    private static Coroutine activeRoutine;
    private static FlashState activeFlashState;

    private bool isActivated;
    private readonly List<Renderer> cachedRenderers = new List<Renderer>();
    private readonly List<Collider> cachedColliders = new List<Collider>();

    void Awake()
    {
        cachedRenderers.AddRange(GetComponentsInChildren<Renderer>(true));
        cachedColliders.AddRange(GetComponentsInChildren<Collider>(true));
    }

    void Update()
    {
        if (!isActivated)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActivated || !other.CompareTag(playerTag))
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
        StartFlash();
        SetPickupVisible(false);
        Destroy(gameObject);
    }

    private void StartFlash()
    {
        TriggerFlash(duration, exposureBoost, ambientIntensityMultiplier, lightIntensityMultiplier, flashLightTint);
    }

    public static void TriggerFlash(
        float flashDuration,
        float flashExposureBoost,
        float flashAmbientMultiplier,
        float flashLightMultiplier,
        Color flashTint)
    {
        MapFlashRunner runner = MapFlashRunner.Instance;
        if (runner == null)
        {
            return;
        }

        if (activeFlashState == null)
        {
            activeFlashState = FlashState.Capture();
        }

        activeFlashState.Apply(flashExposureBoost, flashAmbientMultiplier, flashLightMultiplier, flashTint);

        if (activeRoutine != null)
        {
            runner.StopCoroutine(activeRoutine);
        }

        activeRoutine = runner.StartCoroutine(FlashCountdown(flashDuration));
    }

    private static IEnumerator FlashCountdown(float flashDuration)
    {
        yield return new WaitForSeconds(flashDuration);

        activeFlashState?.Restore();
        activeFlashState = null;
        activeRoutine = null;
    }

    private void SetPickupVisible(bool isVisible)
    {
        for (int i = 0; i < cachedRenderers.Count; i++)
        {
            if (cachedRenderers[i] != null)
            {
                cachedRenderers[i].enabled = isVisible;
            }
        }

        for (int i = 0; i < cachedColliders.Count; i++)
        {
            if (cachedColliders[i] != null)
            {
                cachedColliders[i].enabled = isVisible;
            }
        }
    }

    private sealed class FlashState
    {
        private readonly Volume volume;
        private readonly ColorAdjustments colorAdjustments;
        private readonly float originalExposure;
        private readonly bool originalExposureOverride;
        private readonly float originalAmbientIntensity;
        private readonly Light[] sceneLights;
        private readonly float[] originalLightIntensities;
        private readonly Color[] originalLightColors;

        private FlashState(
            Volume volume,
            ColorAdjustments colorAdjustments,
            float originalExposure,
            bool originalExposureOverride,
            float originalAmbientIntensity,
            Light[] sceneLights,
            float[] originalLightIntensities,
            Color[] originalLightColors)
        {
            this.volume = volume;
            this.colorAdjustments = colorAdjustments;
            this.originalExposure = originalExposure;
            this.originalExposureOverride = originalExposureOverride;
            this.originalAmbientIntensity = originalAmbientIntensity;
            this.sceneLights = sceneLights;
            this.originalLightIntensities = originalLightIntensities;
            this.originalLightColors = originalLightColors;
        }

        public static FlashState Capture()
        {
            Volume volume = Object.FindFirstObjectByType<Volume>();
            ColorAdjustments colorAdjustments = null;
            float originalExposure = 0f;
            bool originalExposureOverride = false;

            if (volume != null && volume.profile != null && volume.profile.TryGet(out colorAdjustments))
            {
                originalExposure = colorAdjustments.postExposure.value;
                originalExposureOverride = colorAdjustments.postExposure.overrideState;
            }

            float originalAmbientIntensity = RenderSettings.ambientIntensity;
            Light[] sceneLights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
            float[] originalLightIntensities = new float[sceneLights.Length];
            Color[] originalLightColors = new Color[sceneLights.Length];

            for (int i = 0; i < sceneLights.Length; i++)
            {
                originalLightIntensities[i] = sceneLights[i].intensity;
                originalLightColors[i] = sceneLights[i].color;
            }

            return new FlashState(
                volume,
                colorAdjustments,
                originalExposure,
                originalExposureOverride,
                originalAmbientIntensity,
                sceneLights,
                originalLightIntensities,
                originalLightColors);
        }

        public void Apply(float exposureBoost, float ambientMultiplier, float lightMultiplier, Color flashTint)
        {
            if (colorAdjustments != null)
            {
                colorAdjustments.postExposure.overrideState = true;
                colorAdjustments.postExposure.value = originalExposure + exposureBoost;
            }

            RenderSettings.ambientIntensity = originalAmbientIntensity * ambientMultiplier;

            for (int i = 0; i < sceneLights.Length; i++)
            {
                if (sceneLights[i] == null)
                {
                    continue;
                }

                sceneLights[i].intensity = originalLightIntensities[i] * lightMultiplier;
                sceneLights[i].color = Color.Lerp(originalLightColors[i], flashTint, 0.35f);
            }
        }

        public void Restore()
        {
            RenderSettings.ambientIntensity = originalAmbientIntensity;

            if (colorAdjustments != null)
            {
                colorAdjustments.postExposure.value = originalExposure;
                colorAdjustments.postExposure.overrideState = originalExposureOverride;
            }

            for (int i = 0; i < sceneLights.Length; i++)
            {
                if (sceneLights[i] == null)
                {
                    continue;
                }

                sceneLights[i].intensity = originalLightIntensities[i];
                sceneLights[i].color = originalLightColors[i];
            }
        }
    }

    private sealed class MapFlashRunner : MonoBehaviour
    {
        private static MapFlashRunner instance;

        public static MapFlashRunner Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject runnerObject = new GameObject("MapFlashRunner");
                    instance = runnerObject.AddComponent<MapFlashRunner>();
                }

                return instance;
            }
        }
    }
}
