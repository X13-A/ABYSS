using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatOrDrinkingSound : MonoBehaviour
{
    [SerializeField] private AudioClip drinkingSound;
    [SerializeField] private AudioClip eatingSound;
    private AudioSource audioSource;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnEnable()
    {
        EventManager.Instance.AddListener<PlayerDrinkEvent>(LaunchDrinkSound);
        EventManager.Instance.AddListener<PlayerEatEvent>(LaunchEatSound);
    }

    public void OnDisable()
    {
        EventManager.Instance.RemoveListener<PlayerDrinkEvent>(LaunchDrinkSound);
        EventManager.Instance.RemoveListener<PlayerEatEvent>(LaunchEatSound);
    }

    public void LaunchDrinkSound(PlayerDrinkEvent e)
    {
        audioSource.PlayOneShot(drinkingSound);
    }

    public void LaunchEatSound(PlayerEatEvent e)
    {
        audioSource.PlayOneShot(eatingSound);
    }
}
