using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

[RequireComponent(typeof(Rigidbody))]
public class TurnBasedPhysicsEntity : MonoBehaviour
{
    private Vector3 savedVelocity = Vector3.zero;
    private Vector3 savedAngularVelocity = Vector3.zero;

    private TransformArrVar physicsEntities => TurnBasedSystem.Instance.runningPhysicsEntities;
    private BoolVar turnBasedGame => TurnBasedSystem.Instance.IsTurnBasedGameVar;

    private Rigidbody body;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        turnBasedGame.OnChange += OnTurnBaseChange;
        TurnBasedSystem.Instance.IsTimeActiveVar.OnChange += OnTimeActiveChange;
    }

    private void OnDestroy() 
    {
        turnBasedGame.OnChange -=  OnTurnBaseChange;
        TurnBasedSystem.Instance.IsTimeActiveVar.OnChange -= OnTimeActiveChange;
    }

    private void OnTurnBaseChange(bool oldVal, bool newVal)
    {
        if(oldVal == newVal)
        {
            return;
        }

        if(newVal)
        {
            SetNeedsResolution(false);
            ResumePhysics();
        }
        else
        {
            CheckResolution();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!turnBasedGame.Value)
        {
            return;
        }
        CheckResolution();
    }

    private void CheckResolution()
    {
        bool hasVelocity = body.velocity.magnitude > TurnBasedSystem.Instance.PhysicsStopMagnitude
                           || body.angularVelocity.magnitude > TurnBasedSystem.Instance.PhysicsStopMagnitude;
        SetNeedsResolution(hasVelocity);
    }
    
    private void OnTimeActiveChange(bool oldVal, bool isTimeActive)
    {
        if(oldVal == isTimeActive)
        {
            return;
        }

        if(isTimeActive)
        {
            ResumePhysics();
        }
        else
        {
            PausePhysics();
        }
    }

    private void PausePhysics() 
    {
        savedVelocity = body.velocity;
        savedAngularVelocity = body.angularVelocity;
        body.isKinematic = true;
    }
 
     private void ResumePhysics() 
     {
         body.isKinematic = false;
         body.AddForce( savedVelocity, ForceMode.VelocityChange );
         body.AddTorque( savedAngularVelocity, ForceMode.VelocityChange );
     }

     private void SetNeedsResolution(bool needsResolution)
     {
        if(!TurnBasedSystem.Instance.WaitForPhysicsEntities)
        {
            physicsEntities.Clear();
            return;
        }

        if(needsResolution && !physicsEntities.Contains(transform))
        {
            physicsEntities.Add(transform);
        }
        else
        {
            physicsEntities.Remove(transform);
        }
     }
}
