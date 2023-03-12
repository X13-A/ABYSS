using SDD.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditorInternal.VersionControl.ListControl;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject fadePanel;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        StartCoroutine(FadeIn(1f, () => {}));
    }
    private void OnEnable()
    {

    }
    public IEnumerator FadeIn(float fadeDuration = 1f, Action actionDelegate = null)
    {
        // Instantiate a new panel
        GameObject panel = new GameObject("FadePanel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvas.transform);

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        rectTransform.anchoredPosition = Vector2.zero;

        // Set the panel color to transparent
        Image panelImage = panel.GetComponent<Image>();
        Color panelColor = Color.black;
        panelColor.a = 1f;
        panelImage.color = panelColor;

        // Fade the panel to black
        float fadeTime = 0f;
        while (fadeTime < fadeDuration)
        {
            panelColor.a = Mathf.Lerp(1f, 0f, fadeTime / fadeDuration);
            panelImage.color = panelColor;
            fadeTime += Time.deltaTime;
            yield return null;
        }

        panelColor.a = 0f;
        panelImage.color = panelColor;

        // Call the action delegate
        actionDelegate?.Invoke();

        // Destroy the panel
        Destroy(panel);
    }

    public IEnumerator FadeOut(float fadeDuration = 1f, Action actionDelegate = null)
    {
        // Set the panel color to transparent
        Image panelImage = fadePanel.GetComponent<Image>();
        Color panelColor = panelImage.color;
        panelColor.a = 0f;
        panelImage.color = panelColor;

        // Fade the panel to black
        float fadeTime = 0f;
        while (fadeTime < fadeDuration)
        {
            panelColor.a = Mathf.Lerp(0f, 1f, fadeTime / fadeDuration);
            panelImage.color = panelColor;
            fadeTime += Time.deltaTime;
            yield return null;
        }

        panelColor.a = 1f;
        panelImage.color = panelColor;

        // Call the action delegate
        actionDelegate?.Invoke();
    }
}
