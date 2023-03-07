using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum AttackType { Melee, Ranged };
public class PlayerCombat : MonoBehaviour, IEventHandler
{
    [SerializeField] GameObject meleeDamager;
    [SerializeField] float meleeDamage;
    [SerializeField] float meleeDuration;

    float attackStartTime;
    public float AttackElaspedTime 
    {
        get 
        {
            return Time.time - attackStartTime; 
        } 
    }

    public void SubscribeEvents()
    {
        //EventManager.Instance.AddListener<PlayerAttackEvent>(MeleeAttack);
    }

    public void UnsubscribeEvents()
    {
        //EventManager.Instance.RemoveListener<PlayerAttackEvent>(MeleeAttack);
    }

    void OnEnable()
    {
        SubscribeEvents();
        // startTime est au passé par défaut pour laisser le joueur attaquer quand il vient d'apparaitre
        attackStartTime = Time.time - 1000;
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }

    void Update()
    {
        if (GameManager.Instance.State != GAMESTATE.play) return;
        if (Input.GetButtonDown("Fire1") && AttackElaspedTime > meleeDuration)
        {
            EventManager.Instance.Raise(new PlayerAttackEvent
            {
                type = AttackType.Melee,
                damage = meleeDamage,
                duration = meleeDuration,
            });
            attackStartTime = Time.time;
        }
    }
}
