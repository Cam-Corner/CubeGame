using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptable : ScriptableObject
{
    public delegate void OnPlayerChange(Transform player);
    public OnPlayerChange OnPlayerChangeEvent;

    private Transform player;
    private Rigidbody body;


    public Transform Player 
    {
        get { return player; }
        set 
        { 
            player = value; 
            
            if(player != null)
            {
                body = value.GetComponent<Rigidbody>();
            }

            OnPlayerChangeEvent?.Invoke(player); 
        }
    }

    public Rigidbody Body => body;
}
