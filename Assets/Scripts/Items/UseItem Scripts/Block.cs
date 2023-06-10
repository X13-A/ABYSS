using SDD.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour, IUseItem
{
    [SerializeField] private ItemId id;
    public bool Use()
    {
        RaycastHit hit = AimUtil.Instance.Aim(~(1 << LayerMask.NameToLayer("Aim")));
        if (hit.collider)
        {
            Instantiate(ItemBank.GetPositionedBlock(id), hit.collider.transform.position + hit.normal, Quaternion.identity);
            return true;
        }
        return false;
    }
}
