using UnityEngine;
using SDD.Events;

public class GameManager : MonoBehaviour, IEventHandler
{
    private static GameManager m_Instance;
    public static GameManager Instance => m_Instance;

    [SerializeField]
    private GAMESTATE m_State;
    public GAMESTATE State => m_State;

    // Permet de suivre le menu "source" pour pouvoir y retourner
    // Exemple: Le menu settings peut être ouvert depuis le menu pause et le menu principal
    private GAMESTATE m_SourceMenu;
    public GAMESTATE SourceMenu => m_SourceMenu;


    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<EscapeButtonClickedEvent>(EscapeButtonClicked);
        EventManager.Instance.AddListener<PlayButtonClickedEvent>(PlayButtonClicked);
        EventManager.Instance.AddListener<ResumeButtonClickedEvent>(ResumeButtonClicked);
        EventManager.Instance.AddListener<MainMenuButtonClickedEvent>(MainMenuButtonClicked);
        EventManager.Instance.AddListener<SettingsButtonClickedEvent>(SettingsButtonClicked);
        EventManager.Instance.AddListener<CancelSettingsButtonClickedEvent>(ExitSettingsButtonClicked);
        EventManager.Instance.AddListener<QuitButtonClickedEvent>(QuitButtonClicked);
        EventManager.Instance.AddListener<CreditsButtonClickedEvent>(CreditsButtonClicked);
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
        EventManager.Instance.RemoveListener<CreditsButtonClickedEvent>(CreditsButtonClicked);
        EventManager.Instance.RemoveListener<SceneAboutToChangeEvent>(PrepareSceneChange);
    }

    private void OnEnable()
    {
        this.SubscribeEvents();
        this.SetState(m_State);
    }
    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void Awake()
    {
        if (!m_Instance)
        {
            m_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        switch (State)
        {
            case GAMESTATE.MAIN_MENU:
                MainMenu();
                break;
            case GAMESTATE.PAUSE_MENU:
                Pause();
                break;
            case GAMESTATE.SETTINGS_MENU:
                Settings();
                break;
            case GAMESTATE.PLAY:
                Play();
                break;
            case GAMESTATE.GAME_OVER:
                GameOver();
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            EventManager.Instance.Raise(new EscapeButtonClickedEvent());
        }
    }

    private void SetState(GAMESTATE newState)
    {
        // Update of settings menu
        switch (State)
        {
            case GAMESTATE.MAIN_MENU:
                m_SourceMenu = m_State;
                break;
            case GAMESTATE.PAUSE_MENU:
                m_SourceMenu = m_State;
                break;
            default:
                m_SourceMenu = GAMESTATE.PLAY;
                break;
        }

        m_State = newState;

        switch (m_State)
        {
            case GAMESTATE.MAIN_MENU:
                EventManager.Instance.Raise(new GameMainMenuEvent());
                break;
            case GAMESTATE.PLAY:
                EventManager.Instance.Raise(new GamePlayEvent());
                break;
            case GAMESTATE.SETTINGS_MENU:
                EventManager.Instance.Raise(new GameSettingsMenuEvent());
                break;
            case GAMESTATE.PAUSE_MENU:
                EventManager.Instance.Raise(new GamePauseMenuEvent());
                break;
            case GAMESTATE.GAME_OVER:
                EventManager.Instance.Raise(new GameOverEvent());
                break;
        }
    }

    private void InitGame()
    {
        // Load save data
    }

    #region MenuManager Actions
    private void Play()
    {
        InitGame();
        SetState(GAMESTATE.PLAY);
    }

    private void Resume()
    {
        SetState(GAMESTATE.PLAY);
    }

    private void Quit()
    {
        // Quit game
    }

    private void Credits()
    {
        EventManager.Instance.Raise(new SceneAboutToChangeEvent
        {
            levelGenerated = 0,
            targetScene = "Credits"
        });
    }

    private void MainMenu()
    {
        SetState(GAMESTATE.MAIN_MENU);
    }

    private void Pause()
    {
        SetState(GAMESTATE.PAUSE_MENU);
    }

    private void Settings()
    {
        SetState(GAMESTATE.SETTINGS_MENU);
    }

    private void GameOver()
    {
        SetState(GAMESTATE.GAME_OVER);
    }
    #endregion

    #region MenuManager event callbacks

    private void PrepareSceneChange(SceneAboutToChangeEvent e)
    {
        SetState(GAMESTATE.LOADING);
    }

    private void PlayButtonClicked(PlayButtonClickedEvent e)
    {
        Play();
    }

    private void ResumeButtonClicked(ResumeButtonClickedEvent e)
    {
        Resume();
    }

    private void EscapeButtonClicked(EscapeButtonClickedEvent e)
    {
        if (State == GAMESTATE.PLAY)
        {
            Pause();
        }
        else if (State == GAMESTATE.PAUSE_MENU)
        {
            Resume();
        }
    }

    private void SettingsButtonClicked(SettingsButtonClickedEvent e)
    {
        Settings();
    }

    private void ExitSettingsButtonClicked(CancelSettingsButtonClickedEvent e)
    {
        EventManager.Instance.Raise(new GameSaveSettingsEvent());
        switch (SourceMenu)
        {
            case GAMESTATE.MAIN_MENU:
                MainMenu();
                break;
            case GAMESTATE.PAUSE_MENU:
                Pause();
                break;
            default:
                Play();
                break;
        }
    }

    private void MainMenuButtonClicked(MainMenuButtonClickedEvent e)
    {
        EventManager.Instance.Raise(new SceneAboutToChangeEvent { targetScene = "Main Menu", levelGenerated = 0 });
    }

    private void QuitButtonClicked(QuitButtonClickedEvent e)
    {
        Quit();
    }

    private void CreditsButtonClicked(CreditsButtonClickedEvent e)
    {
        Credits();
    }
    #endregion
}
