using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// SCRIPT GENERE PAR CHAT GPT POUR NOUS AIDER A TROUVER LES REFERENCES MANQUANTES DANS LA SCENE
// Pour l'utiliser, aller dans l'onglet "Window -> Missing Reference Finder"

public class MissingReferencesFinder : EditorWindow
{
    [MenuItem("Window/Missing Reference Finder")]
    public static void ShowWindow()
    {
        GetWindow(typeof(MissingReferencesFinder));
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Find Missing References in scene"))
        {
            FindMissingReferencesInScene();
        }
    }

    private static void FindMissingReferencesInScene()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (var go in allObjects)
        {
            Component[] components = go.GetComponents<Component>();
            foreach (var c in components)
            {
                if (!c)
                {
                    Debug.LogError("Missing component in " + FullPath(go));
                }
                else
                {
                    SerializedObject so = new SerializedObject(c);
                    var sp = so.GetIterator();

                    while (sp.NextVisible(true))
                    {
                        if (sp.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            if (sp.objectReferenceValue == null
                                && sp.objectReferenceInstanceIDValue != 0)
                            {
                                ShowError(sp, go);
                            }
                        }
                    }
                }
            }
        }
    }

    private static void ShowError(SerializedProperty sp, GameObject go)
    {
        var extend = "Property <color=brown>" + sp.displayName + "</color> in object <color=brown>" + FullPath(go) + "</color> is missing its reference.";
        Debug.LogError(extend, go);
    }

    private static string FullPath(GameObject go)
    {
        return go.transform.parent == null ? go.name : FullPath(go.transform.parent.gameObject) + " -> " + go.name;
    }
}
