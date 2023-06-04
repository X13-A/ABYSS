using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour, IHoldingItem
{
    public void OnHolding()
    {
        EventManager.Instance.Raise(new ToggleMapEvent { value = true });
    }

    public void OnStopHolding()
    {
        EventManager.Instance.Raise(new ToggleMapEvent { value = false });
    }
}
