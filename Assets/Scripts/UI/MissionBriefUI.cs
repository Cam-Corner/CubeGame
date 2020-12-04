using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionBriefUI : MonoBehaviour
{
   [SerializeField]
   private GameObject[] tutorialObjects;

   [SerializeField]
   private GameObject[] nonTutorialObjects;

   [SerializeField]
   private MissionManagerVar managerVar;

   private bool isInTutorial = true;
   private void Start() 
   {
       SwapVisibility(isInTutorial);
   }

   private void Update() 
   {
        if(!isInTutorial)
        {
            return;
        }

        if(!managerVar.Value.IsInMissionBrief)
        {
            SwapVisibility(false);
            Destroy(this);
        }
   }


   private void SwapVisibility(bool inTutorial)
   {
        foreach(GameObject obj in nonTutorialObjects)
        {
            obj.SetActive(!inTutorial);
        }

        foreach(GameObject obj in tutorialObjects)
        {
            obj.SetActive(inTutorial);
        }     
        isInTutorial = inTutorial;
   }
}
