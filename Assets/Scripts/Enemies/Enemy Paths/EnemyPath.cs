using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyPath : MonoBehaviour
{
    [Header("Current Path Points")]
    [SerializeField] private List<PathPoint> m_Path = new List<PathPoint>();
 
    [Header("Path Point Settings")]
    [Space(3)]
    [SerializeField] private Color m_PathPointColour = Color.red;
    [Space(3)]
    [SerializeField] private Color m_PathLineColour = Color.yellow;
    [Space(3)]
    [SerializeField] private bool m_PathLoops = false;


    private int m_CurrentGoToPoint = 0;
    private bool m_ReversePath = false;
    public Vector3 GetNextPoint()
    {
        if (m_Path.Count > 0)
        {
            m_CurrentGoToPoint = m_ReversePath ? m_CurrentGoToPoint - 1 : m_CurrentGoToPoint + 1;

            if (m_CurrentGoToPoint > m_Path.Count)
            {
                if (m_PathLoops)
                    m_CurrentGoToPoint = 0;
                else
                    m_ReversePath = !m_ReversePath;
            }
            else if(m_CurrentGoToPoint < 0)
            {
                if (m_PathLoops)
                    m_CurrentGoToPoint = m_Path.Count - 1;
                else
                    m_ReversePath = !m_ReversePath;
            }

            return m_Path[m_CurrentGoToPoint].transform.position;

        }

        return new Vector3(0, 0, 0);
    }

    public Vector3 GetStartPoint()
    {
        if (m_Path.Count > 0)
        {
            return m_Path[0].transform.position;
        }

        return new Vector3(0, 0, 0);
    }

    public void AddPoint()
    {
        GameObject GO = new GameObject();
        
        GO.transform.position = m_Path.Count > 0 ? m_Path[m_Path.Count - 1].transform.position : transform.position;
        GO.transform.name = "PathPoint (" + m_Path.Count +")";
        GO.transform.parent = transform;

        PathPoint PP = GO.AddComponent<PathPoint>();
        PP.SetPathPointColour(m_PathPointColour);
        m_Path.Add(PP);
    }

    public void DeletePoint()
    {
        if(m_Path.Count > 0)
        {
            int i = 0;
            foreach(PathPoint PP in m_Path)
            {
                i++;
                if (i == m_Path.Count)
                {
                    GameObject GO = PP.transform.gameObject;
                    m_Path.Remove(PP);
                    DestroyImmediate(GO);
                }

            }
        }
    }

    private void OnDrawGizmos()
    {
        UpdateColours();

        if (m_Path.Count >= 2)
        {
            int i = 0;
            foreach(PathPoint PP in m_Path)
            {
                Gizmos.color = m_PathLineColour;
                if (m_PathLoops && i + 1 == m_Path.Count)
                {
                    Gizmos.DrawLine(m_Path[i - 1].transform.position, m_Path[i].transform.position);
                    Gizmos.DrawLine(m_Path[i].transform.position, m_Path[0].transform.position);
                }
                else if(i != 0)
                {
                    Gizmos.DrawLine(m_Path[i - 1].transform.position, m_Path[i].transform.position);
                }
                i++;
            }
        }
    }

    public void UpdateColours()
    {
        if (m_Path.Count > 0)
        {
            foreach (PathPoint PP in m_Path)
            {
                PP.SetPathPointColour(m_PathPointColour);
            }
        }
    }
}
