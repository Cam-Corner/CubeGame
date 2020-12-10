using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using cakeslice;

public class AddOutline : MonoBehaviour
{
    [SerializeField]
    private BoolVar IsLootGame;

    [SerializeField]
    private BoolVar showObjectiveOutlines;

    [SerializeField]
    private MissionManagerVar missionManager;
    private bool hasAdded = false;
    void Start()
    {
        if(!IsLootGame.Value)
        {
            Destroy(this);
        }

        showObjectiveOutlines.OnChange += OnShowChange;
    }

    private void OnShowChange(bool oldVal, bool newVal)
    {
        if(hasAdded && !newVal)
        {
            Outline[] outlines = GetComponentsInChildren<Outline>();
            foreach(Outline script in outlines)
            {
                Destroy(script);
            }
            hasAdded = false;
        }
    }

    private void OnDestroy() 
    {
        showObjectiveOutlines.OnChange -= OnShowChange;
    }

    private void Update() 
    {
        if(hasAdded || !showObjectiveOutlines.Value || missionManager.Value == null || missionManager.Value.IsInMissionBrief)
        {
            return;
        }

        if(!hasAdded)
        {
          MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
          foreach(MeshRenderer mesh in meshes)
          {
              AddOutlineScript(mesh.gameObject);
          }

          meshes = GetComponents<MeshRenderer>();
          foreach(MeshRenderer mesh in meshes)
          {
              AddOutlineScript(mesh.gameObject);
          }
          hasAdded = true;
        }
    }

    private void AddOutlineScript(GameObject gameObject)
    {
        gameObject.AddComponent<Outline>();
    }
}
