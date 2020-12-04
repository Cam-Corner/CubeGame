using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using UnityEngine;

public class MissionObjective : MonoBehaviour
{
    [SerializeField] public float m_GrabDistance = 5.0f;
    [SerializeField] public float m_TimeToSteal = 5.0f;

    [SerializeField] MissionObjectiveVar objectBeingStolen;

    [SerializeField] private PlayerScriptable player;
    [SerializeField] private BoolVar lootGame;

    [SerializeField] private GlobalMissionSettings m_MissionSettings;

    private float m_CurrentTimeLeft = float.MaxValue;

    private SphereCollider m_SC;
    private bool m_bPlayerInRange = false;

    public bool IsBeingStolen => (m_CurrentTimeLeft > 0 && m_bPlayerInRange && objectBeingStolen.Value == this && !player.Movement.IsUp);
    public float RatioToSteal => Mathf.Clamp01(m_CurrentTimeLeft / m_TimeToSteal);

    private void Start()
    {
        m_CurrentTimeLeft = m_TimeToSteal;

    }
    public float GetGrabDistance() => m_GrabDistance;
    
    private void Update()
    {
        if(!TurnBasedSystem.Instance.IsTimeActive || m_MissionSettings.GetMissionType() != eMissionType.EMT_Loot)
        {
            return;
        }
        if(player == null || player.Movement.IsUp)
        {
            m_CurrentTimeLeft = m_TimeToSteal;
            return;
        }

        if (IsBeingStolen)
        {
            m_CurrentTimeLeft -= Time.deltaTime;
            //Debug.Log(m_CurrentTimeLeft);
        }
        else if(objectBeingStolen.Value == null && m_bPlayerInRange && !ObjectStolen())
        {
            objectBeingStolen.Value = this;
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
        if (other.transform.tag == "Player" && lootGame.Value)
        {
            m_bPlayerInRange = true;

            if(objectBeingStolen.Value == null)
            {
                objectBeingStolen.Value = this;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player" && lootGame.Value)
        {
            m_bPlayerInRange = false;
            m_CurrentTimeLeft = m_TimeToSteal;

            if(objectBeingStolen.Value == this)
            {
                objectBeingStolen.Value = null;
            }
        }
    }
}
