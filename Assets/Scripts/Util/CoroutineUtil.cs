using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class CoroutineUtil : MonoBehaviour
{
    private static CoroutineUtil m_Instance;
    public static CoroutineUtil Instance { get { return m_Instance; } }

    private void Awake()
    {
        if (!m_Instance) m_Instance = this;
        else Destroy(gameObject);
    }

    // Allows coroutines to be started from a permanent object
    public Coroutine StartPermanentCoroutine(IEnumerator coroutine)
    {
        return m_Instance.StartCoroutine(coroutine);
    }

    public static IEnumerator FadeTo(MeshRenderer meshRenderer, float duration, float targetAlpha, Action onComplete = null)
    {
        float startTime = Time.time;
        Color startColor = meshRenderer.material.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        Material material = ConvertMaterialToTransparent(meshRenderer.material);
        meshRenderer.material = material;

        while (Time.time - startTime < duration)
        {
            if (meshRenderer == null) yield break; // Important, prevents crash if object is destroyed during coroutine
            float t = (Time.time - startTime) / duration;
            meshRenderer.material.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        if (meshRenderer != null)
        {
            meshRenderer.material.color = targetColor;
            if (onComplete != null) onComplete();
        }
    }

    private static Material ConvertMaterialToTransparent(Material material)
    {
        Material clonedMaterial = new Material(material);
        clonedMaterial.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
        clonedMaterial.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        clonedMaterial.SetInt("_ZWrite", 0);
        clonedMaterial.DisableKeyword("_ALPHATEST_ON");
        clonedMaterial.EnableKeyword("_ALPHABLEND_ON");
        clonedMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        clonedMaterial.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
        return clonedMaterial;
    }


    public static IEnumerator FadeTo(SkinnedMeshRenderer skinnedMeshRenderer, float duration, float targetAlpha, Action onComplete = null)
    {
        float startTime = Time.time;
        Color startColor = skinnedMeshRenderer.material.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (Time.time - startTime < duration)
        {
            if (skinnedMeshRenderer == null) yield break; // Important, prevents crash if object is destroyed during coroutine
            float t = (Time.time - startTime) / duration;
            skinnedMeshRenderer.material.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.material.color = targetColor;
            if (onComplete != null) onComplete();
        }
    }

    public static IEnumerator FadeTextTo(TextMeshPro text, float duration, float targetAlpha, Action onComplete = null)
    {
        float startTime = Time.time;
        Color startColor = text.material.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (Time.time - startTime < duration)
        {
            if (text == null) yield break;
            float t = (Time.time - startTime) / duration;
            text.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        if (text != null)
        {
            text.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
            if (onComplete != null) onComplete();
        }
    }

    public static IEnumerator FadeUITextTo(TextMeshProUGUI text, float duration, float targetAlpha, Action onComplete = null)
    {
        float startTime = Time.time;
        Color startColor = text.material.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (Time.time - startTime < duration)
        {
            if (text == null) yield break;
            float t = (Time.time - startTime) / duration;
            text.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        if (text != null)
        {
            text.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
            if (onComplete != null) onComplete();
        }
    }

    public static IEnumerator ScaleTo(Transform transform, float duration, Vector3 targetScale, Action onComplete = null)
    {
        float startTime = Time.time;
        Vector3 startScale = transform.localScale;

        while (Time.time - startTime < duration)
        {
            if (transform == null) yield break;
            float t = (Time.time - startTime) / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        if (transform != null)
        {
            transform.localScale = targetScale;
            if (onComplete != null) onComplete();
        }
    }
    public static IEnumerator RotateTo(Transform transform, float duration, Quaternion targetRotation, Action onComplete = null)
    {
        float startTime = Time.time;
        Quaternion startRotation = transform.localRotation;
        while (Time.time - startTime < duration)
        {
            if (transform == null) yield break;
            float t = (Time.time - startTime) / duration;
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }
        if (transform != null)
        {
            transform.localRotation = targetRotation;
            if (onComplete != null) onComplete();
        }
    }

    public static IEnumerator DelayAction(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

}
