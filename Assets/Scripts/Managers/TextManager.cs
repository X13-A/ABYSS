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
    private bool canDequeueMessage;
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
        messageQueue = new Queue();
        messageRunning = false;
        canDequeueMessage = true;
        freeze = false;
        textGui = textBubble.GetComponentInChildren<TextMeshProUGUI>();
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
        if (canDequeueMessage && !messageRunning)
        {
            if (messageQueue.Count != 0)
            {
                PerformMessageDisplay();
                canDequeueMessage = false;
            }
            else
            {
                textBubble.SetActive(false);
            }
        }
    }

    private void HandlePause(GamePauseMenuEvent e)
    {
        freeze = true;
        textBubble.SetActive(false);
    }

    private void HandleResume(GamePlayEvent e)
    {
        freeze = false;
        textBubble.SetActive(true);
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<GamePauseMenuEvent>(HandlePause);
        EventManager.Instance.AddListener<GamePlayEvent>(HandleResume);
        EventManager.Instance.AddListener<TextSkipKeyPressedEvent>(HandleSkipKeyPressed);
        EventManager.Instance.AddListener<TextEvent>(HandleTextEvent);
    }


    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<GamePauseMenuEvent>(HandlePause);
        EventManager.Instance.RemoveListener<GamePlayEvent>(HandleResume);
        EventManager.Instance.RemoveListener<TextSkipKeyPressedEvent>(HandleSkipKeyPressed);
        EventManager.Instance.RemoveListener<TextEvent>(HandleTextEvent);
    }

    private void ListenToSkipKeys()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            EventManager.Instance.Raise(new TextSkipKeyPressedEvent());
        }
    }

    private void PerformMessageDisplay()
    {
        TextEvent text = (TextEvent) messageQueue.Dequeue();
        currentText = "";
        fullText = text.text;
        delay = text.delay;
        StartCoroutine(ShowTextRoutine());
    }

    /// <summary>
    /// handle the SkipKeyPressed event,
    /// that occur when the player press space or left click,
    /// </summary>
    /// <param name="e"></param>
    private void HandleSkipKeyPressed(TextSkipKeyPressedEvent e)
    {
        canDequeueMessage = true;
    }

    private void HandleTextEvent(TextEvent e)
    {
        messageQueue.Enqueue(e);
    }

    private IEnumerator ShowTextRoutine()
    {
        messageRunning = true;
        textBubble.SetActive(true);
        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText[..i];
            textGui.text = currentText;
            yield return new WaitForSeconds(delay);
        }
        messageRunning = false;
    }
}
