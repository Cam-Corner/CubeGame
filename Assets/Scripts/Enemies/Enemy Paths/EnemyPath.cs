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
    public sPathPoint GetNextPoint(in int IndexCount)
    {
        sPathPoint PP = new sPathPoint();
        if (IndexCount < m_Path.Count)
        {
            PP.m_GotoPosition = m_Path[(int)IndexCount].transform.position;
            PP.m_Rotation = m_Path[(int)IndexCount].transform.rotation.eulerAngles;
            PP.m_WaitTime = m_Path[(int)IndexCount].GetWaitTime();
            return PP;
        }

        Debug.Log("Enemy Path Script: Get Next Point index count out of range!: Value is " + IndexCount);

        PP.m_GotoPosition = new Vector3(0, 0, 0);
        PP.m_Rotation = transform.rotation.eulerAngles;
        PP.m_WaitTime = 0;

        return PP;

        /*
        sPathPoint PP = new sPathPoint();

        if (m_Path.Count > 0)
        {
            m_CurrentGoToPoint = m_ReversePath ? m_CurrentGoToPoint - 1 : m_CurrentGoToPoint + 1;

            if (m_CurrentGoToPoint > m_Path.Count - 1)
            {
                if (m_PathLoops)
                    m_CurrentGoToPoint = 0;
                else
                {
                    m_ReversePath = !m_ReversePath;
                    m_CurrentGoToPoint -= 2;
                }
            }
            else if(m_CurrentGoToPoint < 0)
            {
                if (m_PathLoops)
                    m_CurrentGoToPoint = m_Path.Count - 1;
                else
                {
                    m_ReversePath = !m_ReversePath;
                    m_CurrentGoToPoint += 2;
                }
            }

            Debug.Log("Path Point Counter: " + m_Path.Count + " | CurrentGoToPoint: " + m_CurrentGoToPoint);

            PP.m_GotoPosition = m_Path[m_CurrentGoToPoint].transform.position;
            PP.m_Rotation = m_Path[m_CurrentGoToPoint].transform.rotation.eulerAngles;
            PP.m_WaitTime = m_Path[m_CurrentGoToPoint].GetWaitTime();

            return PP;//m_Path[m_CurrentGoToPoint].transform.position;

        }

        PP.m_GotoPosition = new Vector3(0, 0, 0);
        PP.m_WaitTime = 0;
        return PP;
        */
        //return new Vector3(0, 0, 0);
    }
     
    public Vector3 GetStartPoint()
    {
        if (m_Path.Count > 0)
        {
            return m_Path[0].transform.position;
        }

        return new Vector3(0, 0, 0);
    }

    public void GetPathDetails(out int MaxIndexCount, out bool bPathLoops)
    {
        MaxIndexCount = m_Path.Count;
        bPathLoops = m_PathLoops;
    }

    public void AddPoint()
    {
        GameObject GO = new GameObject();
        
        GO.transform.position = m_Path.Count > 0 ? m_Path[m_Path.Count - 1].transform.position : transform.position;
        GO.transform.name = "PathPoint (" + m_Path.Count + ")";
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
