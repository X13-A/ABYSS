using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemBase : MonoBehaviour, IInventoryItem
{
    [SerializeField] private string _Name;
    public string Name => _Name;

    [SerializeField] private Sprite _Image;
    public Sprite Image => _Image;

    [SerializeField] private PlayerMode playerModeObject;
    public PlayerMode PlayerModeObject => playerModeObject;


    public virtual void OnUse() {}

    public virtual void OnPickup()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnDrop()
    {
        // TODO: replace to drop on the feet of the player
        RaycastHit hit = AimUtil.Instance.Aim(~(1 << LayerMask.NameToLayer("Aim")));

        if (hit.collider != null)
        {
            gameObject.SetActive(true);
            gameObject.transform.position = hit.point;
        }
    }
}
