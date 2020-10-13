using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EnemyMananger", order = 1)]
public class EnemyManager : ScriptableObject
{
    public float m_FieldOfView = 90.0f;
    public float m_ViewDistance = 50.0f;
    public GameObject m_Player;

    void OnEnable()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
    }
}
