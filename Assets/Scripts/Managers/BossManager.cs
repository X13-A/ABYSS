using SDD.Events;
using System.Collections;
using System.Collections.Generic;
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

    private void StartCoroutineBossPath(EndBossScreamerEvent e)
    {
        EventManager.Instance.Raise(new StartCoroutineBossPathEvent { });
    }
}
