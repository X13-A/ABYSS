using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossSword : MonoBehaviour
{
    [SerializeField] private float damage = 20;
    [SerializeField] private Damager damager;
    [SerializeField] private AudioSource audioSource;

    public void StartDamage()
    {
        damager.EnableDamage(damage);
    }
    public void EndDamage()
    {
        damager.StopDamage();
    }
    public void PlaySound()
    {
        audioSource.PlayOneShot(audioSource.clip);
    }
}
