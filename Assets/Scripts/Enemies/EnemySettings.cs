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
    [SerializeField] public float m_WalkSpeed = 2.8f;
    [SerializeField] public float m_RunSpeed = 7.0f;
    [SerializeField] private float m_SusLevel1MaxValue = 50;
    [SerializeField] private float m_SusLevel2MaxValue = 75;

    [SerializeField] private GlobalMissionSettings m_MissionSettings;
    [SerializeField] private float m_SlowDetectionTime = 5.0f;
    [SerializeField] private float m_AverageDetectionTime = 3.0f;
    [SerializeField] private float m_FastDetectionTime = 1.0f;
    //=========================

    public float GetCorrectDetectionTime()
    {
        if (m_MissionSettings != null)
        {
            float SusLevel = m_MissionSettings.GetAISuspicionLevel();

            if (SusLevel >= m_SusLevel2MaxValue)
                return m_FastDetectionTime;
            else if (SusLevel >= m_SusLevel1MaxValue)
                return m_AverageDetectionTime;
        }
        else
        {
            Debug.Log("EnemySettings Error: Please assign Mission Settings!");
        }

        return m_SlowDetectionTime;
    }

    public float GetSuspicionLevel()
    {
        if (m_MissionSettings != null)
        {
            return m_MissionSettings.GetAISuspicionLevel();
        }

        return 0;
    }
}
