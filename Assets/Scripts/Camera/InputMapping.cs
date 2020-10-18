using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMapping : ScriptableObject
{

    public enum ControllerType
    {
        Mouse,
        Playstation,
        XBox
    }

    [SerializeField]
    private ControllerType controllerType;

    public ControllerType m_ControllerType => controllerType;
    public string HorizontalCameraAxis => "Horizontal Camera " + JoystickString();

    public string VerticalCameraAxis => "Vertical Camera " + JoystickString();

    private string JoystickString()
    {
        switch(controllerType)
        {
            case ControllerType.Playstation:
                return "PS";
            case ControllerType.XBox:
                return "XBOX";
        }
        return "XBOX";
    } 

    public bool GetRunningButton()
    {
        switch(controllerType)
        {
            case ControllerType.Mouse:
                return Input.GetMouseButton(0);
            case ControllerType.XBox:
                return Input.GetKeyDown(KeyCode.JoystickButton0);
            case ControllerType.Playstation:
                return Input.GetKeyDown(KeyCode.JoystickButton1);
        }
        return false;
    }
}
