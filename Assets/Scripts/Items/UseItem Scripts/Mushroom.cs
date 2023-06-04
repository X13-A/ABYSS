using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour, IUseItem
{

    public float damage;

    public void Use()
    {
        float health = PlayerManager.Instance.Health - damage;
        EventManager.Instance.Raise(new HealthPlayerEvent { health = health });
    }
}
