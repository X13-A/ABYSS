using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using UnityEngine.Rendering.VirtualTexturing;

public class SceneBoss : MonoBehaviour
{
    [SerializeField] GameObject wall;
    [SerializeField] private GameObject[] toDeactivate;
    [SerializeField] private AudioSource audioSource;

    private void OnEnable()
    {
        SubscribeEvents();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<PlayerDetectorEvent>(WallEnable);
        EventManager.Instance.AddListener<StartBossScreamerEvent>(StartMusic);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<PlayerDetectorEvent>(WallEnable);
        EventManager.Instance.RemoveListener<StartBossScreamerEvent>(StartMusic);
    }

    private void WallEnable(PlayerDetectorEvent e)
    {
        wall.SetActive(true);
        foreach (GameObject obj in toDeactivate)
        {
            obj.SetActive(false);
        }
    }

    private void StartMusic(StartBossScreamerEvent e)
    {
        audioSource.Play();
    }
}
