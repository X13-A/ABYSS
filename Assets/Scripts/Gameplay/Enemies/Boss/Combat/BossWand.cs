using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWand : MonoBehaviour
{
    [SerializeField] private float damage = 20;
    [SerializeField] private float projectileSpeed = 5;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawn;
    [SerializeField] private AudioSource audioSource;

    public void FireProjectile()
    {
        Instantiate(projectilePrefab, projectileSpawn.position, Quaternion.Euler(
            projectileSpawn.transform.rotation.eulerAngles.x,
            projectileSpawn.transform.rotation.eulerAngles.y,
            projectileSpawn.transform.rotation.eulerAngles.z)
        ).GetComponent<Projectile>().Init(projectileSpeed, damage);

        audioSource.PlayOneShot(audioSource.clip);
    }
}
