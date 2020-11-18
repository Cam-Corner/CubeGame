using System.Collections;
using System.Collections.Generic;
using Google.GData.Spreadsheets;
using UnityEngine;

public enum eMissionState
{
    EMS_MissionBrief = 0,
    EMS_PlayingMission = 1,
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Mission", order = 1)]
public class GlobalMissionSettings : ScriptableObject
{
    //=========================
    [Header("Camera Pan Settings")]
    [SerializeField] private float m_CameraPanMoveSpeed = 20;
    [SerializeField] private float m_CameraPanRotationSpeed = 20;
    private eMissionState m_CurrentMissionState;
    //=========================

    public float GetCameraPanMoveSpeed() => m_CameraPanMoveSpeed;
    public float GetCameraPanRotationSpeed() => m_CameraPanRotationSpeed;
    public eMissionState GetMissionState() => m_CurrentMissionState;
    public void SetMissionState(eMissionState MissionState) => m_CurrentMissionState = MissionState;

}