using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour, IEventHandler
{
    public static LevelManager Instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image progressBar;
    private MapGeneration generator;
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
        if (Instance == null)
        {
            Instance = this;
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
        StartCoroutine(TransitionManager.Instance.FadeOut(1f, () => {
            EventManager.Instance.Raise(new SceneReadyToChangeEvent { targetScene = e.targetScene });
        }));
    }

    private async void LoadScene(SceneReadyToChangeEvent e)
    {
        var operation = SceneManager.LoadSceneAsync(e.targetScene);

        operation.allowSceneActivation = false;
        progressBar.fillAmount = 0;
        loadingScreen.SetActive(true);
        do
        {
            EventManager.Instance.Raise(new LoadingProgressUpdateEvent { progress = operation.progress, message = "Loading scene" });
        }
        while (operation.progress < 0.9f);

        // Génération procédurale
        var scene = SceneManager.GetSceneByName(e.targetScene);
        GameObject map = await generator.GenerateMap();
        DontDestroyOnLoad(map);

        SceneManager.sceneLoaded += OnSceneLoaded;
        operation.allowSceneActivation = true;

        loadingScreen.SetActive(false);
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.MoveGameObjectToScene(map, scene);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}