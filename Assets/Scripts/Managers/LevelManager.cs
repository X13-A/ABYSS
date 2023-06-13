using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour, IEventHandler
{
    public static LevelManager m_Instance;
    public static LevelManager Instance => m_Instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject loadingScreenProgressPanel;
    [SerializeField] private Image progressBar;
    [SerializeField] private int currentLevel;
    private MapGeneration generator;

    public int CurrentLevel
    {
        get => currentLevel;
        set => currentLevel = value;
    }
    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<LoadingProgressUpdateEvent>(UpdateProgress);
        EventManager.Instance.AddListener<SceneAboutToChangeEvent>(PrepareSceneChange);
        EventManager.Instance.AddListener<SceneReadyToChangeEvent>(LoadScene);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<LoadingProgressUpdateEvent>(UpdateProgress);
        EventManager.Instance.RemoveListener<SceneAboutToChangeEvent>(PrepareSceneChange);
        EventManager.Instance.RemoveListener<SceneReadyToChangeEvent>(LoadScene);
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
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        generator = GetComponent<MapGeneration>();
    }

    private void UpdateProgress(LoadingProgressUpdateEvent e)
    {
        progressBar.fillAmount = e.progress;
    }

    private void PrepareSceneChange(SceneAboutToChangeEvent e)
    {
        // Préparation au changement de scene
        StartCoroutine(TransitionManager.Instance.FadeOut(1f, () =>
        {
            EventManager.Instance.Raise(new SceneReadyToChangeEvent { targetScene = e.targetScene, levelGenerated = e.levelGenerated, displayLoading = e.displayLoading });
        }));
    }

    private async void LoadScene(SceneReadyToChangeEvent e)
    {
        var operation = SceneManager.LoadSceneAsync(e.targetScene);

        operation.allowSceneActivation = false;
        progressBar.fillAmount = 0;
        loadingScreen.SetActive(true);
        loadingScreenProgressPanel.SetActive(e.displayLoading);
        do
        {
            EventManager.Instance.Raise(new LoadingProgressUpdateEvent { progress = operation.progress, message = "Loading scene" });
        }
        while (operation.progress < 0.9f);

        currentLevel = e.levelGenerated;

        // Génération proécdurale
        if (currentLevel != 0)
        {
            var scene = SceneManager.GetSceneByName(e.targetScene);
            GameObject map = await generator.GenerateMap(currentLevel - 1);
            map.AddComponent<DisableOnLoad>();

            DontDestroyOnLoad(map);

            SceneManager.sceneLoaded += OnSceneLoaded;
            operation.allowSceneActivation = true;

            void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                SceneManager.MoveGameObjectToScene(map, scene);
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }
        else
        {
            operation.allowSceneActivation = true;
        }
    }
}
