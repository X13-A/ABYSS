using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [SerializeField] private ItemId id;
    private bool pickable = false;

    public void Init(ItemId id)
    {
        this.id = id;
    }

    private void Start()
    {
        StartCoroutine(CoroutineUtil.DelayAction(2f, () => { pickable = true; }));
    }

    public bool Pickup()
    {
        if (!pickable) return false;
        if (!InventoryManager.Instance.HasSpaceForItem(this.id)) return false;

        EventManager.Instance.Raise(new ItemPickedUpEvent { itemId = this.id });
        Destroy(this.gameObject);
        return true;
    }
}
