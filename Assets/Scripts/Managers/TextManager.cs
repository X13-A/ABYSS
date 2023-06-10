using SDD.Events;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour, IEventHandler
{

    private static TextManager m_Instance;
    public TextManager instance => m_Instance;

    [SerializeField] private GameObject textBubble;

    private TextMeshProUGUI textGui;
    private string fullText;
    private string currentText;
    private float delay;
    private Queue messageQueue;
    private bool messageRunning;
    private bool skipRequested;
    private bool freeze;

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
        textGui = textBubble.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        messageQueue = new Queue();
        messageRunning = false;
        skipRequested = false;
        freeze = false;
        textBubble.SetActive(false);
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void Update()
    {
        if (freeze)
        {
            return;
        }
        ListenToSkipKeys();
        if (CanDequeueMessage())
        {
            PerformMessageDisplay((MessageEvent) messageQueue.Dequeue());
        }
        // last message has been displayed -> wait for the player to press skip keys
        if (messageQueue.Count == 0 && !messageRunning && skipRequested)
        {
            textBubble.SetActive(false);
            skipRequested = false;
        }
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<EscapeButtonClickedEvent>(HandlePause);
        EventManager.Instance.AddListener<ResumeButtonClickedEvent>(HandleResume);
        EventManager.Instance.AddListener<MessageEvent>(HandleTextEvent);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<EscapeButtonClickedEvent>(HandlePause);
        EventManager.Instance.RemoveListener<ResumeButtonClickedEvent>(HandleResume);
        EventManager.Instance.RemoveListener<MessageEvent>(HandleTextEvent);
    }

    private void HandlePause(EscapeButtonClickedEvent e)
    {
        textBubble.SetActive(false);
        freeze = true;
    }

    private void HandleResume(ResumeButtonClickedEvent e)
    {
        textBubble.SetActive(true);
        freeze = false;
    }

    /// <summary>
    /// return true if we can dequeue a message, in one of these two cases : 
    /// 1 : there was a message before, it is over and the user has requested text skip,
    /// 2 : there was no message before
    /// </summary>
    /// <returns></returns>
    private bool CanDequeueMessage()
    {
        if (messageQueue.Count == 0)
        {
            return false;
        }
        return (!messageRunning && skipRequested) || !textBubble.activeSelf;
    }

    private void ListenToSkipKeys()
    {
        // if a message is currently running, we don't want a user to skip it
        if (!textBubble.activeSelf || messageRunning)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            skipRequested = true;
        }
    }

    private void PerformMessageDisplay(MessageEvent text)
    {
        skipRequested = false;
        currentText = "";
        fullText = text.text;
        delay = text.delay;
        ContextEnableTextBubble();
        _ = StartCoroutine(ShowTextRoutine());
    }

    private void ContextEnableTextBubble()
    {
        if (!textBubble.activeSelf)
        {
            textBubble.SetActive(true);
        }
    }

    private void HandleTextEvent(MessageEvent e) => messageQueue.Enqueue(e);

    private IEnumerator ShowTextRoutine()
    {
        messageRunning = true;
        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText[..i];
            textGui.text = currentText;
            yield return new WaitForSeconds(delay);
        }
        messageRunning = false;
    }
}
