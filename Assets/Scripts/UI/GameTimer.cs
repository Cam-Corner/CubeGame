using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private GlobalMissionSettings m_MissionSettings;
    private Text m_TimerText;

    private void Start()
    {
        m_TimerText = GetComponent<Text>();
    }

    private void Update()
    {
        string TimeString = "";

        if(m_MissionSettings.GetMissionType() == eMissionType.EMT_Destructable)
        {
            TimeString = "Time Remaining " + m_MissionSettings.GetMissionTimer().TimerInMinutesAndSecondsString;
        }
        else
        {
            TimeString = "Time " + m_MissionSettings.GetMissionTimer().TimerInMinutesAndSecondsString;
        }

        m_TimerText.text = TimeString;
    }
}
