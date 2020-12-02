using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LootUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI lootLabel;

    [SerializeField]
    private MissionManagerVar missionManagerVar;

    // Start is called before the first frame update
    void Start()
    {
        if(missionManagerVar.Value == null)
        {
            missionManagerVar.OnChange += OnManagerChange;
        }
        else
        {
            UpdateLabel();
        }
    }

    private void OnDestroy() 
    {
         missionManagerVar.OnChange -= OnManagerChange;
    }

    private void OnManagerChange(MissionManager oldVal, MissionManager newVal)
    {
        UpdateLabel();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if(missionManagerVar.Value == null)
        {
            return;
        }
        int stolen = missionManagerVar.Value.ObjectsStolen;
        int toSteal = missionManagerVar.Value.ObjectsToSteal;
        if(stolen < toSteal)
        {
            lootLabel.text = $"{stolen}/{toSteal} Artifacts";
        }
        else
        {
            lootLabel.text = "Escape!";
        }
        
    }
}
