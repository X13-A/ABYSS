using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : MonoBehaviour, IUseItem
{
    [SerializeField] private float pickaxeDamage;
    [SerializeField] private float pickaxeDuration;

    private float currentAttackDuration;
    private float attackStartTime;
    private AudioSource audioSource;

    public float AttackElaspedTime => Time.time - attackStartTime;

    private void OnEnable()
    {
        attackStartTime = Time.time - 1000;
        currentAttackDuration = 0;
        audioSource = GetComponent<AudioSource>();
    }

    public void Use()
    {
        if (AttackElaspedTime < currentAttackDuration) return;
        currentAttackDuration = pickaxeDuration / PlayerManager.Instance.PlayerAttackSpeedMultiplier;
        attackStartTime = Time.time;

        EventManager.Instance.Raise(new AnimateAttackEvent
        {
            name = "Pickaxe Attack",
            animationDuration = pickaxeDuration / PlayerManager.Instance.PlayerAttackSpeedMultiplier
        });
        audioSource.Play();
        RaycastHit hit = AimUtil.Instance.Aim(~(1 << LayerMask.NameToLayer("Aim")));
        if (hit.collider)
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null) damageable.Damage(pickaxeDamage, AttackType.PICKAXE);
        }
    }
}
