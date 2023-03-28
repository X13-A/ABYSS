using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class CursorManager : MonoBehaviour, IEventHandler
{
    public static CursorManager Instance;

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
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerSwitchModeEvent>(SetCursorType);
    }

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

    private void Start()
    {
        SetCursorType(activeCursorType);
    }

    public void SetCursorType(CursorType type)
    {
        activeCursorType = type;
        EventManager.Instance.Raise(new CursorUpdateEvent { type = type, sprite = GetSprite(type) });
    }

    private void SetCursorType(PlayerSwitchModeEvent e)
    {
        activeCursorType = EnumConverter.CursorTypeFromPlayerMode(e.mode);
        Debug.Log(e.mode);
        EventManager.Instance.Raise(new CursorUpdateEvent { type = activeCursorType, sprite = GetSprite(activeCursorType) });
    }

    private Sprite GetSprite(CursorType type)
    {
        return type switch
        {
            CursorType.MENU => menuCursor,
            CursorType.MELEE => meleeCursor,
            CursorType.MAGIC => magicCursor,
            CursorType.RANGE => bowCursor,
            CursorType.BUILD => buildCursor,
            _ => menuCursor,
        };
    }
}
