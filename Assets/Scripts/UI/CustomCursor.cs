using SDD.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomCursor : MonoBehaviour
{
    private Image image;
    private void Awake()
    {
        Cursor.visible = false;
        this.image = GetComponent<Image>();
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

    private void ClampPosition()
    {
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(transform.position.x, 0, RenderManager.Instance.ScreenWidth);
        clampedPosition.y = Mathf.Clamp(transform.position.y, 0, RenderManager.Instance.ScreenHeight);
        transform.position = clampedPosition;
    }

    private void Move()
    {
        transform.position = Input.mousePosition;
    }
    private void ToggleImage(bool value)
    {
        if (this.image != null && this.image.enabled != value)
        {
            this.image.enabled = value;
        }
    }

    private bool IsActive()
    {
        if (GameManager.Instance.State == GAMESTATE.PLAY)
        {
            return Aiming.Instance.AimingMode == AimingMode.CURSOR;
        }
        else
        {
            return true;
        }
    }

    private void Update()
    {

        if (this.IsActive())
        {
            this.Move();
            this.ClampPosition();
            this.ToggleImage(true);
        }
        else
        {
            this.ToggleImage(false);
        }
    }
}
