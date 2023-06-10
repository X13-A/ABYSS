using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignHandler : MonoBehaviour
{
    [SerializeField] private List<string> messages;
    private bool needToListen;

    private const float FACING_NORMAL_VALUE = 0.7f;

    private void Start() => needToListen = false;

    private void Update()
    {
        if (needToListen)
        {
            ListenToInteractionKey();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject player = other.gameObject;
        Vector3 panelForward = transform.forward;
        Vector3 playerDirection = (player.transform.position - transform.position).normalized;

        float angle = Vector3.Dot(panelForward, playerDirection);

        if (angle > FACING_NORMAL_VALUE)
        {
            EventManager.Instance.Raise(new ShowSignInteractionMessage());
            needToListen = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EventManager.Instance.Raise(new HideSignInteractionMessage());
        needToListen = false;
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
