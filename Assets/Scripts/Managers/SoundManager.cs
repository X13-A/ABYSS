using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager m_Instance;
    public static SoundManager Instance => m_Instance;

    private AudioSource audioSource;

    [SerializeField] private AudioClip EnteringIntoPortal;
    [SerializeField] private AudioClip LootItem;
    [SerializeField] private AudioClip HitDamage;
    [SerializeField] private AudioClip ButtonClicked;
    [SerializeField] private AudioClip BossDefeated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        m_Instance = this;
        gameObject.transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
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
        EventManager.Instance.AddListener<SceneAboutToChangeEvent>(LaunchEnteringIntoPortalSound);
        EventManager.Instance.AddListener<ItemPickedUpEvent>(LaunchLoot);
        EventManager.Instance.AddListener<DamagePlayerEvent>(LaunchDamageHit);
        EventManager.Instance.AddListener<ButtonClickedEvent>(LaunchButtonClicked);
        EventManager.Instance.AddListener<BossDefeatedEvent>(LaunchBossDefeated);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<SceneAboutToChangeEvent>(LaunchEnteringIntoPortalSound);
        EventManager.Instance.RemoveListener<ItemPickedUpEvent>(LaunchLoot);
        EventManager.Instance.RemoveListener<DamagePlayerEvent>(LaunchDamageHit);
        EventManager.Instance.RemoveListener<ButtonClickedEvent>(LaunchButtonClicked);
        EventManager.Instance.RemoveListener<BossDefeatedEvent>(LaunchBossDefeated);
    }

    private void LaunchEnteringIntoPortalSound(SceneAboutToChangeEvent e)
    {
        audioSource.PlayOneShot(EnteringIntoPortal);
    }

    private void LaunchLoot(ItemPickedUpEvent e)
    {
        audioSource.PlayOneShot(LootItem);
    }

    private void LaunchDamageHit(DamagePlayerEvent e)
    {
        audioSource.PlayOneShot(HitDamage);
    }

    private void LaunchButtonClicked(ButtonClickedEvent e)
    {
        audioSource.PlayOneShot(ButtonClicked);
    }

    public void LaunchBossDefeated(BossDefeatedEvent e)
    {
        audioSource.PlayOneShot(BossDefeated);
    }
}
