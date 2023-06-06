using System;
using UnityEngine;
using UnityEngine.UI;
public enum ItemId
{
    Sword,
    Pickaxe,
    Wand,
    Map,
    Mushroom,
    DirtBlock,
    DarkDirtBlock,
    lavaBlockVariant1,
    lavaBlockVariant2,
    RockBlock,
    CanopyBlock,
    CoalBlock,
    GroundBlock,
    GroundGrassBlock,
    IceBlock,
    SandBlock,
    SandstoneBlock,
    SnowBlock,
    BottleHealth,
    AttackBoost,
    ShieldBoost,
};

public enum ItemRarity
{
    Common,
    Rare,
    Legendary,
}

[Serializable]
public class ItemEntry
{
    public string name;
    public Sprite icon;
    public GameObject heldPrefab;
    public GameObject droppedPrefab;
    public ItemRarity rarity;
    public bool isConsumable;
}
