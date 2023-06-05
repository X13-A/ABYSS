using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour, IUseItem
{

    public float damage;

    public void Use()
    {
        EventManager.Instance.Raise(new DamagePlayerEvent { damage = damage });
    }
}
