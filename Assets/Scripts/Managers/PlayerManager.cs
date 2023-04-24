using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class PlayerManager : MonoBehaviour, IEventHandler
{
    public static PlayerManager Instance;

    [SerializeField] private PlayerMode activePlayerMode;
    [SerializeField] private PlayerLook activePlayerLook;
    [SerializeField] private AimingMode activeAimingMode;
    public PlayerMode ActivePlayerMode => this.activePlayerMode;
    public PlayerLook ActivePlayerLook => this.activePlayerLook;
    public AimingMode ActiveAimingMode => this.activeAimingMode;

    private void OnEnable()
    {
        this.SubscribeEvents();
    }

    private void OnDisable()
    {
        this.UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<PlayerSwitchModeEvent>(this.SetPlayerMode);
        EventManager.Instance.AddListener<PlayerSwitchLookModeEvent>(this.SetPlayerLook);
        EventManager.Instance.AddListener<AimingModeUpdateEvent>(this.SetPlayerAim);

        // Reset aim mode on menus
        EventManager.Instance.AddListener<GameMainMenuEvent>(this.SetAimingModeFromUIEvent);
        EventManager.Instance.AddListener<GamePauseMenuEvent>(this.SetAimingModeFromUIEvent);
        EventManager.Instance.AddListener<GameSettingsMenuEvent>(this.SetAimingModeFromUIEvent);
        EventManager.Instance.AddListener<GameOverEvent>(this.SetAimingModeFromUIEvent);
        EventManager.Instance.AddListener<GameSaveSettingsEvent>(this.SetAimingModeFromUIEvent);
        EventManager.Instance.AddListener<GameCancelSettingsEvent>(this.SetAimingModeFromUIEvent);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerSwitchModeEvent>(this.SetPlayerMode);
        EventManager.Instance.RemoveListener<PlayerSwitchLookModeEvent>(this.SetPlayerLook);
        EventManager.Instance.RemoveListener<AimingModeUpdateEvent>(this.SetPlayerAim);

        // Reset aim mode on menus
        EventManager.Instance.RemoveListener<GameMainMenuEvent>(this.SetAimingModeFromUIEvent);
        EventManager.Instance.RemoveListener<GamePauseMenuEvent>(this.SetAimingModeFromUIEvent);
        EventManager.Instance.RemoveListener<GameSettingsMenuEvent>(this.SetAimingModeFromUIEvent);
        EventManager.Instance.RemoveListener<GameOverEvent>(this.SetAimingModeFromUIEvent);
        EventManager.Instance.RemoveListener<GameSaveSettingsEvent>(this.SetAimingModeFromUIEvent);
        EventManager.Instance.RemoveListener<GameCancelSettingsEvent>(this.SetAimingModeFromUIEvent);
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
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetPlayerMode(PlayerSwitchModeEvent e)
    {
        this.activePlayerMode = e.mode;
    }

    private void SetPlayerLook(PlayerSwitchLookModeEvent e)
    {
        this.activePlayerLook = e.lookMode;
    }

    private void SetPlayerAim(AimingModeUpdateEvent e)
    {
        this.activeAimingMode = e.mode;
    }
}
