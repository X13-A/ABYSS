using UnityEngine;
using System.Collections;

public class SkyboxColorChange : MonoBehaviour
{
    [SerializeField] private float targetFogDensity = 0.0f;
    [SerializeField] private float duration = 10f;
    [SerializeField] private Light targetLight;
    [SerializeField] private float targetIntensity;

    [SerializeField] private MeshRenderer fakeSky;

    private void Start()
    {
        StartCoroutine(ChangeLightIntensity());
        StartCoroutine(ChangeFogDensity());
        StartCoroutine(CoroutineUtil.FadeTo(fakeSky, duration, targetFogDensity));
    }
    private IEnumerator ChangeLightIntensity()
    {
        float startIntensity = targetLight.intensity;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp(elapsed / duration, 0f, 1f);
            targetLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, normalizedTime);
            yield return null;
        }

        targetLight.intensity = targetIntensity;
    }
    private IEnumerator ChangeFogDensity()
    {
        float startFogDensity = RenderSettings.fogDensity;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp(elapsed / duration, 0f, 1f);
            RenderSettings.fogDensity = Mathf.Lerp(startFogDensity, targetFogDensity, normalizedTime);
            yield return null;
        }

        RenderSettings.fogDensity = targetFogDensity;
    }
}
