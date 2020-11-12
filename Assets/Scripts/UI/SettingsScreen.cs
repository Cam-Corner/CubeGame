using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
    [SerializeField]
    private Dropdown controllerTypeDropdown;

    [SerializeField]
    private InputMapping mapping;

    private void Start() 
    {
        if(MenuHelper.Instance.openSettings != null)
        {
            Destroy(gameObject);
        }
        else
        {
            MenuHelper.Instance.openSettings = transform;
        }

        controllerTypeDropdown.ClearOptions();
        
        int index = 0;
        int indexToSelect = 0;

        Array types = Enum.GetValues(typeof(InputMapping.ControllerType));
        foreach(InputMapping.ControllerType type in types)
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = InputMapping.TypeName(type);
            controllerTypeDropdown.options.Add(data);
            if(type == mapping.m_ControllerType)
            {
                indexToSelect = index;
            }
            index++;
        }
        controllerTypeDropdown.value = indexToSelect;
        controllerTypeDropdown.captionText.text =InputMapping.TypeName((InputMapping.ControllerType)types.GetValue(indexToSelect));
    }
    
    public void OnControllerTypeChanged()
    {
        Array types = Enum.GetValues(typeof(InputMapping.ControllerType));
        InputMapping.ControllerType type = (InputMapping.ControllerType)types.GetValue(controllerTypeDropdown.value);
        mapping.m_ControllerType = type;
    }

    public void CloseMenu() 
    {
        Destroy(this.gameObject);
    }

    private void OnDestroy() 
    {
        if(MenuHelper.Instance.openSettings == transform)
        {
            MenuHelper.Instance.openSettings = null;
        }    
    }
}
