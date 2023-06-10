using SDD.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [SerializeField] private Material floorMaterial;
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
        floorMaterial.SetColor("_Color", colors[0]);
    }

    private void StartCoroutineBossPath(EndBossScreamerEvent e)
    {
        floorMaterial.SetColor("_Color", colors[3]);
        EventManager.Instance.Raise(new StartCoroutineBossPathEvent { });
    }
}
