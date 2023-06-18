using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class PlayerManager : MonoBehaviour, IEventHandler
{
    private static PlayerManager m_Instance;
    public static PlayerManager Instance => m_Instance;

    [SerializeField] private Transform playerReference;
    [SerializeField] private PlayerMode activePlayerMode;
    [SerializeField] private AimingMode activeAimingMode;
    [SerializeField] private float health;
    [SerializeField] private float maxShield;
    [SerializeField] private float shield;
    [SerializeField] private int score;
    [SerializeField] private GameObject playerProjectileStart;
    [SerializeField] private float playerAttackSpeedMultiplier;


    public Transform PlayerReference => playerReference;
    public PlayerMode ActivePlayerMode => activePlayerMode;
    public AimingMode ActiveAimingMode => activeAimingMode;
    public float Health => health;
    public float MaxShield => maxShield;
    public float Shield => shield;
    public int Score => score;
    public float PlayerAttackSpeedMultiplier
    {
        get => Mathf.Max(playerAttackSpeedMultiplier, 0.01f);
        set => playerAttackSpeedMultiplier = value;
    }
    public GameObject PlayerProjectileStart => playerProjectileStart;


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
        EventManager.Instance.AddListener<PlayerSwitchModeEvent>(SetPlayerMode);
        EventManager.Instance.AddListener<AimingModeUpdateEvent>(SetPlayerAim);
        EventManager.Instance.AddListener<DamagePlayerEvent>(SetHealthDamage);
        EventManager.Instance.AddListener<CarePlayerEvent>(SetHealthCare);
        EventManager.Instance.AddListener<DamageShieldPlayerEvent>(SetShieldDamage);
        EventManager.Instance.AddListener<SetShieldPlayerEvent>(SetShieldCare);
        EventManager.Instance.AddListener<SetScoreEvent>(SetScore);
        EventManager.Instance.AddListener<AttackSpeedMultiplierEvent>(SetAttackSpeedMultiplier);

        // Reset aim mode on menus
        EventManager.Instance.AddListener<GameMainMenuEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.AddListener<GamePauseMenuEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.AddListener<GameSettingsMenuEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.AddListener<GameOverEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.AddListener<GameSaveSettingsEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.AddListener<GameCancelSettingsEvent>(SetAimingModeFromUIEvent);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerSwitchModeEvent>(SetPlayerMode);
        EventManager.Instance.RemoveListener<AimingModeUpdateEvent>(SetPlayerAim);
        EventManager.Instance.RemoveListener<DamagePlayerEvent>(SetHealthDamage);
        EventManager.Instance.RemoveListener<CarePlayerEvent>(SetHealthCare);
        EventManager.Instance.RemoveListener<DamageShieldPlayerEvent>(SetShieldDamage);
        EventManager.Instance.RemoveListener<SetShieldPlayerEvent>(SetShieldCare);
        EventManager.Instance.RemoveListener<SetScoreEvent>(SetScore);
        EventManager.Instance.RemoveListener<AttackSpeedMultiplierEvent>(SetAttackSpeedMultiplier);

        // Reset aim mode on menus
        EventManager.Instance.RemoveListener<GameMainMenuEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.RemoveListener<GamePauseMenuEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.RemoveListener<GameSettingsMenuEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.RemoveListener<GameOverEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.RemoveListener<GameSaveSettingsEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.RemoveListener<GameCancelSettingsEvent>(SetAimingModeFromUIEvent);
    }

    #region UI events callbacks
    private void SetAimingModeFromUIEvent(GameMainMenuEvent e)
    {
        EventManager.Instance.Raise(new AimingModeUpdateEvent() { mode = AimingMode.CAMERA });
    }
    private void SetAimingModeFromUIEvent(GamePauseMenuEvent e)
    {
        EventManager.Instance.Raise(new AimingModeUpdateEvent() { mode = AimingMode.CAMERA });
    }
    private void SetAimingModeFromUIEvent(GameSettingsMenuEvent e)
    {
        EventManager.Instance.Raise(new AimingModeUpdateEvent() { mode = AimingMode.CAMERA });
    }
    private void SetAimingModeFromUIEvent(GameOverEvent e)
    {
        EventManager.Instance.Raise(new AimingModeUpdateEvent() { mode = AimingMode.CAMERA });
    }
    private void SetAimingModeFromUIEvent(GameSaveSettingsEvent e)
    {
        EventManager.Instance.Raise(new AimingModeUpdateEvent() { mode = AimingMode.CAMERA });
    }
    private void SetAimingModeFromUIEvent(GameCancelSettingsEvent e)
    {
        EventManager.Instance.Raise(new AimingModeUpdateEvent() { mode = AimingMode.CAMERA });
    }
    #endregion

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
    }

    private void SetPlayerMode(PlayerSwitchModeEvent e)
    {
        activePlayerMode = e.mode;
    }

    private void SetPlayerAim(AimingModeUpdateEvent e)
    {
        activeAimingMode = e.mode;
    }

    private void SetHealthDamage(DamagePlayerEvent e)
    {
        if (health <= 0) return;
        if (PlayerManager.Instance.Shield > 0)
        {
            // Calculates damage after shield is broken
            float damageToHealth = e.damage - PlayerManager.Instance.Shield;
            EventManager.Instance.Raise(new DamageShieldPlayerEvent { shieldDamage = e.damage });

            // If there is, inflict it
            if (damageToHealth > 0)
            {
                health = Mathf.Max(health - damageToHealth, 0);
                EventManager.Instance.Raise(new UpdatePlayerHealthEvent { newHealth = health - damageToHealth });
            }

            // Check if dead
            if (health <= 0)
            {
                Die();
            }
            return;
        }


        health = Mathf.Max(health - e.damage, 0);
        EventManager.Instance.Raise(new UpdatePlayerHealthEvent { newHealth = health });
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        health = 0;
        EventManager.Instance.Raise(new SaveScoreEvent { });
        EventManager.Instance.Raise(new PlayerDeadEvent { });
        StartCoroutine(CoroutineUtil.DelayAction(1f, () =>
        {
            EventManager.Instance.Raise(new GameOverEvent { });
        }));
    }

    private void SetHealthCare(CarePlayerEvent e)
    {
        health = Mathf.Min(health + e.care, 100);
        EventManager.Instance.Raise(new UpdatePlayerHealthEvent { newHealth = health });
    }

    private void SetShieldDamage(DamageShieldPlayerEvent e)
    {
        float damage = e.shieldDamage * 0.75f;
        shield = Mathf.Max(shield - damage, 0);
        EventManager.Instance.Raise(new UpdateShieldPlayerHealthEvent { newShieldHealth = shield });
    }

    private void SetShieldCare(SetShieldPlayerEvent e)
    {
        shield = Mathf.Min(shield + e.shield, 100);
        EventManager.Instance.Raise(new UpdateShieldPlayerHealthEvent { newShieldHealth = shield });
    }

    private void SetScore(SetScoreEvent e)
    {
        score += e.addedScore;
        EventManager.Instance.Raise(new UpdateScoreEvent { updatedScore = score });
    }

    private void SetAttackSpeedMultiplier(AttackSpeedMultiplierEvent e)
    {
        PlayerManager.Instance.PlayerAttackSpeedMultiplier = e.speed;
    }
}
