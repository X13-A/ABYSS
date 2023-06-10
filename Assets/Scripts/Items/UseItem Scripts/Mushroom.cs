using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour, IUseItem
{
     [SerializeField] private float damage;

    public bool Use()
    {
        EventManager.Instance.Raise(new DamagePlayerEvent { damage = damage });
        EventManager.Instance.Raise(new PlayerEatEvent { });
        return true;
    }
}
