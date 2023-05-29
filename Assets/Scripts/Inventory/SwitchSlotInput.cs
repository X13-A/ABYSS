using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSlotInput : MonoBehaviour
{
    private float mouseScrollWheel;
    private int count;
 
    void Update()
    {
        this.mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (this.mouseScrollWheel != 0) 
        {
            if (this.mouseScrollWheel < 0) this.count = 1;
            if (this.mouseScrollWheel > 0) this.count = -1;
            int slot = (InventoryManager.Instance.ActiveSlot + this.count) % 9;
            if (slot < 0) slot += 9;
            EventManager.Instance.Raise(new SwitchSlot { slot = slot });
        }
    }
}
