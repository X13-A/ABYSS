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
    private Queue messageQueue;
    private bool messageRunning;

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
        EventManager.Instance.AddListener<TextSkipKeyPressedEvent>(HandleSkipKeyPressed);
        EventManager.Instance.AddListener<TextEvent>(HandleTextEvent);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<TextSkipKeyPressedEvent>(HandleSkipKeyPressed);
        EventManager.Instance.RemoveListener<TextEvent>(HandleTextEvent);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            EventManager.Instance.Raise(new TextSkipKeyPressedEvent());
	    }
    }

    private void HandleSkipKeyPressed(TextSkipKeyPressedEvent e)
    { 
        if (messageRunning) // if a message is currently dispalying, return
        {
            return;
	    }
        if (messageQueue.Count == 0) // if there is no more message to display, return
        {
            return;
	    }
        // start the message animation
        TextEvent text = (TextEvent) messageQueue.Dequeue();
        currentText = "";
        fullText = text.text;
        delay = text.delay;
        StartCoroutine(ShowTextRoutine());
    }

    private void HandleTextEvent(TextEvent e)
    {
        messageQueue.Enqueue(e);
    }

    private IEnumerator ShowTextRoutine()
    {
        messageRunning = true;
        for (int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText[..i];
            textGui.text = currentText;
            yield return new WaitForSeconds(delay);
        }
        messageRunning = false;
    }
}
