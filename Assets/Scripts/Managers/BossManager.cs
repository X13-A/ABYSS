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
    private float initBossHealth;
    private Vector3[] coordinates;
    private static BossManager m_Instance;

    public static BossManager Instance => m_Instance;
    public float TimeBeforeWakingUp => timeBeforeWakingUp;

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
        initBossHealth = bossDamage.Health;
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

    private void Update()
    {
        if (bossDamage.Health <= initBossHealth * 0.70)
        {
            EventManager.Instance.Raise(new ModeBossEvent { });
            bossParticle[0].SetActive(false);
            bossParticle[1].SetActive(true);
        }

        if (bossDamage.Health <= initBossHealth * 0.30)
        {
            EventManager.Instance.Raise(new ModeBossEvent { });
            bossParticle[1].SetActive(false);
            bossParticle[2].SetActive(true);
        }
    }

    private void PushesPlayer(EnemyAttackEvent e)
    {
        Debug.Log("sibngfrmqie");
        //Vector3 force = new Vector3(5f, 5f, 5f);
        //Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
        //playerRigidbody.AddForce(force, ForceMode.Impulse);

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
