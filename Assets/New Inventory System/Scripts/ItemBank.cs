using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using SDD.Events;

public class ItemBank : MonoBehaviour
{
    #region Custom Inspector
    [SerializeField] private List<ItemId> itemKeys = new List<ItemId>();
    [SerializeField] private List<ItemEntry> itemValues = new List<ItemEntry>();

    [SerializeField] public GameObject Inspector_CommonParticles;
    [SerializeField] public GameObject Inspector_RareParticles;
    [SerializeField] public GameObject Inspector_LegendaryParticles;

    private bool loaded = false;

    public List<ItemId> ItemKeys => itemKeys;
    public List<ItemEntry> ItemValues => itemValues;

    public void Inspector_AddItem(ItemId id, ItemEntry entry)
    {
        if (!itemKeys.Contains(id))
        {
            itemKeys.Add(id);
            itemValues.Add(entry);
        }
    }

    public void Inspector_RemoveItem(ItemId id)
    {
        if (itemKeys.Contains(id))
        {
            int index = itemKeys.IndexOf(id);
            itemKeys.RemoveAt(index);
            itemValues.RemoveAt(index);
        }
    }

    private void Inspector_LoadValues()
    {
        for (int i = 0; i < itemKeys.Count; i++)
        {
            items[itemKeys[i]] = itemValues[i];
        }
        commonParticles = Inspector_CommonParticles;
        rareParticles = Inspector_RareParticles;
        legendaryParticles = Inspector_LegendaryParticles;
    }
    #endregion

    private static ItemBank m_Instance;
    public static ItemBank Instance => m_Instance;

    private Dictionary<ItemId, ItemEntry> items = new Dictionary<ItemId, ItemEntry>();

    private static GameObject commonParticles;
    private static GameObject rareParticles;
    private static GameObject legendaryParticles;

    private void Awake()
    {
        if (!m_Instance)
        {
            m_Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (!loaded)
        {
            Inspector_LoadValues();
        }
    }

    public static string GetName(ItemId id)
    {
        return Instance.items[id].name;
    }

    public static Sprite GetIcon(ItemId id)
    {
        return Instance.items[id].icon;
    }

    public static GameObject GetHeldPrefab(ItemId id)
    {
        return Instance.items[id].heldPrefab;
    }

    public static GameObject GetDroppedPrefab(ItemId id)
    {
        return Instance.items[id].droppedPrefab;
    }

    public static GameObject GetDroppedItem(ItemId id)
    {
        GameObject prefab = GetDroppedPrefab(id);
        DroppedItem droppedItem = prefab.AddComponent<DroppedItem>();
        droppedItem.Init(id);
        return prefab;
    }

    public static bool IsConsumable(ItemId id)
    {
        return Instance.items[id].isConsumable;
    }

    public static ItemRarity GetRarity(ItemId id)
    {
        return Instance.items[id].rarity;
    }

    public static GameObject GetDroppedParticles(ItemId id)
    {
        ItemRarity rarity = GetRarity(id);
        switch (rarity)
        {
            case ItemRarity.Common: return commonParticles;
            case ItemRarity.Rare: return rareParticles;
            case ItemRarity.Legendary: return legendaryParticles;
            default: return commonParticles;
        }
    }

    public static void UseItem(ItemId id)
    {
        if (GameManager.Instance.State != GAMESTATE.PLAY) return;

        switch (id)
        {
            case ItemId.Map:
            {
                Debug.Log("Used Map");
                break;
            }
            case ItemId.Pickaxe:
            {
                EventManager.Instance.Raise(new PlayerAttackEvent
                {
                    type = AttackType.PICKAXE,
                    damage = 10f,
                    cooldown = 0.5f,
                    animationDuration = 0.5f,
                });
                break;
            }
            case ItemId.Sword:
            {
                EventManager.Instance.Raise(new PlayerAttackEvent
                {
                    type = AttackType.MELEE,
                    damage = 10f,
                    damageStartPercentage = 0.4f,
                    cooldown = 0.5f,
                    animationDuration = 0.5f,
                    hitDuration = 0.2f,
                });
                break;
            }
            case ItemId.Wand:
            {
                EventManager.Instance.Raise(new PlayerAttackEvent
                {
                    type = AttackType.MAGIC,
                    damage = 20f,
                    damageStartPercentage = 0.5f,
                    cooldown = 1f,
                    animationDuration = 1f,
                    hitDuration = -1f,
                    projectileSpeed = 50,
                });
                break;
            }
            case ItemId.Heart:
            {
                float health = PlayerManager.Instance.Health + 20;
                EventManager.Instance.Raise(new HealthPlayerEvent { health = health });
                break;
            }
            case ItemId.Mushroom:
            {
                float health = PlayerManager.Instance.Health - 20;
                EventManager.Instance.Raise(new HealthPlayerEvent { health = health });
                break;
            }
        }
    }

    public static void OnHoldItem(ItemId id)
    {
        switch (id)
        {
            case ItemId.Map:
            {
                EventManager.Instance.Raise(new ToggleMapEvent { value = true });
                return;
            }
        }
    }

    public static void OnStopHoldingItem(ItemId id)
    {
        switch (id)
        {
            case ItemId.Map:
            {
                EventManager.Instance.Raise(new ToggleMapEvent { value = false });
                return;
            }
        }
    }

    // TODO: Should be deleted as playerMode gets replaced
    public static PlayerMode? PlayerModeFromItem(ItemId? item)
    {
        if (item == null) return null;
        switch (item)
        {
            case ItemId.Sword: return PlayerMode.MELEE;
            case ItemId.Wand: return PlayerMode.MAGIC;
            case ItemId.Pickaxe: return PlayerMode.PICKAXE;
            default: return null;
        }
    }
}



