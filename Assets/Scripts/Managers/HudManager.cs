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
    [SerializeField] private GameObject map;
    private GameObject HUD;

    public bool MapDisplayed
    {
        get
        {
            if (this.map == null) return false;
            Minimap mapScript = this.map.GetComponent<Minimap>();
            if (mapScript != null) return mapScript.Visibility;
            else return false;
        }
        set
        {
            if (this.map == null) return;
            Minimap mapScript = this.map.GetComponent<Minimap>();
            if (mapScript != null) mapScript.Visibility = value;
        }
    }

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

        HUD = GameObject.Find("HUD");
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
        EventManager.Instance.AddListener<GameMainMenuEvent>(this.CloseHud);
        EventManager.Instance.AddListener<GameSettingsMenuEvent>(this.CloseHud);
        EventManager.Instance.AddListener<GamePauseMenuEvent>(this.CloseHud);
        EventManager.Instance.AddListener<GameResumeEvent>(this.CloseHud);
        EventManager.Instance.AddListener<GameOverEvent>(this.CloseHud);
        EventManager.Instance.AddListener<GamePlayEvent>(this.OpenHud);
        EventManager.Instance.AddListener<ToggleMapEvent>(this.ToggleMap);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerSwitchModeEvent>(this.SetPlayerModeText);
        EventManager.Instance.RemoveListener<GameMainMenuEvent>(this.CloseHud);
        EventManager.Instance.RemoveListener<GameSettingsMenuEvent>(this.CloseHud);
        EventManager.Instance.RemoveListener<GamePauseMenuEvent>(this.CloseHud);
        EventManager.Instance.RemoveListener<GameResumeEvent>(this.CloseHud);
        EventManager.Instance.RemoveListener<GameOverEvent>(this.CloseHud);
        EventManager.Instance.RemoveListener<GamePlayEvent>(this.OpenHud);
        EventManager.Instance.AddListener<ToggleMapEvent>(this.ToggleMap);
    }

    private void ToggleMap(ToggleMapEvent e)
    {
        this.MapDisplayed = e.value;
    }

    #region Close HUD
    private void CloseHud(GameMainMenuEvent e)
    {
        HUD.SetActive(false);
    }
    private void CloseHud(GameSettingsMenuEvent e)
    {
        HUD.SetActive(false);
    }
    private void CloseHud(GamePauseMenuEvent e)
    {
        HUD.SetActive(false);
    }
    private void CloseHud(GameOverEvent e)
    {
        HUD.SetActive(false);
    }
    private void CloseHud(GameResumeEvent e)
    {
        HUD.SetActive(false);
    }
    #endregion

    private void OpenHud(GamePlayEvent e)
    {
        HUD.SetActive(true);
    }

    private void SetPlayerModeText(PlayerSwitchModeEvent e)
    {
        playerModeText.text = EnumConverter.StringFromPlayerMode(e.mode);
    }
}
