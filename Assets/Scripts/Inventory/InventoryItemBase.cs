using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemBase : MonoBehaviour, IInventoryItem
{
    [SerializeField] private string _Name;
    public string Name
    {
        get { return _Name; }
    }

    [SerializeField] private Sprite _Image;
    public Sprite Image
    {
        get { return _Image; }
    }

    [SerializeField] private PlayerMode playerModeObject;
    public PlayerMode PlayerModeObject
    {
        get { return playerModeObject; }
    }

    public virtual void OnUse()
    {

    }

    public virtual void OnPickup()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnDrop()
    {
        RaycastHit hit = AimUtil.Instance.Aim(~(1 << LayerMask.NameToLayer("Aim")));

        if (hit.collider != null)
        {
            gameObject.SetActive(true);
            gameObject.transform.position = hit.point;
        }
    }
}
