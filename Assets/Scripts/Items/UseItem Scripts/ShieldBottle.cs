using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBottle : MonoBehaviour, IUseItem
{
    [SerializeField] private float shield;

    public bool Use()
    {
        EventManager.Instance.Raise(new SetShieldPlayerEvent { shield = shield });
        EventManager.Instance.Raise(new PlayerDrinkEvent { });
        return true;
    }
}
