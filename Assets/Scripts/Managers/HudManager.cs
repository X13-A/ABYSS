using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using TMPro;

public class HudManager : MonoBehaviour, IEventHandler
{
    public static HudManager Instance;
    [SerializeField] private TextMeshProUGUI playerModeText;

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

    private void SetPlayerModeText(PlayerSwitchModeEvent e)
    {
        this.playerModeText.text = EnumConverter.StringFromPlayerMode(e.mode);
    }
}
