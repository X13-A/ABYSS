using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignHandler : MonoBehaviour
{
    [SerializeField] private List<string> messages;
    private bool needToListen;
    private bool playerNearSign;
    private const float FACING_NORMAL_VALUE = 0.7f;


    private void Start()
    {
        needToListen = false;
    }

    private void Update()
    {
        if (TextManager.Instance.LastClosedTime != 0 && Time.time - TextManager.Instance.LastClosedTime < 0.25f) return; // Hack to prevent opening the same at the same time as closing it
        if (playerNearSign)
        {
            Vector3 panelForward = transform.forward;
            Vector3 playerDirection = (PlayerManager.Instance.PlayerReference.transform.position - transform.position).normalized;
            float angle = Vector3.Dot(panelForward, playerDirection);
            if (TextManager.Instance.MessageActive == false && angle > FACING_NORMAL_VALUE)
            {
                EventManager.Instance.Raise(new ShowSignInteractionMessage());
                needToListen = true;
            }
        }
        if (needToListen)
        {
            ListenToInteractionKey();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IPlayerCollider player = other.gameObject.GetComponent<IPlayerCollider>();
        if (player == null) return;
        playerNearSign = true;
    }

    private void OnTriggerExit(Collider other)
    {
        IPlayerCollider player = other.gameObject.GetComponent<IPlayerCollider>();
        if (player != null)
        {
            needToListen = false;
            playerNearSign = false;
            EventManager.Instance.Raise(new HideSignInteractionMessage());
        }
    }

    private void ListenToInteractionKey()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            EventManager.Instance.Raise(new HideSignInteractionMessage());
            RaiseAllMessages();
            needToListen = false;
        }
    }

    private void RaiseAllMessages()
    {
        foreach (string message in messages)
        {
            EventManager.Instance.Raise(new MessageEvent
            {
                text = message,
                delay = 0.05f
            });
        }
    }

}
