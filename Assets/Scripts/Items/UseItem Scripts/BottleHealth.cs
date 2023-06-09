using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleHealth : MonoBehaviour, IUseItem
{
    [SerializeField] private float health;

    public bool Use()
    {
        EventManager.Instance.Raise(new CarePlayerEvent { care = health });
        EventManager.Instance.Raise(new PlayerDrinkEvent { });
        return true;
    }
}
