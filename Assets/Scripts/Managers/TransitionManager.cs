using SDD.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditorInternal.VersionControl.ListControl;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager m_Instance;
    public static TransitionManager Instance => m_Instance;

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject fadePanelPrefab;
    private GameObject fadePanel;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        StartCoroutine(FadeIn(1f, () => { }));
    }
    public IEnumerator FadeIn(float fadeDuration = 1f, Action actionDelegate = null)
    {
        CreateFadePanel();

        // Set the panel color to transparent
        Image panelImage = fadePanel.GetComponent<Image>();
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
        Destroy(fadePanel);
    }

    private void CreateFadePanel()
    {
        if (this.fadePanel != null) Destroy(this.fadePanel);
        this.fadePanel = Instantiate(this.fadePanelPrefab, this.canvas.transform);
    }

    public IEnumerator FadeOut(float fadeDuration = 1f, Action actionDelegate = null)
    {
        CreateFadePanel();

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

        Destroy(this.fadePanel);

        // Call the action delegate
        actionDelegate?.Invoke();
    }
}
