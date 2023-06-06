using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleHealth : MonoBehaviour, IUseItem
{
    [SerializeField] private float health;

    public void Use()
    {
        EventManager.Instance.Raise(new HealthPlayerEvent { health = health });
    }
}
