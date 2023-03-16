using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomCursor : MonoBehaviour
{
    Image image;

    private void Awake()
    {
        Cursor.visible = false;
        this.image = GetComponent<Image>();
    }

    void OnEnable()
    {
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }
    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<CursorUpdateEvent>(UpdateCursor);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<CursorUpdateEvent>(UpdateCursor);
    }

    private void UpdateCursor(CursorUpdateEvent e)
    {
        this.image.sprite = e.sprite;
    }

    private void Update()
    {
        this.transform.position = Input.mousePosition;
    }
}
