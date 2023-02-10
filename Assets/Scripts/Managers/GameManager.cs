using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using UnityEngine.SocialPlatforms;
using System;

public enum GAMESTATE { play, gameOver, mainMenu, pauseMenu, settingsMenu}

public class GameManager : MonoBehaviour, IEventHandler
{
    private static GameManager m_Instance;
    public static GameManager Instance { get { return m_Instance; } }

    [SerializeField] // Choix du statut de base dans la scène
    GAMESTATE m_State;
    GAMESTATE m_SourceMenu; // Permet de suivre le menu "source" pour pouvoir y retourner
                          // Exemple: Le menu settings peut être ouvert depuis le menu pause et le menu principal

    public GAMESTATE State { get { return m_State; } }
    public GAMESTATE SourceMenu { get { return m_SourceMenu; } }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<EscapeButtonClickedEvent>(EscapeButtonClicked);
        EventManager.Instance.AddListener<PlayButtonClickedEvent>(PlayButtonClicked);
        EventManager.Instance.AddListener<ResumeButtonClickedEvent>(ResumeButtonClicked);
        EventManager.Instance.AddListener<MainMenuButtonClickedEvent>(MainMenuButtonClicked);
        EventManager.Instance.AddListener<SettingsButtonClickedEvent>(SettingsButtonClicked);
        EventManager.Instance.AddListener<SaveSettingsButtonClickedEvent>(SaveSettingsButtonClicked);
        EventManager.Instance.AddListener<CancelSettingsButtonClickedEvent>(CancelSettingsButtonClicked);
        EventManager.Instance.AddListener<QuitButtonClickedEvent>(QuitButtonClicked);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<EscapeButtonClickedEvent>(EscapeButtonClicked);
        EventManager.Instance.RemoveListener<PlayButtonClickedEvent>(PlayButtonClicked);
        EventManager.Instance.RemoveListener<ResumeButtonClickedEvent>(ResumeButtonClicked);
        EventManager.Instance.RemoveListener<MainMenuButtonClickedEvent>(MainMenuButtonClicked);
        EventManager.Instance.RemoveListener<SettingsButtonClickedEvent>(SettingsButtonClicked);
        EventManager.Instance.RemoveListener<SaveSettingsButtonClickedEvent>(SaveSettingsButtonClicked);
        EventManager.Instance.RemoveListener<CancelSettingsButtonClickedEvent>(CancelSettingsButtonClicked);
        EventManager.Instance.RemoveListener<QuitButtonClickedEvent>(QuitButtonClicked);
    }

    void OnEnable()
    {
        SubscribeEvents();
    }
    void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void Awake()
    {
        if (!m_Instance) m_Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        MainMenu();
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            EventManager.Instance.Raise(new EscapeButtonClickedEvent());
        }
    }

    void SetState(GAMESTATE newState)
    {

        // Mise à jour du menu source
        switch (State)
        {
            case GAMESTATE.mainMenu:
                m_SourceMenu = m_State;
                break;
            case GAMESTATE.pauseMenu:
                m_SourceMenu = m_State;
                break;
            default:
                m_SourceMenu = GAMESTATE.play;
                break;
        }

        m_State = newState;

        switch (m_State)
        {
            case GAMESTATE.mainMenu:
                EventManager.Instance.Raise(new GameMainMenuEvent());
                break;
            case GAMESTATE.play:
                EventManager.Instance.Raise(new GamePlayEvent());
                break;
            case GAMESTATE.settingsMenu:
                EventManager.Instance.Raise(new GameSettingsMenuEvent());
                break;
            case GAMESTATE.pauseMenu:
                EventManager.Instance.Raise(new GamePauseMenuEvent());
                break;
            case GAMESTATE.gameOver:
                EventManager.Instance.Raise(new GameOverEvent());
                break;
        }
    }

    void InitGame()
    {
        // Load save data
    }

    #region MenuManager Actions
    void Play()
    {
        InitGame();
        SetState(GAMESTATE.play);
    }

    void Resume()
    {
        SetState(GAMESTATE.play);
    }

    void Quit()
    {
        // Quit game
    }

    void MainMenu()
    {
        SetState(GAMESTATE.mainMenu);
    }

    void Pause()
    {
        SetState(GAMESTATE.pauseMenu);
    }

    void Settings()
    {
        SetState(GAMESTATE.settingsMenu);
    }
    #endregion

    #region MenuManager event callbacks
    void PlayButtonClicked(PlayButtonClickedEvent e)
    {
        Play();
    }

    void ResumeButtonClicked(ResumeButtonClickedEvent e)
    {
        Resume();
    }

    void EscapeButtonClicked(EscapeButtonClickedEvent e)
    {
        if (State == GAMESTATE.play)
        {
            Pause();
        }
        else if (State == GAMESTATE.pauseMenu)
        {
            Resume();
        }
    }

    void SettingsButtonClicked(SettingsButtonClickedEvent e)
    {
        Settings();
    }

    void SaveSettingsButtonClicked(SaveSettingsButtonClickedEvent e)
    {
        // Todo: Save settings
        EventManager.Instance.Raise(new GameSaveSettingsEvent());
        switch (SourceMenu)
        {
            case GAMESTATE.mainMenu:
                MainMenu();
                break;
            case GAMESTATE.pauseMenu:
                Pause();
                break;
            default:
                Play();
                break;
        }
    }

    void CancelSettingsButtonClicked(CancelSettingsButtonClickedEvent e)
    {
        EventManager.Instance.Raise(new GameSaveSettingsEvent());
        switch (SourceMenu)
        {
            case GAMESTATE.mainMenu:
                MainMenu();
                break;
            case GAMESTATE.pauseMenu:
                Pause();
                break;
            default:
                Play();
                break;
        }
    }

    void MainMenuButtonClicked(MainMenuButtonClickedEvent e)
    {
        MainMenu();
    }

    void QuitButtonClicked(QuitButtonClickedEvent e)
    {
        Quit();
    }
    #endregion
}
