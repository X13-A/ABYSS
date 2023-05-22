using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour, IPlayerCollider
{
    private IInventoryItem mItemToPickup = null;

    private void Update()
    {
        if (mItemToPickup != null && Input.GetKeyUp(KeyCode.F))
        {
            InventoryManager.Instance.AddItem(mItemToPickup);
            mItemToPickup.OnPickup();
            //Hud.CloseMessagePanel();
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

            //Hud.OpenMessagePanel("");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Hud.CloseMessagePanel();
        mItemToPickup = null;
    }
}

