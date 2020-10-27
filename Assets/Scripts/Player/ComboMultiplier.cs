using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;

public class ComboMultiplier : MonoBehaviour
{
    UnityEvent m_ForceResetEvent = new UnityEvent(); // Listen Event - Will Listen out for a call.
    UnityEvent m_CallStackEvent = new UnityEvent();// Listen Event - Will Listen out for a call.
    private int CurrentStacks = 0; // What is the current stack?
    public int SecondsBeforeTimeOut = 5; //Seconds before the stacks reset to 0
    private bool StackReset = false; // Has the stack been updated before the timer? if not, reset CurrentStacks
    private bool CoroutineRunning = false; //Is the Coroutine Running? - prevents multiple coroutines

    // Start is called before the first frame update
    void Start()
    {
        m_CallStackEvent.AddListener(CallStack); //Add Listener Event
        m_ForceResetEvent.AddListener(ForceReset); //Add Listener Event
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("q") && m_CallStackEvent != null) // TEMP (will replace with destruction BP)
        {
            //Begin the action
            m_CallStackEvent.Invoke();
        }

        // if (Input.GetKeyDown("e") && m_ForceResetEvent != null) //[Disabled, unless we need it]
        {
            //Begin the action
            //m_ForceResetEvent.Invoke(); //[Disabled, unless we need it]
        }
    }

    // Listener Event [Forces the Stack to reset to 0 and return bools to default]
    void ForceReset()
    {
        CurrentStacks = 0;
        CoroutineRunning = false;
        StackReset = false;
    }

    // Listener Event [Calls to add a stack and initiate the Coroutine]
    void CallStack()
    {
    CurrentStacks = CurrentStacks + 1;
    Debug.Log(CurrentStacks);
    StackReset = false;
        if (CoroutineRunning == false)
        {
            StartCoroutine("TimeOutCoro");
            CoroutineRunning = true;
        }
    }
    // Coroutine IEnumerator Event
    private IEnumerator TimeOutCoro()
    {
        while (true)
        {
            if (StackReset == true)
            {
                CurrentStacks = 0;
                Debug.Log("hello");
                StopCoroutine("TimeOutCoro");
                CoroutineRunning = false; 
            }
                StackReset = true;
                yield return new WaitForSeconds(SecondsBeforeTimeOut);
        }

    }
}