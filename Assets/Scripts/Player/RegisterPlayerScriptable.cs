using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterPlayerScriptable : MonoBehaviour
{
    [SerializeField]
    private PlayerScriptable playerScriptable;

    void Start()
    {
        if(playerScriptable != null)
        {
            playerScriptable.Player = transform;
        }
        Destroy(this);
    }
}
