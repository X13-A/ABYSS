using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBottle : MonoBehaviour, IUseItem
{
    [SerializeField] private float shield;

    public void Use()
    {
        Debug.Log("SHIELLLDS");
    }
}
