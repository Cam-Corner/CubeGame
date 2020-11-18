using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using UnityEngine;

public class MissionObjective : MonoBehaviour
{
    [SerializeField] public float m_GrabDistance = 5.0f;
    [SerializeField] public float m_TimeToSteal = 5.0f;


    private float m_CurrentTimeLeft = 0;
    private SphereCollider m_SC;
    private bool m_bPlayerInRange = false;

    private void Start()
    {
        m_CurrentTimeLeft = m_TimeToSteal;

    }
    public float GetGrabDistance() => m_GrabDistance;

    private void Update()
    {
        if (m_bPlayerInRange && m_CurrentTimeLeft > 0)
        {
            m_CurrentTimeLeft -= Time.deltaTime;
            //Debug.Log(m_CurrentTimeLeft);
        }
    }

    public bool ObjectStolen()
    {
        if (m_CurrentTimeLeft <= 0)
            return true;

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
            m_bPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
            m_bPlayerInRange = false;
    }
}
