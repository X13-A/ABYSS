using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lava : InventoryItemBase
{
    public override string Name
    {
        get
        {
            return "Lava";
        }
    }

    public override void OnUse()
    {
        base.OnUse();
    }
}
