using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


public class PositionCopyWizard : ScriptableWizard
{
    public Transform[] originals;
    public Transform copyPrefab;

    public bool destroyOriginal = true;

   [MenuItem("RobberDuck/ReplaceWizard")]
    static void CreateWizard()
    {
        PositionCopyWizard wizard = ScriptableWizard.DisplayWizard<PositionCopyWizard>("Replace Objects", "Replace");
    }

    private void OnWizardCreate()
    {
        for(int i = 0; i < originals.Length; i++)
        {
            Transform original = originals[i];
            if(original == null)
            {
                continue;
            }

            PrefabAssetType type =  PrefabUtility.GetPrefabAssetType(copyPrefab.gameObject);
            if(type != PrefabAssetType.MissingAsset && type != PrefabAssetType.NotAPrefab && type != PrefabAssetType.Model)
            {
                Transform instance = PrefabUtility.InstantiatePrefab(copyPrefab) as Transform;
                instance.parent = original.parent;
                instance.position = original.position;
                instance.rotation = original.rotation;
                instance.localScale = original.localScale;
                instance.name = original.name;
            }
            else
            {
                Transform instance = Instantiate(copyPrefab, original.position, original.rotation, original.parent);
                instance.name = original.name;
                instance.localScale = original.localScale;
            }

            if(destroyOriginal)
            {
                DestroyImmediate(original.gameObject);
            }
        }
    }

    public void OnWizardUpdate()
    {
        isValid = originals != null && originals.Length > 0 && copyPrefab != null;
    }
}
#endif