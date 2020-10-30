using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [SerializeField]
    private Transform laserTransform;

    [SerializeField]
    private float maxLaserDistance = 300;

    [SerializeField]
    private LayerMask laserCalculateMask;
    
    [SerializeField]
    float extraLength = 0;

    [SerializeField]
    private PlayerScriptable playerScriptable;

    private Vector3 laserStartScale = Vector3.zero;
    void Start()
    {
        SetupStartScale();
    }

    private void SetupStartScale()
    {
        laserStartScale = new Vector3(laserTransform.localScale.x,
                                0.5f,
                                laserTransform.localScale.z);
    }

    private void RecalculateLaser()
    {
        float distance = maxLaserDistance;
        if(Physics.Raycast(transform.position, 
                            transform.forward, 
                            out RaycastHit hit,
                            maxLaserDistance,
                            laserCalculateMask.value,
                            QueryTriggerInteraction.Ignore))
        {
            distance = hit.distance;
            Debug.DrawLine(transform.position, transform.position + transform.forward.normalized * distance, Color.red, 10.0f);
        }
        
        laserTransform.localScale = new Vector3(laserStartScale.x, 
                                                distance * laserStartScale.y + extraLength,
                                                laserStartScale.z);
        laserTransform.localPosition = Vector3.forward * (distance * (1.0f - laserStartScale.y) + extraLength);
    }

    private void OnTriggerEnter(Collider other)
    {
        RecalculateLaser();
        if(other.transform == playerScriptable.Player)
        {
            Debug.Log("Found Player");
        }
    }
    void FixedUpdate()
    {
        RecalculateLaser();
    }

    private void OnTriggerExit(Collider other)
    {
        RecalculateLaser();
         Debug.Log("Lost Player");
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            if(laserStartScale == Vector3.zero)
            {
                SetupStartScale();
            }
            RecalculateLaser();
        }
    }
#endif
}
