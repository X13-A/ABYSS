using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandleObject : MonoBehaviour
{
    [SerializeField] private GameObject hand;

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
        EventManager.Instance.AddListener<UpdateObjectInhand>(setObject);
    }

    public void UnsubscribeEvents()
    {
        EventManager.Instance.RemoveListener<UpdateObjectInhand>(setObject);
    }

    public void setObject(UpdateObjectInhand e)
    {
        for (int i = 2; i < hand.transform.childCount; i++)
        {
            Destroy(hand.transform.GetChild(i).gameObject);
        }
        if (e.objectInSlot != null)
        {
            GameObject objectInHand = Instantiate(e.objectInSlot, hand.transform);
            objectInHand.SetActive(true);
            objectInHand.transform.localPosition = Vector3.zero;

            BlockDamage blockDamage = objectInHand.GetComponent<BlockDamage>();
            if (blockDamage != null)
            {
                objectInHand.transform.localRotation = Quaternion.identity;
                blockDamage.Health = 10;
            }
        }

    }
}
