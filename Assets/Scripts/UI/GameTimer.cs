using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private GlobalMissionSettings m_MissionSettings;
    private TextMeshProUGUI m_TimerText;

    private void Start()
    {
        m_TimerText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        string TimeString = m_MissionSettings.GetMissionTimer().TimerInMinutesAndSecondsString;

        m_TimerText.text = TimeString;
    }
}
