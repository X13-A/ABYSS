using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager m_Instance;
    public static SoundManager Instance => m_Instance;

    private AudioSource audioSource;

    [SerializeField] private AudioClip SwordAttack;
    [SerializeField] private AudioClip EnteringIntoPortal;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
        EventManager.Instance.AddListener<PlayerAttackEvent>(LaunchSwordAttackSound);
        EventManager.Instance.AddListener<SceneAboutToChangeEvent>(LaunchEnteringIntoPortalSound);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerAttackEvent>(LaunchSwordAttackSound);
        EventManager.Instance.RemoveListener<SceneAboutToChangeEvent>(LaunchEnteringIntoPortalSound);

    }

    private void LaunchSwordAttackSound(PlayerAttackEvent e)
    {
        audioSource.PlayOneShot(SwordAttack);
    }

    private void LaunchEnteringIntoPortalSound(SceneAboutToChangeEvent e)
    {
        audioSource.PlayOneShot(EnteringIntoPortal);
    }
}
