using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionTimer : MonoBehaviour
{
    public int CurrentSeconds = 0;          // Current countup timer (should be left at 0)
    public bool EnableCountDown = false;   // Countdown instead of counting up?
    public int CountDownTimer = 60;          // Countdown timer in seconds
    private bool PauseTimer = false;         // If We Ever Need to Pause the Timer (cutscenes etc.)

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Debug.Log(CountDownTimer);
        UnityEngine.Debug.Log(CurrentSeconds);
    }
    void Awake()
    {
        StartCoroutine("MissionTimerCoro");
    }

    private IEnumerator MissionTimerCoro()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (PauseTimer == false && EnableCountDown == false)
            CurrentSeconds = CurrentSeconds + 1;
            UnityEngine.Debug.Log(CurrentSeconds);
            if (PauseTimer == false && EnableCountDown == true)
            CountDownTimer = CountDownTimer - 1;
            UnityEngine.Debug.Log(CountDownTimer);
        }

    }
}
