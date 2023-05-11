using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour, IPlayerCollider
{
    public Inventory inventory;

    public HUD Hud;

    private IInventoryItem mItemToPickup = null;

    private void Update()
    {
        if (mItemToPickup != null && Input.GetKeyUp(KeyCode.F))
        {
            inventory.AddItem(mItemToPickup);
            mItemToPickup.OnPickup();
            Hud.CloseMessagePanel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IInventoryItem item = other.GetComponent<IInventoryItem>();

        if (item != null)
        {
            mItemToPickup = item;
            //inventory.AddItem(item);
            //item.OnPickup();

            Hud.OpenMessagePanel("");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInventoryItem item = other.GetComponent<IInventoryItem>();

        if (item != null)
        {
            Hud.CloseMessagePanel();
            mItemToPickup = null;
        }
    }
}

