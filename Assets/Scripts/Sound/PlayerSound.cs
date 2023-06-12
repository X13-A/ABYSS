using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
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
        if (index >= stepSound.Length) return;
        if (stepSound[index] == null) return;
        audioSource.PlayOneShot(stepSound[index]);
    }

    public void LaunchPickaxeSound()
    {
        if (pickaxeSound == null) return;
        audioSource.PlayOneShot(pickaxeSound);
    }
}
