using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class PlayerManager : MonoBehaviour, IEventHandler
{
    private static PlayerManager m_Instance;
    public static PlayerManager Instance => m_Instance;

    [SerializeField] private PlayerMode activePlayerMode;
    [SerializeField] private AimingMode activeAimingMode;
    [SerializeField] private float health;
    public PlayerMode ActivePlayerMode => activePlayerMode;
    public AimingMode ActiveAimingMode => activeAimingMode;
    public float Health => health;


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
        EventManager.Instance.AddListener<PlayerSwitchModeEvent>(this.SetPlayerMode);
        EventManager.Instance.AddListener<PlayerHeldItemUpdateEvent>(this.SetPlayerMode);
        EventManager.Instance.AddListener<AimingModeUpdateEvent>(this.SetPlayerAim);
        EventManager.Instance.AddListener<EnemyAttackEvent>(this.SetHealth);

        // Reset aim mode on menus
        EventManager.Instance.AddListener<GameMainMenuEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.AddListener<GamePauseMenuEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.AddListener<GameSettingsMenuEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.AddListener<GameOverEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.AddListener<GameSaveSettingsEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.AddListener<GameCancelSettingsEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.AddListener<EnemyAttackEvent>(UpdatePlayerHealth);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerSwitchModeEvent>(this.SetPlayerMode);
        EventManager.Instance.RemoveListener<PlayerHeldItemUpdateEvent>(this.SetPlayerMode);
        EventManager.Instance.RemoveListener<AimingModeUpdateEvent>(this.SetPlayerAim);
        EventManager.Instance.RemoveListener<EnemyAttackEvent>(this.SetHealth);

        // Reset aim mode on menus
        EventManager.Instance.RemoveListener<GameMainMenuEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.RemoveListener<GamePauseMenuEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.RemoveListener<GameSettingsMenuEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.RemoveListener<GameOverEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.RemoveListener<GameSaveSettingsEvent>(SetAimingModeFromUIEvent);
        EventManager.Instance.RemoveListener<GameCancelSettingsEvent>(SetAimingModeFromUIEvent);
    }

    private void UpdatePlayerHealth(EnemyAttackEvent e) => health -= e.damage;

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

    // TODO: SHOULD BE DELETED WITH EVERYTHING RELATED TO PLAYERMODE
    private void SetPlayerMode(PlayerHeldItemUpdateEvent e)
    {
        PlayerMode? playerMode = ItemBank.PlayerModeFromItem(e.itemId);
        if (playerMode == null) playerMode = PlayerMode.UNARMED;
        EventManager.Instance.Raise(new PlayerSwitchModeEvent { mode = playerMode.Value });
    }

    private void SetPlayerAim(AimingModeUpdateEvent e)
    {
        activeAimingMode = e.mode;
    }

    private void SetHealth(EnemyAttackEvent e)
    {
        health = Mathf.Max(health - e.damage, 0);
    }
}
