using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionController : MonoBehaviour
{
    public delegate void OnComboDelegate();
    public OnComboDelegate OnComboFinishedEvent;
    public OnComboDelegate OnComboStartedEvent;

    [SerializeField]
    private FloatVar destructionScore;

    [SerializeField]
    private FloatVar comboCounter;

    [SerializeField]
    private FloatVar comboCountdownVar;

    [SerializeField]
    private int maxCombo = int.MaxValue;
    
    [SerializeField,
    Tooltip("Timer in Seconds")]
    private float comboTimer;

    [SerializeField]
    private float comboStackMultiplier = 0.1f;

    [SerializeField]
    private BoolVar isTimeActive;

    [SerializeField]
    private BoolVar isTurnBasedGame;

    private float startComboScore = 1;
    private bool isUpdateCombo = false;

    private IEnumerator comboRoutine = null;

    private bool IsCountingDownCombo => (comboRoutine != null);
    

    private void Start()
    {
        destructionScore.OnChange += OnDestruction;
    }

    private void OnDestroy() 
    {
        destructionScore.OnChange -= OnDestruction;
    }

    private void OnDestruction(float oldVal, float newVal)
    {
        if (!gameObject.activeInHierarchy)
            return;

        if(isUpdateCombo)
        {
            isUpdateCombo = false;
            return;
        }

        if(newVal <= oldVal)
        {
            return;
        }

        if(IsCountingDownCombo)
        {
            comboCounter.Value = Mathf.Clamp(comboCounter.Value + 1, 0 , maxCombo);
            Debug.Log("Combo: " + comboCounter.Value);
        }
        else
        {
            Debug.Log("Started Combo");
            comboCounter.Value = 1;
            startComboScore = newVal;
            OnComboStartedEvent?.Invoke();
        }

        if(comboRoutine != null)
        {
            StopCoroutine(comboRoutine);
        }
        comboRoutine = ComboCountdown();

        StartCoroutine(comboRoutine);
    }

    private IEnumerator ComboCountdown()
    {
        float elapsed = 0;
        while(elapsed < comboTimer)
        {
            yield return new WaitForEndOfFrame();
            if(!isTurnBasedGame.Value || isTimeActive.Value)
            {
                elapsed += Time.deltaTime;
                comboCountdownVar.Value = 1.0f - Mathf.Clamp(elapsed, 0, comboTimer) / comboTimer;
            }
        }

        float comboDelta = destructionScore.Value - startComboScore;
        float extraScore = comboDelta * comboStackMultiplier * (comboCounter.Value-1);
        
        isUpdateCombo = true;
        destructionScore.Value += extraScore;

        Debug.Log("Combo Finished: " + comboCounter.Value + " Extra Score: " + extraScore);
        comboRoutine = null;

        OnComboFinishedEvent?.Invoke();
        comboCounter.Value = 1;
        comboCountdownVar.Value = 0;
    }
}
