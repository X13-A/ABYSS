using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksAnimations : MonoBehaviour, IEventHandler
{

    private Animator m_Animator;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void SubscribeEvents()
    {
    }

    public void UnsubscribeEvents()
    {
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    void Update()
    {

    }
}
