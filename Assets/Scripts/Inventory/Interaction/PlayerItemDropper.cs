using SDD.Events;
using UnityEngine;

public class PlayerItemDropper : MonoBehaviour
{
    [SerializeField] private GameObject dropPosition;

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        EventManager.Instance.AddListener<ItemDroppedEvent>(DropItem);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<ItemDroppedEvent>(DropItem);
    }

    private void DropItem(ItemDroppedEvent e)
    {
        GameObject droppedPrefab = ItemBank.GetDroppedPrefab(e.itemId);
        GameObject droppedGameObject = Instantiate(droppedPrefab, dropPosition.transform.position, droppedPrefab.transform.rotation);
        GameObject particles = ItemBank.GetDroppedParticles(e.itemId);

        GameObject particlesInstance = Instantiate(particles, droppedGameObject.transform.position, Quaternion.identity);
        particlesInstance.transform.SetParent(droppedGameObject.transform, false);
        particlesInstance.transform.localPosition = Vector3.zero;
        particlesInstance.transform.rotation = Quaternion.identity;

        // Just for safety, the script should be in the prefab
        if (droppedGameObject.GetComponent<DroppedItem>() == null)
        {
            droppedGameObject.AddComponent<DroppedItem>().Init(e.itemId);
        }
    }
}
