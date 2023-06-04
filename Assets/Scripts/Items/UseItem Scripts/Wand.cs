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

    private float currentAttackDuration;
    private float attackStartTime;

    public float AttackElaspedTime => Time.time - attackStartTime;

    public void OnEnable()
    {
        attackStartTime = Time.time - 1000;
        currentAttackDuration = 0;
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
        EventManager.Instance.Raise(new AnimateItemEvent
        {
            itemId = ItemId.Wand,
            animations = new Dictionary<string, float>
            {
                { "startTrail", 0f },
                { "stopTrail", wandDuration * wandStartPercentage},
                { "startWandCast", wandDuration * wandStartPercentage }
            }
        });

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
