using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public enum CursorType { MENU, MELEE, BOW, MAGIC }

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [SerializeField] Sprite menuCursor;
    [SerializeField] Sprite meleeCursor;
    [SerializeField] Sprite bowCursor;
    [SerializeField] Sprite magicCursor;
    [SerializeField] CursorType activeCursorType;
    public CursorType ActiveCursorType { get { return activeCursorType; } }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SetCursorType(activeCursorType);
    }

    public void SetCursorType(CursorType type)
    {
        this.activeCursorType = type;
        EventManager.Instance.Raise(new CursorUpdateEvent { type = type, sprite = GetSprite(type) });
    }

    public Sprite GetSprite(CursorType type)
    {
        return type switch
        {
            CursorType.MENU => menuCursor,
            CursorType.MELEE => meleeCursor,
            CursorType.MAGIC => magicCursor,
            CursorType.BOW => bowCursor,
            _ => menuCursor,
        };
    }
}