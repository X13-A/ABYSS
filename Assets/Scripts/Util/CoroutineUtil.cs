using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CoroutineUtil : MonoBehaviour
{
    public static IEnumerator FadeTo(MeshRenderer meshRenderer, float duration, float targetAlpha, Action onComplete)
    {
        float startTime = Time.time;
        Color startColor = meshRenderer.material.color;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            meshRenderer.material.color = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, targetAlpha), t);
            yield return null;
        }
        meshRenderer.material.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
        if (onComplete != null) onComplete();
    }

    public static IEnumerator DelayAction(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
}
