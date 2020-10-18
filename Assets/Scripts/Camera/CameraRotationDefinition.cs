using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CameraRotationDefinition
{
    public KeyCode key;
    public Vector3 rotationEuler;
    public bool isAbsolute;
}
