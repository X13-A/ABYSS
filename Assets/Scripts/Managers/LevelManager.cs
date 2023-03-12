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

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<SceneAboutToChangeEvent>(PrepareSceneChange);
        EventManager.Instance.AddListener<SceneReadyToChangeEvent>(LoadScene);
    }

    public void UnsubscribeEvents()
    {
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
        var scene = SceneManager.LoadSceneAsync(e.targetScene);

        scene.allowSceneActivation = false;
        progressBar.fillAmount = 0;

        loadingScreen.SetActive(true);

        do
        {
            await Task.Delay(1000);
            progressBar.fillAmount = scene.progress;
        }
        while (scene.progress < 0.9f);

        // Génération procédurale


        scene.allowSceneActivation = true;
        loadingScreen.SetActive(false);
    }
}
