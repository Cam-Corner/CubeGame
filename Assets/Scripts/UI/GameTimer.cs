using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private GlobalMissionSettings m_MissionSettings;

    [SerializeField] private MissionManagerVar missionManager;
    private TextMeshProUGUI m_TimerText;

    private void Start()
    {
        m_TimerText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if(missionManager.Value.IsInMissionBrief)
        {
            m_TimerText.text = "";
            return;
        }

        string TimeString = m_MissionSettings.GetMissionTimer().TimerInMinutesAndSecondsString;

        m_TimerText.text = TimeString;
    }
}
