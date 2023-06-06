using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : MonoBehaviour, IUseItem
{
    [SerializeField] private GameObject magicProjectilePrefab;
    [SerializeField] private GameObject magicProjectilePosition;
    [SerializeField] private float magicProjectileSpeed;
    [SerializeField] private float magicProjectileDamage;
    [SerializeField] private float wandDuration;
    [SerializeField] private float wandStartPercentage;
    private WandAnimation wandAnimation;
    private AudioSource wandAudioSource;

    private float currentAttackDuration;
    private float attackStartTime;

    public float AttackElaspedTime => Time.time - attackStartTime;

    public void OnEnable()
    {
        attackStartTime = Time.time - 1000;
        currentAttackDuration = 0;
        wandAnimation = GetComponent<WandAnimation>();
        wandAudioSource = GetComponent<AudioSource>();
    }
    public void Use()
    {
        if (AttackElaspedTime < currentAttackDuration) return;
        currentAttackDuration = wandDuration;
        attackStartTime = Time.time;
        EventManager.Instance.Raise(new AnimateAttackEvent
        {
            name = "Wand Attack",
            animationDuration = wandDuration
        });
        wandAnimation.Animate(wandDuration * currentAttackDuration);
        wandAudioSource.Play();
        StartCoroutine(CoroutineUtil.DelayAction(wandDuration * wandStartPercentage, () =>
        {
            Projectile projectile = Instantiate(magicProjectilePrefab, magicProjectilePosition.transform.position, Quaternion.Euler(
                magicProjectilePosition.transform.rotation.eulerAngles.x,
                magicProjectilePosition.transform.rotation.eulerAngles.y,
                magicProjectilePosition.transform.rotation.eulerAngles.z)
            ).GetComponent<Projectile>();
            projectile.Init(magicProjectileSpeed, magicProjectileDamage);
        }));
    }
}
