using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EnemySettings", order = 1)]
public class EnemySettings : ScriptableObject
{
    //=========================
    [SerializeField] public uint m_FieldOfView = 45;
    [SerializeField] public uint m_FOVDistance = 45;
    [SerializeField] public uint m_AmountOfRays = 50;
    //=========================
}
