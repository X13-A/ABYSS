using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip sound;
    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
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
        EventManager.Instance.AddListener<BossDefeatedEvent>(LaunchSound);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<BossDefeatedEvent>(LaunchSound);
    }

    public void LaunchSound(BossDefeatedEvent e)
    {
        audioSource.PlayOneShot(sound);
    }
}
