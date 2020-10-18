using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceArrow : MonoBehaviour
{
    [SerializeField]
    private Transform FillArrow;

    [SerializeField]
    private float arrowDistance = 1.5f;

    private Vector3 startScale;

    private void Awake() 
    {
        startScale = FillArrow.localScale;   
    }
    
    public void UpdateArrow(Vector3 pos, Vector3 dir, float ratio)
    {
        dir = dir.normalized;
        
        FillArrow.localScale = Vector3.Scale(startScale, new Vector3(ratio, 1.0f, ratio));
        
        transform.position = pos;
        
        //rotate the arrow so it faces the forward direction
        //Y Is the up vector in unity so we will use the Y as the Z Force
        Quaternion DirectionRotation = Quaternion.LookRotation(dir, 
                                                               new Vector3(0, 1, 0));

        transform.rotation = DirectionRotation;
        transform.position += dir * arrowDistance;
    }
}
