using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemBase : MonoBehaviour, IInventoryItem
{
    public virtual string Name
    {
        get
        {
            return "_base item_";
        }
    }

    public Sprite _Image;
    public Sprite Image
    {
        get { return _Image; }
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
