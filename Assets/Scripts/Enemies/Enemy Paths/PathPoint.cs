using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PathPoint : MonoBehaviour
{
    [SerializeField] private Color m_Colour = Color.red;
    [SerializeField] private float m_WaitTime = 0;


    public void SetPathPointColour(Color PointColour) => m_Colour = PointColour; 


    private void OnDrawGizmos()
    {
        Gizmos.color = m_Colour;
        Gizmos.DrawSphere(transform.position, 1);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * 2));
    }

    public float GetWaitTime() => m_WaitTime;
}
