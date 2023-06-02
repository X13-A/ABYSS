using System;
using UnityEngine;
using UnityEngine.UI;
public enum ItemId
{
    Sword,
    Pickaxe,
    Wand,
    Map,
    Heart,
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
    SnowBlock
};

[Serializable]
public class ItemEntry
{
    public string name;
    public Sprite icon;
    public GameObject heldPrefab;
    public GameObject droppedPrefab;
    public bool isConsumable;
}
