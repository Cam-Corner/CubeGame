using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("Rotation Points")]
    [Range(0, 360)]
    [SerializeField] private float m_RotationPointA = 0.0f;
    [Range(0, 360)]
    [SerializeField] private float m_RotationPointB = 90.0f;
    [SerializeField] private bool m_RotateClockwise = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
