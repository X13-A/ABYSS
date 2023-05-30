using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class CursorManager : MonoBehaviour, IEventHandler
{
    private static CursorManager m_Instance;
    public static CursorManager Instance => m_Instance;

    [SerializeField] private float sensitivity;
    public float Sensitivity => sensitivity;

    [SerializeField] private Sprite menuCursor;
    [SerializeField] private Sprite meleeCursor;
    [SerializeField] private Sprite bowCursor;
    [SerializeField] private Sprite magicCursor;
    [SerializeField] private Sprite buildCursor;
    [SerializeField] private CursorType activeCursorType;

    public CursorType ActiveCursorType => activeCursorType;
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
        EventManager.Instance.AddListener<PlayerSwitchModeEvent>(SetCursorType);
        EventManager.Instance.AddListener<AimingModeUpdateEvent>(SetCursorLockMode);
        // Menus
        EventManager.Instance.AddListener<GameMainMenuEvent>(SetCursorFromMainMenuEvent);
        EventManager.Instance.AddListener<GamePauseMenuEvent>(SetCursorFromPauseMenuEvent);
        EventManager.Instance.AddListener<GameSettingsMenuEvent>(SetCursorFromSettingsMenuEvent);
        EventManager.Instance.AddListener<GamePlayEvent>(SetCursorFromPlayEvent);
        EventManager.Instance.AddListener<GameResumeEvent>(SetCursorFromResumeEvent);
        EventManager.Instance.AddListener<GameOverEvent>(SetCursorFromGameOverEvent);
        EventManager.Instance.AddListener<GameSaveSettingsEvent>(SetCursorFromSaveSettingsEvent);
        EventManager.Instance.AddListener<GameCancelSettingsEvent>(SetCursorFromCancelSettingsEvent);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerSwitchModeEvent>(SetCursorType);
        EventManager.Instance.RemoveListener<AimingModeUpdateEvent>(SetCursorLockMode);

        // Menus
        EventManager.Instance.RemoveListener<GameMainMenuEvent>(SetCursorFromMainMenuEvent);
        EventManager.Instance.RemoveListener<GamePauseMenuEvent>(SetCursorFromPauseMenuEvent);
        EventManager.Instance.RemoveListener<GameSettingsMenuEvent>(SetCursorFromSettingsMenuEvent);
        EventManager.Instance.RemoveListener<GamePlayEvent>(SetCursorFromPlayEvent);
        EventManager.Instance.RemoveListener<GameResumeEvent>(SetCursorFromResumeEvent);
        EventManager.Instance.RemoveListener<GameOverEvent>(SetCursorFromGameOverEvent);
        EventManager.Instance.RemoveListener<GameSaveSettingsEvent>(SetCursorFromSaveSettingsEvent);
        EventManager.Instance.RemoveListener<GameCancelSettingsEvent>(SetCursorFromCancelSettingsEvent);
    }

    #region UI Callbacks
    private void SetCursorFromPlayEvent(GamePlayEvent e)
    {
        CursorType type = EnumConverter.CursorTypeFromPlayerMode(PlayerManager.Instance.ActivePlayerMode);
        Cursor.lockState = CursorLockMode.Locked;
        SetCursorType(type);
    }
    private void SetCursorFromResumeEvent(GameResumeEvent e)
    {
        CursorType type = EnumConverter.CursorTypeFromPlayerMode(PlayerManager.Instance.ActivePlayerMode);
        Cursor.lockState = CursorLockMode.Locked;
        SetCursorType(type);
    }
    private void SetCursorFromMainMenuEvent(GameMainMenuEvent e)
    {
        SetCursorType(CursorType.MENU);
    }
    private void SetCursorFromPauseMenuEvent(GamePauseMenuEvent e)
    {
        SetCursorType(CursorType.MENU);
    }
    private void SetCursorFromGameOverEvent(GameOverEvent e)
    {
        SetCursorType(CursorType.MENU);
    }
    private void SetCursorFromSettingsMenuEvent(GameSettingsMenuEvent e)
    {
        SetCursorType(CursorType.MENU);
    }
    private void SetCursorFromSaveSettingsEvent(GameSaveSettingsEvent e)
    {
        SetCursorType(CursorType.MENU);
    }
    private void SetCursorFromCancelSettingsEvent(GameCancelSettingsEvent e)
    {
        SetCursorType(CursorType.MENU);
    }
    #endregion

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

    private void Start()
    {
        SetCursorType(activeCursorType);
    }

    public void SetCursorType(CursorType type)
    {
        if (type == CursorType.MENU)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        activeCursorType = type;
        EventManager.Instance.Raise(new CursorUpdateEvent { type = type, sprite = GetSprite(type) });
    }

    private void SetCursorType(PlayerSwitchModeEvent e)
    {
        CursorType type = EnumConverter.CursorTypeFromPlayerMode(e.mode);
        if (type == CursorType.MENU)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        activeCursorType = type;
        EventManager.Instance.Raise(new CursorUpdateEvent { type = type, sprite = GetSprite(type) });
    }

    private void SetCursorLockMode(AimingModeUpdateEvent e)
    {
        if (MenuManager.Instance.HasMenuOpened)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (e.mode == AimingMode.CAMERA)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (e.mode == AimingMode.CURSOR)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private Sprite GetSprite(CursorType type)
    {
        return type switch
        {
            CursorType.MENU => menuCursor,
            CursorType.MELEE => meleeCursor,
            CursorType.MAGIC => magicCursor,
            CursorType.RANGE => bowCursor,
            _ => menuCursor,
        };
    }
}
