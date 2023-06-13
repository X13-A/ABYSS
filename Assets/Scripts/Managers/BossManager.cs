using SDD.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    private static BossManager m_Instance;
    public static BossManager Instance => m_Instance;

    [SerializeField] private new Light light;
    [SerializeField] private float timeBeforeWakingUp;
    [SerializeField] private ParticleSystem[] bossParticle;
    [SerializeField] private EnemyDamage bossDamage;

    public float BossHealth => bossDamage.Health;
    public float TimeBeforeWakingUp => timeBeforeWakingUp;
    private bool defeated = false;

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
        //EventManager.Instance.AddListener<EndBossScreamerEvent>(StartCoroutineBossPath);
    }

    public void UnsubscribeEvents()
    {
        //EventManager.Instance.RemoveListener<EndBossScreamerEvent>(StartCoroutineBossPath);
    }

    private void Start()
    {
        light.enabled = true;
    }

    private void StartCoroutineBossPath(EndBossScreamerEvent e)
    {
        EventManager.Instance.Raise(new StartCoroutineBossPathEvent { });
    }

    private void updateParticule(float health)
    {
        //if(health < 70)
        //{
            

        //}
    }

    private void Update()
    {
        // HACK: Should use events instead of constant checking
        if (!defeated && BossHealth <= 0)
        {
            defeated = true;
            EventManager.Instance.Raise(new BossDefeatedEvent {});

            // Roll credits
            StartCoroutine(CoroutineUtil.DelayAction(5f, () =>
            {
                EventManager.Instance.Raise(new SceneAboutToChangeEvent
                {
                    levelGenerated = 0,
                    displayLoading = false,
                    targetScene = "Credits"
                });
            }));
        }
    }
}
