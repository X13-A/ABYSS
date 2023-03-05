using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class CameraManager : MonoBehaviour, IEventHandler
{
    [SerializeField] GameObject m_PlayCamera;
    [SerializeField] GameObject m_MainMenuCamera;
    List<GameObject> m_Cameras;

    void SwitchCamera(GameObject camera)
    {
        m_Cameras.ForEach(item => item.SetActive(camera == item));
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<GameMainMenuEvent>(GameMainMenu);
        EventManager.Instance.AddListener<GamePlayEvent>(GamePlay);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<GameMainMenuEvent>(GameMainMenu);
        EventManager.Instance.RemoveListener<GamePlayEvent>(GamePlay);
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
        m_Cameras = new List<GameObject>(
            new GameObject[]
            {
                m_PlayCamera,
                m_MainMenuCamera,
            }
        );
    }

    // CameraManager events' callbacks
    void GameMainMenu(GameMainMenuEvent e)
    {
        if (m_MainMenuCamera == null) return;
        SwitchCamera(m_MainMenuCamera);
    }

    void GamePlay(GamePlayEvent e)
    {
        SwitchCamera(m_PlayCamera);
    }
}