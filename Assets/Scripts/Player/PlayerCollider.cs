using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerCollider : MonoBehaviour, IPlayerCollider
{
    private IInventoryItem mItemToPickup = null;

    private void Update()
    {
        if (mItemToPickup != null && Input.GetKeyUp(KeyCode.F))
        {
            InventoryManager.Instance.AddItem(mItemToPickup);
            mItemToPickup.OnPickup();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IInventoryItem item = other.GetComponent<IInventoryItem>();
        if (item != null)
        {
            mItemToPickup = item;

            EventManager.Instance.Raise(new ItemCollideWithPlayerEvent { item = item });
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInventoryItem item = other.GetComponent<IInventoryItem>();
        if (item != null)
        {
            EventManager.Instance.Raise(new ItemEndCollideWithPlayerEvent { });

            mItemToPickup = null;
        }
    }
}

