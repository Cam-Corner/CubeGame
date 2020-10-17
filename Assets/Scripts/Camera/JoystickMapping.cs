using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickMapping : ScriptableObject
{

    public enum ControllerType
    {
        Playstation,
        XBox
    }

    [SerializeField]
    private ControllerType controllerType;

    public string HorizontalCameraAxis => "Horizontal Camera " + ControllerString();

    public string VerticalCameraAxis => "Vertical Camera " + ControllerString();

    private string ControllerString()
    {
        switch(controllerType)
        {
            case ControllerType.Playstation:
                return "PS";
            case ControllerType.XBox:
                return "XBOX";
        }
        return "XBox";
    } 
}
