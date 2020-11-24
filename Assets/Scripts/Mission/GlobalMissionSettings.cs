using System.Collections;
using System.Collections.Generic;
using Google.GData.Spreadsheets;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum eMissionState
{
    EMS_MissionBrief = 0,
    EMS_PlayingMission = 1,
}

public enum eMissionType
{
    EMT_Loot = 0,
    EMT_Destructable,
}

[System.Serializable]
public struct sTimeDetails
{
    public float Minutes;
    public float Seconds;
    public float TotalTimerInSeconds;
    public string TimerInMinutesAndSecondsString;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Mission", order = 1)]
public class GlobalMissionSettings : ScriptableObject
{
    //=========================
    [Header("Camera Pan Settings")]
    [SerializeField] private float m_CameraPanMoveSpeed = 20;
    [SerializeField] private float m_CameraPanRotationSpeed = 20;
    [SerializeField] private string m_DestructableGameOverSceneName = "Name";
    [SerializeField] private sTimeDetails m_DestructionGameStartTime;
    private uint m_AISuspicionLevel = 0;
    private eMissionState m_CurrentMissionState;
    private eMissionType m_MissionType = eMissionType.EMT_Loot;
    private float m_MissionTimer = 80;
    //=========================

    public float GetCameraPanMoveSpeed() => m_CameraPanMoveSpeed;
    public float GetCameraPanRotationSpeed() => m_CameraPanRotationSpeed;
    public eMissionState GetMissionState() => m_CurrentMissionState;
    public void SetMissionState(eMissionState MissionState) => m_CurrentMissionState = MissionState;

    public void SetMissionType(eMissionType MissionType) => m_MissionType = MissionType;
    public eMissionType GetMissionType() => m_MissionType;

    //public eTimeDetails GetStartLootGameTimer() => m_LootGameStartTime;

    private void OnEnable()
    {
        GameStart(m_MissionType);
    }

    public void GameStart(eMissionType MissionType)
    {
        m_MissionType = MissionType;
        m_AISuspicionLevel = 0;

        if (MissionType == eMissionType.EMT_Destructable)
            m_MissionTimer = ConvertTimeDetailsToFloat(m_DestructionGameStartTime);
        else
            m_MissionTimer = 0;
    }

    public void IncreaseAISuspicionLevel(uint Amount)
    {
        m_AISuspicionLevel += Amount;
    }

    public float GetAISuspicionLevel() => m_AISuspicionLevel;

    private sTimeDetails ConvertSecondsToTimeDetails(float Time)
    {
        sTimeDetails FinalTimeDetails = new sTimeDetails();

        int Minutes = (int)(Time / 60.0f);
        int Seconds = (int)(Time % 60.0f);

        FinalTimeDetails.Minutes = Minutes;
        FinalTimeDetails.Seconds = Seconds;
        FinalTimeDetails.TotalTimerInSeconds = Time;
        
        if(Seconds <= 9)
        {
            FinalTimeDetails.TimerInMinutesAndSecondsString = Minutes.ToString() + ":0" + Seconds.ToString();
        }
        else
        {
            FinalTimeDetails.TimerInMinutesAndSecondsString = Minutes.ToString() + ":" + Seconds.ToString();
        }

        return FinalTimeDetails;
    }

    private float ConvertTimeDetailsToFloat(sTimeDetails TimeDetails)
    {
        float TimeInSeconds = 0;
        TimeInSeconds += TimeDetails.Seconds;
        TimeInSeconds += (60 * TimeDetails.Minutes);
        return TimeInSeconds;
    }

    public sTimeDetails GetMissionTimer()
    {
        return ConvertSecondsToTimeDetails(m_MissionTimer);
    }

    /* This only needs to be called once per frame **/
    public void UpdateMissionTimer()
    {
        if (m_MissionType == eMissionType.EMT_Destructable)
        {
            m_MissionTimer -= Time.deltaTime;

            if(m_MissionTimer <= 0)
            {
                SceneManager.LoadScene(m_DestructableGameOverSceneName);
                
            }
        }
        else
        {
            m_MissionTimer += Time.deltaTime;
        }

        sTimeDetails MissionTimer = ConvertSecondsToTimeDetails(m_MissionTimer);

        //Debug.Log("Timer: " + MissionTimer.TimerInMinutesAndSecondsString);
    }
}