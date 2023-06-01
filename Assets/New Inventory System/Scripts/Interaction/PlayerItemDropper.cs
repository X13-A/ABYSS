using SDD.Events;
using UnityEngine;

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
        EventManager.Instance.AddListener<ItemDroppedEvent>(this.DropItem);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ItemDroppedEvent>(this.DropItem);
    }

    private void DropItem(ItemDroppedEvent e)
    {
        GameObject droppedPrefab = ItemBank.GetDroppedPrefab(e.itemId);
        GameObject droppedGameObject = Instantiate(droppedPrefab, dropPosition.transform.position, droppedPrefab.transform.rotation);
        droppedGameObject.AddComponent<DroppedItem>().Init(e.itemId);
    }
}
