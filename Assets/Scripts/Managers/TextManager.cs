using SDD.Events;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour, IEventHandler
{
    private static TextManager m_Instance;
    public TextManager instance => m_Instance;

    [SerializeField] private TextMeshProUGUI textGui;

    private string fullText;
    private string currentText;
    private float delay;

    private void Awake()
    {
        if (!m_Instance)
        {
            m_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SubscribeEvents();

    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<TextEvent>(HandleTextEvent);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<TextEvent>(HandleTextEvent);
    }

    private void HandleTextEvent(TextEvent e)
    {
        fullText = e.text;
        delay = e.duration;
        currentText = "";
        StartCoroutine(ShowTextRoutine());
    }

    private IEnumerator ShowTextRoutine()
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText[..i];
            textGui.text = currentText;
            yield return new WaitForSeconds(delay);
        }
    }
}
