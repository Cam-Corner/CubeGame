using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class TurnBasedSystem : SingletonScriptableObject<TurnBasedSystem>
{
    public BoolVar IsTimeActiveVar;
    public bool IsTimeActive => IsTimeActiveVar.Value;
    public BoolVar IsTurnBasedGameVar;
    public bool IsTurnBasedGame => IsTurnBasedGameVar.Value;
    public TransformArrVar runningPhysicsEntities;
    public bool WaitForPhysicsEntities;

    public bool IsWaitingForPhysicsEntities => WaitForPhysicsEntities && runningPhysicsEntities.Value.Length > 0;

    public delegate void OnWaitForPhysicsEntitiesChanged (bool isWaiting);

    public event OnWaitForPhysicsEntitiesChanged OnWaitForPhysicsEntitiesChangedEvent;

    public float PhysicsStopMagnitude = 2.0f;

    private void OnEnable() 
    {
        IsTimeActiveVar.Value = false;
        runningPhysicsEntities.Clear();

        if(WaitForPhysicsEntities)
        {
            runningPhysicsEntities.OnChange += OnChangedAwaitingEntities;
        }
    }

    private void OnDisable() 
    {
        runningPhysicsEntities.Clear();
        runningPhysicsEntities.OnChange -= OnChangedAwaitingEntities;
    }

    private void OnChangedAwaitingEntities(Transform[] oldEntities, Transform[] newEntities)
    {
        int oldCount = oldEntities.Length;
        int newCount = newEntities.Length;

        if(!WaitForPhysicsEntities
           || (oldCount <= 0 && newCount == 0)
           || (oldCount > 0 && newCount > 0))
        {
            return;
        }

        bool isWaiting = newCount > 0;
        OnWaitForPhysicsEntitiesChangedEvent?.Invoke(isWaiting);

        Debug.Log("IsWaiting for physics entities: " + isWaiting);
    }
}
