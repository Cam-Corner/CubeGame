using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StealingUI : MonoBehaviour
{
    [SerializeField]
    private MissionObjectiveVar objectBeingStolen;
    
    [SerializeField]
    private Transform stealingUI;
    
    [SerializeField]
    private Transform barTransform;

    [SerializeField]
    private TextMeshPro textLabel;

    [SerializeField]
    private string[] stealingPuns;

    private void Awake() 
    {   
        stealingUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(stealingUI.gameObject.activeInHierarchy && (objectBeingStolen.Value == null || !objectBeingStolen.Value.IsBeingStolen))
        {
            stealingUI.gameObject.SetActive(false);
        }
        
        if(objectBeingStolen.Value != null && objectBeingStolen.Value.IsBeingStolen)
        {
            if(!stealingUI.gameObject.activeInHierarchy)
            {
                stealingUI.gameObject.SetActive(true);
                string pun = stealingPuns[Random.Range(0, stealingPuns.Length)];
                string[] punSections = pun.Split('|');
                pun = "";
                foreach(string section in punSections)
                {
                    pun += section + "\n";
                }
                textLabel.text = pun;
            }
            
            Vector3 scale = barTransform.localScale;
            scale.x = objectBeingStolen.Value.RatioToSteal;
            barTransform.localScale = scale;
        }
    }
}
