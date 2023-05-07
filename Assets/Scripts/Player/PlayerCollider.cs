using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour, IPlayerCollider
{
    public Inventory inventory;

    void OnTriggerEnter(Collider other)
    {
        IInventoryItem item = other.GetComponent<IInventoryItem>();

        if (item != null)
        {
            inventory.AddItem(item);
        }
    }
}

