using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using TMPro;
using System.ComponentModel;
using System;
using UnityEngine.UI;

public class HudManager : MonoBehaviour, IEventHandler
{
    private static HudManager m_Instance;
    public static HudManager Instance => m_Instance;
    [SerializeField] private TextMeshProUGUI playerModeText;

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
        EventManager.Instance.AddListener<PlayerSwitchModeEvent>(this.SetPlayerModeText);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerSwitchModeEvent>(this.SetPlayerModeText);
    }

    private void SetPlayerModeText(PlayerSwitchModeEvent e)
    {
        this.playerModeText.text = EnumConverter.StringFromPlayerMode(e.mode);
    }
}
