using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private Damager damager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float damage;
    [SerializeField] private float variant_damage;

    public void StartDamage(int variant)
    {
        float damage = variant == 0 ? this.damage : variant_damage;
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
