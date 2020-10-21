using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint : MonoBehaviour
{
    [SerializeField] private Color m_Colour = Color.red;

    public void SetPathPointColour(Color PointColour) { m_Colour = PointColour; }


    private void OnDrawGizmos()
    {
        Gizmos.color = m_Colour;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
