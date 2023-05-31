using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryInputs : MonoBehaviour
{
    private int activeSlot;

    private void HandleSlotChange()
    {
        float mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScrollWheel == 0) return;
        if (mouseScrollWheel < 0) this.activeSlot = (this.activeSlot + 1) % InventoryManager2.Instance.SlotCount;
        if (mouseScrollWheel > 0) this.activeSlot = (this.activeSlot - 1) % InventoryManager2.Instance.SlotCount;
        if (this.activeSlot < 0) this.activeSlot += InventoryManager2.Instance.SlotCount;
        EventManager.Instance.Raise(new SwitchSlotEvent { slot = this.activeSlot });
    }

    private void HandleItemPickup()
    {
        if (Input.GetButton("Pickup Item"))
        {
            EventManager.Instance.Raise(new PickupKeyPressedEvent {});
        }
    }

    private void HandleItemDrop()
    {
        if (Input.GetButtonDown("Drop Item"))
        {
            EventManager.Instance.Raise(new DropKeyPressedEvent {});
        }
    }

    private void HandleItemUse()
    {
        if (Input.GetButtonDown("Use Item"))
        {
            EventManager.Instance.Raise(new UseKeyPressedEvent {});
        }
    }

    private void Update()
    {
        this.HandleSlotChange();
        this.HandleItemUse();
        this.HandleItemDrop();
        this.HandleItemPickup();
    }
}
