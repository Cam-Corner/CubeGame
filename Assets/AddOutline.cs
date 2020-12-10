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
    private MissionManagerVar missionManager;
    private bool hasAdded = false;
    void Start()
    {
        if(!IsLootGame.Value)
        {
            Destroy(this);
        }
    }

    private void Update() 
    {
        if(missionManager.Value == null || missionManager.Value.IsInMissionBrief || hasAdded)
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
