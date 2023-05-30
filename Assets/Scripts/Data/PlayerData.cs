using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData m_Instance;
    public static PlayerData Instance => m_Instance;

    private float maxHealth;
    public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }

    private float health;

    public float Health { get { return health; } set { health = value; } }

    private Dictionary<string, GameObject> mItems = new Dictionary<string, GameObject>();
    private Dictionary<string, int> mItemsCount = new Dictionary<string, int>();
    private Dictionary<string, int> mItemsSlot = new Dictionary<string, int>();
    public Dictionary<string, GameObject> Items { get { return mItems; } set { mItems = value; } }
    public Dictionary<string, int> ItemsCount { get { return mItemsCount; } set { mItemsCount = value; } }
    public Dictionary<string, int> ItemsSlot { get { return mItemsSlot; } set { mItemsSlot = value; } }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        m_Instance = this;
        gameObject.transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        //foreach (KeyValuePair<string, IInventoryItem> item in mItems)
        //{
        //    Debug.Log(item.Key);
        //}
        //Debug.Log("Next frame\n");
    }
}

