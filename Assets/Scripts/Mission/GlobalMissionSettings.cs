using System.Collections;
using System.Collections.Generic;
using Google.GData.Spreadsheets;
using UnityEngine;
using UnityEngine.SceneManagement;
using AmoaebaUtils;

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
    [SerializeField] private BoolVar m_TimeActive;
    private List<Vector3> m_BrokenCollectablePositions = new List<Vector3>();
    //[SerializeField] private FloatVar ;
    [SerializeField] private BoolVar m_IsLootMode;
    private uint m_AISuspicionLevel = 0;
    private eMissionState m_CurrentMissionState;
    [SerializeField] private eMissionType m_MissionType = eMissionType.EMT_Loot;
    private float m_MissionTimer = 80;
    private float m_StartingAmountOfObjectives = 0;
    private float m_AmountOfMissionObjectivesLeft = 0;
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
        if (UnityEngineUtils.IsInPlayModeOrAboutToPlay())
            return;

        GameStart(m_MissionType);

        if (m_MissionType == eMissionType.EMT_Loot)
            m_IsLootMode.Value = true;
        else
            m_IsLootMode.Value = false;
    }

    public void GameStart(eMissionType MissionType)
    {
        m_MissionType = MissionType;
        m_AISuspicionLevel = 0;

        if (MissionType == eMissionType.EMT_Destructable)
            m_MissionTimer = ConvertTimeDetailsToFloat(m_DestructionGameStartTime);
        else
        {
            m_MissionTimer = 0;          
        }

        if(m_BrokenCollectablePositions.Count > 0)
            m_BrokenCollectablePositions.Clear();
    }

    public List<Vector3> GetBrokenLocations() => m_BrokenCollectablePositions;

    public void AddBrokenPosition(Vector3 Value) => m_BrokenCollectablePositions.Add(Value);

    public void StartingAmountOfMissionObjectives(uint Value)
    {
        m_StartingAmountOfObjectives = Value;
        m_AmountOfMissionObjectivesLeft = Value;
    }

    public void CollectedAMissionObjective()
    {
        if(m_AmountOfMissionObjectivesLeft > 0)
            m_AmountOfMissionObjectivesLeft -= 1;
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
        if (m_CurrentMissionState == eMissionState.EMS_PlayingMission)
        {
            if (m_MissionType == eMissionType.EMT_Destructable)
            {
                if(m_TimeActive.Value)
                    m_MissionTimer -= Time.deltaTime;

                float Score = 1;//m_DestructionScore.Value;
                Score = Score / 50.0f;

                if ((uint)Score >= 75)
                    m_AISuspicionLevel = 3;
                else if ((uint)Score >= 50)
                    m_AISuspicionLevel = 2;
                else
                    m_AISuspicionLevel = 1;

                Debug.Log("AI Suspicion Level = " + m_AISuspicionLevel);

                if (m_MissionTimer <= 0)
                {
                    SceneManager.LoadScene(m_DestructableGameOverSceneName);
                }
            }
            else
            {
                //work out % left
                uint PercentOfCollectablesTaken = 100;
                PercentOfCollectablesTaken -= (uint)((100 / m_StartingAmountOfObjectives) * m_AmountOfMissionObjectivesLeft);
                
                if (PercentOfCollectablesTaken >= 66)
                    m_AISuspicionLevel = 3;
                else if (PercentOfCollectablesTaken >= 33)
                    m_AISuspicionLevel = 2;
                else
                    m_AISuspicionLevel = 1;

                Debug.Log("AI Suspicion Level = " + m_AISuspicionLevel);

                if (m_TimeActive.Value)
                    m_MissionTimer += Time.deltaTime;
            }

            sTimeDetails MissionTimer = ConvertSecondsToTimeDetails(m_MissionTimer);

            //Debug.Log("Timer: " + MissionTimer.TimerInMinutesAndSecondsString);
        }
    }
}