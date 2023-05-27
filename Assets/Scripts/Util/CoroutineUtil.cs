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

    // Allows coroutines to be started from a permanant object
    public Coroutine StartPermanentCoroutine(IEnumerator coroutine)
    {
        return m_Instance.StartCoroutine(coroutine);
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
