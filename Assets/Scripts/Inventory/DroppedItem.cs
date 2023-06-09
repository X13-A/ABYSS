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
        // Maybe put some delay before allowing pickup
        StartCoroutine(CoroutineUtil.DelayAction(0f, () => pickable = true));
    }

    public bool Pickup()
    {
        if (!pickable) return false;
        if (!InventoryManager.Instance.HasSpaceForItem(id)) return false;

        EventManager.Instance.Raise(new ItemPickedUpEvent { itemId = id });
        Destroy(gameObject);
        return true;
    }
}
