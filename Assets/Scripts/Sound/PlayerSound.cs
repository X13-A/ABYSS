using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] stepSound;
    [SerializeField] private AudioClip pickaxeSound;

    private AudioSource audioSource;
    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void LaunchStepSound(int index)
    {
        audioSource.PlayOneShot(stepSound[index]);
    }

    public void LaunchPickaxeSound()
    {
        audioSource.PlayOneShot(pickaxeSound);
    }
}
