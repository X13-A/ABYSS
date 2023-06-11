using SDD.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [SerializeField] private Light light;
    [SerializeField] private Color[] colors;

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
        EventManager.Instance.AddListener<EndBossScreamerEvent>(StartCoroutineBossPath);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<EndBossScreamerEvent>(StartCoroutineBossPath);
    }

    private void Start()
    {
        light.enabled = true;
        light.color = colors[0];
    }

    private void StartCoroutineBossPath(EndBossScreamerEvent e)
    {
        light.color = colors[1];
        EventManager.Instance.Raise(new StartCoroutineBossPathEvent { });
    }
}
