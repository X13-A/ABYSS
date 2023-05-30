using System;
using UnityEngine;
using UnityEngine.UI;
public enum ItemId
{
    Sword,
    Pickaxe,
    Wand,
    Map
};

[Serializable]
public class ItemEntry
{
    public string name;
    public Image icon;
    public GameObject heldPrefab;
    public GameObject droppedPrefab;
}
