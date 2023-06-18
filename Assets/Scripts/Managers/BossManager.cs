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
    [SerializeField] private float timeBeforeWakingUp;

    [SerializeField] private GameObject[] bossParticles;
    [SerializeField] private EnemyDamage bossDamage;
    [SerializeField] private GameObject bossReference;
    [SerializeField] private BossAnimationController bossAnimationController;
    public GameObject BossReference => bossReference;

    [SerializeField] private ParticleSystem bossWarpParticles;
    [SerializeField] private GameObject WarpPath;

    [SerializeField] private Vector3 impactDirection;
    [SerializeField] private float impactSpeed;
    [SerializeField] private float magicAttackCooldown = 1f;

    public float MagicAttackCooldown => magicAttackCooldown;
    public float BossHealth => bossDamage.Health;
    private float bossMaxHealth;
    private Vector3[] coordinates;
    private static BossManager m_Instance;
    private bool particlesActivated70 = false;
    private bool particlesActivated30 = false;
    private float bossSpeedScale = 0.5f;
    public float BossSpeedScale => bossSpeedScale;

    private float lastMagicAttackTime;
    public float LastMagicAttackTime => lastMagicAttackTime;

    //private float mass = 3.0F;
    //Vector3 impact = Vector3.zero;

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
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ModeBossEvent>(StartCoroutineBossPath);
    }

    private void Start()
    {
        ResetLastMagicAttackTime();

        bossMaxHealth = bossDamage.Health;
        bossParticles[0].SetActive(true);
        bossParticles[1].SetActive(false);
        bossParticles[2].SetActive(false);
        Transform[] childrenTransforms = WarpPath.GetComponentsInChildren<Transform>();
        int numChildren = childrenTransforms.Length - 1;
        coordinates = new Vector3[numChildren];
        for (int i = 1; i < childrenTransforms.Length; i++)
        {
            coordinates[i - 1] = childrenTransforms[i].localPosition;
        }
    }

    public void ResetLastMagicAttackTime()
    {
        lastMagicAttackTime = Time.time;
    }

    // TODO: Manage knockback in player collider directly for wide use

    //private void ApplyKnockbackForce()
    //{
    //    // apply the impact force:
    //    if (impact.magnitude > 0.2F) PlayerManager.Instance.PlayerReference.GetComponent<CharacterController>().Move(impact * Time.deltaTime);
    //    // consumes the impact energy each cycle:
    //    impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    //}
    //public void AddImpact(Vector3 dir, float force)
    //{
    //    dir.Normalize();
    //    if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
    //    impact += dir.normalized * force / mass;
    //}

    //private void PushPlayer()
    //{
    //    AddImpact(impactDirection, impactSpeed);
    //}

    private void StartCoroutineBossPath(ModeBossEvent e)
    {
        StartCoroutine(BossPath());
    }

    private IEnumerator BossPath()
    {
        System.Random random = new System.Random();

        List<Vector3> selectedPositions = new List<Vector3>();
        while (selectedPositions.Count < 5)
        {
            int randomIndex = random.Next(0, coordinates.Length);
            Vector3 selectedPosition = coordinates[randomIndex];
            if (!selectedPositions.Contains(selectedPosition))
            {
                selectedPositions.Add(selectedPosition);
            }
        }

        // Iterate selected positions
        foreach (Vector3 position in selectedPositions)
        {
            // Warp boss
            bossReference.transform.position = position;

            // Instant fire magic projectile
            lastMagicAttackTime = Time.time - magicAttackCooldown / bossSpeedScale;
            bossWarpParticles.transform.position = position;
            bossWarpParticles.Stop();
            bossWarpParticles.Play();
            bossAnimationController.StartMagicAttackAnimation();

            // Make boss face player
            Vector3 directionToPlayer = ((PlayerManager.Instance.PlayerReference.position) - (BossReference.transform.position)).normalized;
            directionToPlayer.y = 0;
            Quaternion quaternionToPlayer = Quaternion.LookRotation(directionToPlayer);
            BossReference.GetComponent<Rigidbody>().MoveRotation(quaternionToPlayer);

            // Wait until next warp
            yield return new WaitForSeconds(1.25f / bossSpeedScale);
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
            bossSpeedScale = 0.75f;
            EventManager.Instance.Raise(new ModeBossEvent { });
        }

        if (BossHealth <= bossMaxHealth * 0.30 && !particlesActivated30)
        {
            particlesActivated30 = true;
            bossParticles[1].SetActive(false);
            bossParticles[2].SetActive(true);
            EventManager.Instance.Raise(new ModeBossEvent { });
            bossSpeedScale = 1f;
        }

        // HACK: Should use events instead of constant checking
        if (!defeated && BossHealth <= 0)
        {
            defeated = true;
            EventManager.Instance.Raise(new BossDefeatedEvent { });
            EventManager.Instance.Raise(new SetScoreEvent { addedScore = 1000 });

            // Roll credits
            StartCoroutine(CoroutineUtil.DelayAction(5f, () =>
            {
                EventManager.Instance.Raise(new ClearInventoryEvent { });
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
