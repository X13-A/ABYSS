using SDD.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEditor.PlayerSettings;

public class BossManager : MonoBehaviour
{
    [SerializeField] private new Light light;
    [SerializeField] private float timeBeforeWakingUp;
    [SerializeField] private GameObject[] bossParticles;
    [SerializeField] private EnemyDamage bossDamage;
    [SerializeField] private float movementSpeed;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject path;
    [SerializeField] private Vector3 impactDirection;
    [SerializeField] private float impactSpeed;
    [SerializeField] private GameObject bossReference;
    public GameObject BossReference => bossReference;

    public float BossHealth => bossDamage.Health;
    private float bossMaxHealth;
    private Vector3[] coordinates;
    private static BossManager m_Instance;
    private bool particlesActivated70 = false;
    private bool particlesActivated30 = false;
    private float mass = 3.0F;
    Vector3 impact = Vector3.zero;
    private CharacterController character;

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
        //EventManager.Instance.AddListener<EnemyAttackEvent>(PushesPlayer);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ModeBossEvent>(StartCoroutineBossPath);
        //EventManager.Instance.RemoveListener<EnemyAttackEvent>(PushesPlayer);
    }

    private void Start()
    {
        character = PlayerManager.Instance.PlayerReference.GetComponent<CharacterController>();
        bossMaxHealth = bossDamage.Health;
        bossParticles[0].SetActive(true);
        bossParticles[1].SetActive(false);
        bossParticles[2].SetActive(false);
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
        if (BossHealth <= bossMaxHealth * 0.70 && !particlesActivated70)
        {
            particlesActivated70 = true;
            particlesActivated30 = false;
            bossParticles[0].SetActive(false);
            bossParticles[1].SetActive(true);
            EventManager.Instance.Raise(new ModeBossEvent { });
        }

        if (BossHealth <= bossMaxHealth * 0.30 && !particlesActivated30)
        {
            particlesActivated30 = true;
            bossParticles[1].SetActive(false);
            bossParticles[2].SetActive(true);
            EventManager.Instance.Raise(new ModeBossEvent { });
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

        // apply the impact force:
        if (impact.magnitude > 0.2F) character.Move(impact * Time.deltaTime);
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }

    //private void PushesPlayer(EnemyAttackEvent e)
    //{
    //    AddImpact(impactDirection, impactSpeed);
    //}

    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }

    private void StartCoroutineBossPath(ModeBossEvent e)
    {
        Debug.Log("tesssssssst");
        StartCoroutine(BossPath());
    }

    private IEnumerator BossPath()
    {
        System.Random random = new System.Random();

        int numPositions = random.Next(3, coordinates.Length);

        List<Vector3> selectedPositions = new List<Vector3>();

        while (selectedPositions.Count < numPositions)
        {
            int randomIndex = random.Next(0, coordinates.Length);
            Vector3 selectedPosition = coordinates[randomIndex];

            if (!selectedPositions.Contains(selectedPosition))
            {
                selectedPositions.Add(selectedPosition);
            }
        }

        // Parcours des positions sélectionnées
        foreach (Vector3 position in selectedPositions)
        {
            boss.transform.position = position;
            yield return new WaitForSeconds(1f);
        }
    }

}
