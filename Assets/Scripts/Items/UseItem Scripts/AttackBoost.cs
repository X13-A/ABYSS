using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBoost : MonoBehaviour, IUseItem
{
    public bool Use()
    {
        EventManager.Instance.Raise(new StartAttackBoostPlayerEvent { });
        EventManager.Instance.Raise(new PlayerDrinkEvent { });
        return true;
    }
}
