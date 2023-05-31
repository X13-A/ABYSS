using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerItemDropper : MonoBehaviour
{
    [SerializeField] private GameObject dropPosition;

    private void OnEnable()
    {
        this.SubscribeEvents();
    }

    private void OnDisable()
    {
        this.UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<ItemDroppedEvent2>(this.DropItem);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ItemDroppedEvent2>(this.DropItem);
    }

    private void DropItem(ItemDroppedEvent2 e)
    {
        GameObject droppedPrefab = ItemBank.GetDroppedPrefab(e.itemId);
        droppedPrefab.AddComponent<DroppedItem>().Init(e.itemId);
        Instantiate(droppedPrefab, dropPosition.transform.position, Quaternion.identity);
    }
}
