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

    [SerializeField] GameObject loadingScreen;
    [SerializeField] Image progressBar;
    MapGeneration generator;
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
            Destroy(this.gameObject);
        }
        this.generator = GetComponent<MapGeneration>();
    }

    void UpdateProgress(LoadingProgressUpdateEvent e)
    {
        this.progressBar.fillAmount = e.progress;
    }

    void PrepareSceneChange(SceneAboutToChangeEvent e)
    {
        // Préparation au changement de scene
        StartCoroutine(TransitionManager.Instance.FadeOut(1f, () => {
            EventManager.Instance.Raise(new SceneReadyToChangeEvent { targetScene = e.targetScene });
        }));
    }

    async void LoadScene(SceneReadyToChangeEvent e)
    {
        var operation = SceneManager.LoadSceneAsync(e.targetScene);

        operation.allowSceneActivation = false;
        this.progressBar.fillAmount = 0;
        this.loadingScreen.SetActive(true);
        do
        {
            EventManager.Instance.Raise(new LoadingProgressUpdateEvent { progress = operation.progress, message = "Loading scene" });
        }
        while (operation.progress < 0.9f);

        // Génération procédurale
        var scene = SceneManager.GetSceneByName(e.targetScene);
        GameObject map = await this.generator.GenerateMap();
        DontDestroyOnLoad(map);

        SceneManager.sceneLoaded += OnSceneLoaded;
        operation.allowSceneActivation = true;

        this.loadingScreen.SetActive(false);
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.MoveGameObjectToScene(map, scene);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}