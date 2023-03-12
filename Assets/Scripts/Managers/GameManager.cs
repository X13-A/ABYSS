using UnityEngine;
using SDD.Events;

public enum GAMESTATE { play, gameOver, mainMenu, pauseMenu, settingsMenu, loading}

public class GameManager : MonoBehaviour, IEventHandler
{
    private static GameManager m_Instance;
    public static GameManager Instance { get { return m_Instance; } }

    [SerializeField]
    GAMESTATE m_State;
    public GAMESTATE State { get { return m_State; } }

    // Permet de suivre le menu "source" pour pouvoir y retourner
    // Exemple: Le menu settings peut être ouvert depuis le menu pause et le menu principal
    GAMESTATE m_SourceMenu;
    public GAMESTATE SourceMenu { get { return m_SourceMenu; } }


    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<EscapeButtonClickedEvent>(EscapeButtonClicked);
        EventManager.Instance.AddListener<PlayButtonClickedEvent>(PlayButtonClicked);
        EventManager.Instance.AddListener<ResumeButtonClickedEvent>(ResumeButtonClicked);
        EventManager.Instance.AddListener<MainMenuButtonClickedEvent>(MainMenuButtonClicked);
        EventManager.Instance.AddListener<SettingsButtonClickedEvent>(SettingsButtonClicked);
        EventManager.Instance.AddListener<CancelSettingsButtonClickedEvent>(ExitSettingsButtonClicked);
        EventManager.Instance.AddListener<QuitButtonClickedEvent>(QuitButtonClicked);
        EventManager.Instance.AddListener<SceneAboutToChangeEvent>(PrepareSceneChange);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<EscapeButtonClickedEvent>(EscapeButtonClicked);
        EventManager.Instance.RemoveListener<PlayButtonClickedEvent>(PlayButtonClicked);
        EventManager.Instance.RemoveListener<ResumeButtonClickedEvent>(ResumeButtonClicked);
        EventManager.Instance.RemoveListener<MainMenuButtonClickedEvent>(MainMenuButtonClicked);
        EventManager.Instance.RemoveListener<SettingsButtonClickedEvent>(SettingsButtonClicked);
        EventManager.Instance.RemoveListener<CancelSettingsButtonClickedEvent>(ExitSettingsButtonClicked);
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
        switch (State)
        {
            case GAMESTATE.mainMenu:
                MainMenu();
                break;
            case GAMESTATE.pauseMenu:
                Pause();
                break;
            case GAMESTATE.settingsMenu:
                Settings();
                break;
            case GAMESTATE.play:
                Play();
                break;
            case GAMESTATE.gameOver:
                GameOver();
                break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            EventManager.Instance.Raise(new EscapeButtonClickedEvent());
        }
    }

    void SetState(GAMESTATE newState)
    {
        // Update of settings menu
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

    void GameOver()
    {
        SetState(GAMESTATE.gameOver);
    }
    #endregion

    #region MenuManager event callbacks

    void PrepareSceneChange(SceneAboutToChangeEvent e)
    {
        SetState(GAMESTATE.loading);
    }

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

    void ExitSettingsButtonClicked(CancelSettingsButtonClickedEvent e)
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
