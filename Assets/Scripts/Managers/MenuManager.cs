using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using UnityEditor;

public class MenuManager : MonoBehaviour, IEventHandler
{
    [SerializeField] private GameObject m_MainMenuPanel;
    [SerializeField] private GameObject m_PauseMenuPanel;
    [SerializeField] private GameObject m_SettingsMenuPanel;
    [SerializeField] private GameObject m_GameOverPanel;
    private List<GameObject> m_Panels;

    private void OpenPanel(GameObject panel)
    {
        m_Panels.ForEach(item => item.SetActive(panel == item));
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<GameMainMenuEvent>(GameMainMenu);
        EventManager.Instance.AddListener<GamePauseMenuEvent>(GamePauseMenu);
        EventManager.Instance.AddListener<GameSettingsMenuEvent>(GameSettingsMenu);
        EventManager.Instance.AddListener<GamePlayEvent>(GamePlay);
        EventManager.Instance.AddListener<GameResumeEvent>(GameResume);
        EventManager.Instance.AddListener<GameOverEvent>(GameOver);
        EventManager.Instance.AddListener<GameSaveSettingsEvent>(GameSaveSettings);
        EventManager.Instance.AddListener<GameCancelSettingsEvent>(GameCancelSettings);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<GameMainMenuEvent>(GameMainMenu);
        EventManager.Instance.RemoveListener<GamePauseMenuEvent>(GamePauseMenu);
        EventManager.Instance.RemoveListener<GameSettingsMenuEvent>(GameSettingsMenu);
        EventManager.Instance.RemoveListener<GamePlayEvent>(GamePlay);
        EventManager.Instance.RemoveListener<GameResumeEvent>(GameResume);
        EventManager.Instance.RemoveListener<GameOverEvent>(GameOver);
        EventManager.Instance.RemoveListener<GameSaveSettingsEvent>(GameSaveSettings);
        EventManager.Instance.RemoveListener<GameCancelSettingsEvent>(GameCancelSettings);
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void Awake()
    {
        m_Panels = new List<GameObject>(
            new GameObject[]
            {
                m_MainMenuPanel,
                m_PauseMenuPanel,
                m_SettingsMenuPanel,
                m_GameOverPanel
            }
        );
    }

    // GameManager events' callbacks

    private void GameSaveSettings(GameSaveSettingsEvent e)
    {
        CursorManager.Instance.SetCursorType(CursorType.MENU);
        GAMESTATE source = GameManager.Instance.SourceMenu;
        switch (source)
        {
            case GAMESTATE.MAIN_MENU:
                OpenPanel(m_MainMenuPanel);
                break;
            case GAMESTATE.PAUSE_MENU:
                OpenPanel(m_PauseMenuPanel);
                break;
            default:
                OpenPanel(null);
                break;
        }
    }

    private void GameCancelSettings(GameCancelSettingsEvent e)
    {
        CursorManager.Instance.SetCursorType(CursorType.MENU);
        GAMESTATE source = GameManager.Instance.SourceMenu;
        switch (source)
        {
            case GAMESTATE.MAIN_MENU:
                OpenPanel(m_MainMenuPanel);
                break;
            case GAMESTATE.PAUSE_MENU:
                OpenPanel(m_PauseMenuPanel);
                break;
            default:
                OpenPanel(null);
                break;
        }
    }

    private void GameMainMenu(GameMainMenuEvent e)
    {
        CursorManager.Instance.SetCursorType(CursorType.MENU);
        if (m_MainMenuPanel == null)
        {
            return;
        }

        OpenPanel(m_MainMenuPanel);
    }

    private void GamePauseMenu(GamePauseMenuEvent e)
    {
        CursorManager.Instance.SetCursorType(CursorType.MENU);
        OpenPanel(m_PauseMenuPanel);
    }

    private void GameSettingsMenu(GameSettingsMenuEvent e)
    {
        CursorManager.Instance.SetCursorType(CursorType.MENU);
        OpenPanel(m_SettingsMenuPanel);

    }

    private void GamePlay(GamePlayEvent e)
    {
        CursorManager.Instance.SetCursorType(CursorType.MELEE);
        OpenPanel(null);
    }

    private void GameResume(GameResumeEvent e)
    {
        CursorManager.Instance.SetCursorType(CursorType.MELEE);
        OpenPanel(null);
    }

    private void GameOver(GameOverEvent e)
    {
        CursorManager.Instance.SetCursorType(CursorType.MENU);
        OpenPanel(m_GameOverPanel);
    }

    // UI events' callbacks
    public void PlayButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new PlayButtonClickedEvent());
    }

    public void QuitButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new QuitButtonClickedEvent());
    }

    public void SettingsButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new SettingsButtonClickedEvent());
    }

    public void MainMenuButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new MainMenuButtonClickedEvent());
    }

    public void ResumeButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new ResumeButtonClickedEvent());
    }

    public void SaveSettingsButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new SaveSettingsButtonClickedEvent());
    }

    public void CancelSettingsButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new CancelSettingsButtonClickedEvent());
    }
}
