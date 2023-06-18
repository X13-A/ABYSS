using SDD.Events;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour, IEventHandler
{

    private static TextManager m_Instance;
    public static TextManager Instance => m_Instance;

    [SerializeField] private GameObject textBubble;
    public bool MessageActive => textBubble.activeSelf;
    private TextMeshProUGUI textGui;
    private string fullText;
    private string currentText;
    private float delay;
    private Queue messageQueue;
    private bool messageRunning;
    private bool skipRequested;
    private bool fastForwadRequested;
    private bool freeze;

    private float lastInteractionTime;
    public float TimeSinceLastInteraction => Time.time - lastInteractionTime;
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
        lastInteractionTime = Time.time - 1000;
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
            // Hack: should create proper event but that should work
            EventManager.Instance.Raise(new PlayButtonClickedEvent { });
            textBubble.SetActive(false);
            skipRequested = false;
        }
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<EscapeButtonClickedEvent>(HandlePause);
        EventManager.Instance.AddListener<ResumeButtonClickedEvent>(HandleResume);
        EventManager.Instance.AddListener<GameOverEvent>(HandleGameOver);
        EventManager.Instance.AddListener<MessageEvent>(HandleTextEvent);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<EscapeButtonClickedEvent>(HandlePause);
        EventManager.Instance.RemoveListener<ResumeButtonClickedEvent>(HandleResume);
        EventManager.Instance.RemoveListener<GameOverEvent>(HandleGameOver);
        EventManager.Instance.RemoveListener<MessageEvent>(HandleTextEvent);
    }

    private void HandlePause(EscapeButtonClickedEvent e)
    {
        textBubble.SetActive(false);
        freeze = true;
    }

    private void HandleResume(ResumeButtonClickedEvent e)
    {
        if (messageQueue.Count > 0 && messageRunning)
        {
            textBubble.SetActive(true);
            EventManager.Instance.Raise(new TextBubbleActiveEvent { });
        }
        freeze = false;
    }

    private void HandleGameOver(GameOverEvent e)
    {
        textBubble.SetActive(false);
        freeze = true;
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
        if (!textBubble.activeSelf) return;
        if (TimeSinceLastInteraction < 0.25f) return;

        // if a message is currently running, fast forward it, otherwise skip it
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) && (messageRunning))
        {
            lastInteractionTime = Time.time;
            fastForwadRequested = true;
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            lastInteractionTime = Time.time;
            skipRequested = true;
            return;
        }
    }

    private void PerformMessageDisplay(MessageEvent text)
    {
        EventManager.Instance.Raise(new TextBubbleActiveEvent { });
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
            // Fast forward text
            if (fastForwadRequested == true)
            {
                fastForwadRequested = false;
                currentText = fullText;
                textGui.text = currentText;
                messageRunning = false;
                yield break;
            }
            // Display text slowly
            else
            {
                currentText = fullText[..i];
                textGui.text = currentText;
                yield return new WaitForSeconds(delay);
            }
        }
        messageRunning = false;
    }
}
