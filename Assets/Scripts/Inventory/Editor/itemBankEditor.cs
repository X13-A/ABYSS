using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

[CustomEditor(typeof(ItemBank))]
public class ItemBankEditor : Editor
{
    private int selectedIndex;
    private List<ItemId> availableItems;
    private bool particlesFoldout = true;
    private bool itemsFoldout = true;

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

        this.particlesFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(this.particlesFoldout, "Dropped item particles");

        if (this.particlesFoldout)
        {
            bank.Inspector_CommonParticles = (GameObject) EditorGUILayout.ObjectField("Common Particles", bank.Inspector_CommonParticles, typeof(GameObject), true);
            bank.Inspector_RareParticles = (GameObject) EditorGUILayout.ObjectField("Rare Particles", bank.Inspector_RareParticles, typeof(GameObject), true);
            bank.Inspector_LegendaryParticles = (GameObject) EditorGUILayout.ObjectField("Legendary Particles", bank.Inspector_LegendaryParticles, typeof(GameObject), true);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        this.itemsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(this.itemsFoldout, "All items");
        if (this.itemsFoldout)
        {
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
                entry.positionedBlock = (GameObject) EditorGUILayout.ObjectField("Positioned Block Prefab", entry.positionedBlock, typeof(GameObject), true);
                EditorGUILayout.Space();
                entry.isConsumable = EditorGUILayout.Toggle("Is Consumable", entry.isConsumable);
                EditorGUILayout.Space();
                entry.rarity = (ItemRarity) EditorGUILayout.EnumPopup("Rarity", entry.rarity);
                EditorGUILayout.Space();
                entry.heldCursorType = (CursorType) EditorGUILayout.EnumPopup("Held Cursor Type", entry.heldCursorType);
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
        }


        // Apply changes
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(bank);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
