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
            if (map == null) return false;
            Minimap mapScript = map.GetComponent<Minimap>();
            if (mapScript != null) return mapScript.Visibility;
            else return false;
        }
        set
        {
            if (map == null) return;
            Minimap mapScript = map.GetComponent<Minimap>();
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
        EventManager.Instance.AddListener<PlayerSwitchModeEvent>(SetPlayerModeText);
        EventManager.Instance.AddListener<GameMainMenuEvent>(CloseHud);
        EventManager.Instance.AddListener<GameSettingsMenuEvent>(CloseHud);
        EventManager.Instance.AddListener<GamePauseMenuEvent>(CloseHud);
        EventManager.Instance.AddListener<GameResumeEvent>(CloseHud);
        EventManager.Instance.AddListener<GameOverEvent>(CloseHud);
        EventManager.Instance.AddListener<SceneAboutToChangeEvent>(CloseHud);
        EventManager.Instance.AddListener<GamePlayEvent>(OpenHud);
        EventManager.Instance.AddListener<ToggleMapEvent>(ToggleMap);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerSwitchModeEvent>(SetPlayerModeText);
        EventManager.Instance.RemoveListener<GameMainMenuEvent>(CloseHud);
        EventManager.Instance.RemoveListener<GameSettingsMenuEvent>(CloseHud);
        EventManager.Instance.RemoveListener<GamePauseMenuEvent>(CloseHud);
        EventManager.Instance.RemoveListener<GameResumeEvent>(CloseHud);
        EventManager.Instance.RemoveListener<GameOverEvent>(CloseHud);
        EventManager.Instance.RemoveListener<SceneAboutToChangeEvent>(CloseHud);
        EventManager.Instance.RemoveListener<GamePlayEvent>(OpenHud);
        EventManager.Instance.AddListener<ToggleMapEvent>(ToggleMap);
    }

    private void ToggleMap(ToggleMapEvent e)
    {
        MapDisplayed = e.value;
    }

    #region Close HUD
    private void CloseHud(SceneAboutToChangeEvent e)
    {
        HUD.SetActive(false);
    }
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
