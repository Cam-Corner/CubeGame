using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

public class CreateScriptableObject : ScriptableWizard
{
    [SerializeField]
    private MonoScript originalScript;

    [SerializeField, Range(1, 20)]
    private int instances = 1;

    [SerializeField]
    private string instanceName = "";

    [SerializeField]
    private bool createAnother = false;

    [MenuItem("Assets/Create/Scriptable Objects",false,(int)'a')]
    public static void ShowWizard()
    {
        ScriptableWizard.DisplayWizard<CreateScriptableObject>("Create Scriptable Objects", "Create");
    }

    private void OnWizardUpdate()
    {
        this.isValid = (originalScript != null) && originalScript.GetClass() != null && originalScript.GetClass().IsSubclassOf(typeof(ScriptableObject));
        originalScript = this.isValid ? originalScript : null;
    }

    private void OnWizardCreate()
    {
        for(int i = 0; i < instances; i++) {
            if (instanceName == null || instanceName == "")
            {
                ScriptableObjectUtility.CreateAsset(ScriptableObject.CreateInstance(originalScript.GetClass()));
            }
            else
            { 
                string name = instanceName + (i == 0 ? "" : " " + i);
                ScriptableObjectUtility.CreateAsset(ScriptableObject.CreateInstance(originalScript.GetClass()), name);
            }
        }

        if(createAnother)
        {
            CreateScriptableObject wizard = ScriptableWizard.DisplayWizard<CreateScriptableObject>("Create Scriptable Objects", "Create");
        }
    }
}
