using SDD.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [SerializeField] private new Light light;
    [SerializeField] private float timeBeforeWakingUp;
    [SerializeField] private GameObject[] bossParticle;
    [SerializeField] private EnemyDamage bossDamage;
    [SerializeField] private float movementSpeed;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject path;

    public float BossHealth => bossDamage.Health;
    private float bossMaxHealth;
    private Vector3[] coordinates;
    private static BossManager m_Instance;

    public static BossManager Instance => m_Instance;
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
        EventManager.Instance.AddListener<ModeBossEvent>(StartCoroutineBossPath);
        EventManager.Instance.AddListener<EnemyAttackEvent>(PushesPlayer);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ModeBossEvent>(StartCoroutineBossPath);
        EventManager.Instance.RemoveListener<EnemyAttackEvent>(PushesPlayer);
    }

    private void Start()
    {
        bossMaxHealth = bossDamage.Health;
        bossParticle[0].SetActive(true);
        bossParticle[1].SetActive(false);
        bossParticle[2].SetActive(false);
        Transform[] childrenTransforms = path.GetComponentsInChildren<Transform>();
        int numChildren = childrenTransforms.Length - 1;
        coordinates = new Vector3[numChildren];

        for (int i = 1; i < childrenTransforms.Length; i++)
        {
            coordinates[i - 1] = childrenTransforms[i].localPosition;
        }
    }

    private void PushesPlayer(EnemyAttackEvent e)
    {
        Debug.Log("sibngfrmqie");
        //Vector3 force = new Vector3(5f, 5f, 5f);
        //Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
        //playerRigidbody.AddForce(force, ForceMode.Impulse);

    }

    private void Update()
    {
        // TODO: Trigger event only once !!!!
        if (BossHealth <= bossMaxHealth * 0.70)
        {
            EventManager.Instance.Raise(new ModeBossEvent { });
            bossParticle[0].SetActive(false);
            bossParticle[1].SetActive(true);
        }

        if (BossHealth <= bossMaxHealth * 0.30)
        {
            EventManager.Instance.Raise(new ModeBossEvent { });
            bossParticle[1].SetActive(false);
            bossParticle[2].SetActive(true);
        }

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
    
    private void StartCoroutineBossPath(ModeBossEvent e)
    {
        StartCoroutine(BossPath());
    }

    private IEnumerator BossPath()
    {
        yield return null;
    }

}
