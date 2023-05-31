using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(ItemBank))]
public class ItemBankEditor : Editor
{
    private int selectedIndex;
    private List<ItemId> availableItems;

    private void OnEnable()
    {
        UpdateAvailableItems();
    }

    private void UpdateAvailableItems()
    {
        ItemBank itemBank = (ItemBank) target;
        availableItems = new List<ItemId>((ItemId[]) System.Enum.GetValues(typeof(ItemId)));
        availableItems.RemoveAll(item => itemBank.ItemKeys.Contains(item));
        Debug.Log(availableItems.Count);
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        ItemBank bank = (ItemBank) target;

        for (int i = bank.ItemKeys.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginVertical("helpbox");
            EditorGUILayout.LabelField(bank.ItemKeys[i].ToString());

            // Render ItemEntry
            ItemEntry entry = bank.ItemValues[i];
            EditorGUILayout.Space();
            entry.name = EditorGUILayout.TextField("Name", entry.name);
            EditorGUILayout.Space();
            entry.icon = (Sprite) EditorGUILayout.ObjectField("Icon", entry.icon, typeof(Sprite), true);
            EditorGUILayout.Space();
            entry.heldPrefab = (GameObject) EditorGUILayout.ObjectField("Held Prefab", entry.heldPrefab, typeof(GameObject), true);
            EditorGUILayout.Space();
            entry.droppedPrefab = (GameObject) EditorGUILayout.ObjectField("Dropped Prefab", entry.droppedPrefab, typeof(GameObject), true);
            EditorGUILayout.Space();
            entry.isConsumable = EditorGUILayout.Toggle("Is Consumable", entry.isConsumable);
            EditorGUILayout.Space();
            bank.ItemValues[i] = entry;

            if (GUILayout.Button("Remove Item"))
            {
                bank.Inspector_RemoveItem(bank.ItemKeys[i]);
                UpdateAvailableItems();
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndVertical();
        }

        if (availableItems.Count > 0)
        {
            EditorGUILayout.BeginHorizontal("helpbox");
            selectedIndex = EditorGUILayout.Popup("New Item Type", selectedIndex, availableItems.Select(x => x.ToString()).ToArray());
            ItemId selectedId = availableItems[selectedIndex];

            if (GUILayout.Button("Add Item"))
            {
                if (!bank.ItemKeys.Contains(selectedId))
                {
                    Undo.RecordObject(bank, "Add item");
                    bank.Inspector_AddItem(selectedId, null);
                    UpdateAvailableItems();
                }
                else
                {
                    Debug.LogWarning($"An item with the ID {selectedId} already exists in the bank!");
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        // Apply changes
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(bank);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
