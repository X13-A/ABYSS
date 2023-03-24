using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class CursorManager : MonoBehaviour, IEventHandler
{
    public static CursorManager Instance;

    [SerializeField] private float sensitivity;
    public float Sensitivity { get { return sensitivity; } }

    [SerializeField] private Sprite menuCursor;
    [SerializeField] private Sprite meleeCursor;
    [SerializeField] private Sprite bowCursor;
    [SerializeField] private Sprite magicCursor;
    [SerializeField] private Sprite buildCursor;
    [SerializeField] private CursorType activeCursorType;
    public CursorType ActiveCursorType { get { return this.activeCursorType; } }
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
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        SetCursorType(this.activeCursorType);
    }

    public void SetCursorType(CursorType type)
    {
        this.activeCursorType = type;
        EventManager.Instance.Raise(new CursorUpdateEvent { type = type, sprite = this.GetSprite(type) });
    }

    private void SetCursorType(PlayerSwitchModeEvent e)
    {
        this.activeCursorType = EnumConverter.CursorTypeFromPlayerMode(e.mode);
        Debug.Log(e.mode);
        EventManager.Instance.Raise(new CursorUpdateEvent { type = this.activeCursorType, sprite = this.GetSprite(this.activeCursorType) });
    }

    private Sprite GetSprite(CursorType type)
    {
        return type switch
        {
            CursorType.MENU => this.menuCursor,
            CursorType.MELEE => this.meleeCursor,
            CursorType.MAGIC => this.magicCursor,
            CursorType.RANGE => this.bowCursor,
            CursorType.BUILD => this.buildCursor,
            _ => this.menuCursor,
        };
    }
}
