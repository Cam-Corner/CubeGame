﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct sPathPoint
{
    public Vector3 m_GotoPosition;
    public Vector3 m_Rotation;
    public float m_WaitTime;
}

public class HumanEnemy : MonoBehaviour
{
    struct sPathDetails
    {
        public uint m_MaxPathIndex;
        public uint m_CurrentPathIndex;
        public bool m_bPathLoops;
        public bool m_bReversePath;
    }

    //=========================================
    //Inspector Properties
    [SerializeField] protected EnemySettings m_EnemySettings;
    [Space(3)]
    [Header("Patrol Path Settings")]
    [SerializeField] protected EnemyPath m_MyPath;
    [SerializeField] protected uint m_StartingPathIndex = 0;
    [SerializeField] protected bool m_TeleportToStartPoint = false;
    [Space(5)]
    [SerializeField] private GameObject m_Player;
    [SerializeField] private float m_WalkSpeed = 3.0f;
    [SerializeField] private float m_RunSpeed = 5.0f;
    [Header("Line of Sight (Only takes affect when overriding the default settings)")]
    [SerializeField] protected bool m_OverrideDefaultLineOfSightSettings = false;
    [SerializeField] protected uint m_FieldOfView = 45;
    [SerializeField] protected uint m_FOVDistance = 45;
    [SerializeField] protected uint m_AmountOfRays = 50;
    //=========================================
    enum eEnemyState
    {
        EES_Patrol = 0,
        EES_ChasingPlayer = 1,
        EES_CheckingLastLocation = 2,
        EES_AngryRoar = 3,
        EES_CaughtPlayer = 4
    }

    //Non inspector properties
    //=========================================
    //protected
    protected float m_CurrentHealth = 100;
    protected LineOfSight m_LineOfSight;
    //protected PlayerRadiusChecker m_PRC;
    //=========================================

    //=========================================
    //private
    private sPathDetails m_MyPathDetails;
    private bool m_InRadiusOfPlayer = false;
    private sPathPoint m_CurrentPP;
    private NavMeshAgent m_NMA;
    private Vector3 m_LastPlayerSighting;
    private eEnemyState m_CurrentState = eEnemyState.EES_Patrol;
    private Animator m_AC;
    private float m_AngryRoarTimer = 0;
    //=========================================

    private void Start()
    {
        //Line Of Sight
        m_LineOfSight = GetComponentInChildren<LineOfSight>();
        //m_PRC = GetComponentInChildren<PlayerRadiusChecker>();

        m_NMA = GetComponent<NavMeshAgent>();

        m_AC = GetComponent<Animator>();

        if (m_MyPath != null)
        {
            m_MyPath.GetPathDetails(out m_MyPathDetails.m_MaxPathIndex, out m_MyPathDetails.m_bPathLoops);
            m_MyPathDetails.m_CurrentPathIndex = 0;
            m_MyPathDetails.m_bReversePath = false;
            SetStartingPathIndex();
            m_NMA.SetDestination(m_CurrentPP.m_GotoPosition);

        }

        m_AC.SetBool("NearPlayer", true);
    }

    private void Update()
    {
        m_LineOfSight.ClearMesh();
        ResetAnimationControllerToDefault();

        switch (m_CurrentState)
        {
            case eEnemyState.EES_Patrol:
                Patroling();
                break;
            case eEnemyState.EES_ChasingPlayer:
                ChasingPlayer();
                break;
            case eEnemyState.EES_CheckingLastLocation:
                CheckingLastLocation();
                break;
            case eEnemyState.EES_AngryRoar:
                AngryRoar();
                break;
            default:
                m_NMA.speed = 0.0f;
                break;              
        }
    }

    private void Patroling()
    {
        float Distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                                         new Vector2(m_CurrentPP.m_GotoPosition.x, m_CurrentPP.m_GotoPosition.z));
        if (Distance < 0.5f)
        {
            m_NMA.speed = 0.0f;
            if (m_CurrentPP.m_WaitTime > 0)
            {
                ResetAnimationControllerToDefault();
                m_CurrentPP.m_WaitTime -= Time.deltaTime;

                Vector3 LerpRotation = Vector3.Lerp(transform.rotation.eulerAngles, m_CurrentPP.m_Rotation, 5.0f * Time.deltaTime);
                transform.rotation = Quaternion.Euler(LerpRotation);
            }
            else
            {
                //m_CurrentPP = m_MyPath.GetNextPoint();

                //m_CurrentGotoPatrolPoint = m_MyPath.GetNextPoint();
                SetNextPathPoint();
                m_NMA.SetDestination(m_CurrentPP.m_GotoPosition);
            }
        }
        else
        {
            m_AC.SetBool("Walking", true);
            m_NMA.speed = m_WalkSpeed;
        }

        if (m_InRadiusOfPlayer)//m_PRC.PlayerInRadius())
        {
            List<GameObject> ObjectsHit = m_LineOfSight.SightCheck(!m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FieldOfView : m_FieldOfView,
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FOVDistance : m_FOVDistance,
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_AmountOfRays : m_AmountOfRays);

            foreach (GameObject GO in ObjectsHit)
            {
                if (GO == m_Player)
                {
                    //m_Player.GetComponent<CubeMovement>().PlayerFound();
                    if (!m_Player.GetComponent<CubeMovement>().PlayerHidden())
                    {
                        m_CurrentState = eEnemyState.EES_ChasingPlayer;
                        Debug.Log(transform.name + ": Current AI State: " + m_CurrentState);
                    }                    
                    break;
                }
            }
        }
        else if (Vector3.Angle((transform.position - Camera.main.transform.position).normalized, Camera.main.transform.forward) <= Camera.main.fieldOfView)
        {
            m_LineOfSight.SightCheckNoReturn(!m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FieldOfView : m_FieldOfView,
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FOVDistance : m_FOVDistance,
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_AmountOfRays : m_AmountOfRays);
        }
    }

    void ChasingPlayer()
    {
        m_NMA.speed = m_RunSpeed;
        m_AC.SetBool("Running", true);

        m_NMA.SetDestination(m_Player.transform.position);

        /*
        if (Vector3.Angle((transform.position - Camera.main.transform.position).normalized, Camera.main.transform.forward) <= Camera.main.fieldOfView)
        {
            m_LineOfSight.SightCheckNoReturn(!m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FieldOfView : m_FieldOfView,
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FOVDistance : m_FOVDistance,
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_AmountOfRays : m_AmountOfRays);
        }
        */

        RaycastHit Hit;
        bool HitSomething = Physics.Raycast(transform.position, (m_Player.transform.position - transform.position).normalized,
                                            out Hit, 1000);

        if (HitSomething)
        {

            //Debug.DrawRay(transform.position, (m_Player.transform.position - transform.position).normalized * Hit.distance, Color.cyan);

            if (Hit.transform.gameObject == m_Player)
            {
                m_LastPlayerSighting = m_Player.transform.position;
            }
            else
            {
                m_LastPlayerSighting = m_Player.transform.position;
                m_CurrentState = eEnemyState.EES_CheckingLastLocation;
                Debug.Log(transform.name + ": Current AI State: " + m_CurrentState);
            }
        }
        else
        {           
            //Debug.DrawRay(transform.position, (m_Player.transform.position - transform.position).normalized * 1000, Color.cyan);
            m_LastPlayerSighting = m_Player.transform.position;
            m_CurrentState = eEnemyState.EES_CheckingLastLocation;
            Debug.Log(transform.name + ": Current AI State: " + m_CurrentState);
        }

        //Improve Later
        Vector2 A = new Vector2(transform.position.x, transform.position.z);
        Vector2 B = new Vector2(m_Player.transform.position.x, m_Player.transform.position.z);

        if (Vector2.Distance(A, B) < 3)
        {
            m_Player.GetComponent<CubeMovement>().PlayerFound(eFoundPlayerType.EFPT_HumanEnemy);
            m_NMA.SetDestination(m_CurrentPP.m_GotoPosition);
            m_CurrentState = eEnemyState.EES_Patrol;
            Debug.Log(transform.name + ": Current AI State: " + m_CurrentState);
        }
    }

    void CheckingLastLocation()
    {
        m_NMA.speed = m_RunSpeed;
        m_AC.SetBool("Running", true);

        m_NMA.SetDestination(m_LastPlayerSighting);

        /*
        if (Vector3.Angle((transform.position - Camera.main.transform.position).normalized, Camera.main.transform.forward) <= Camera.main.fieldOfView)
        {
            m_LineOfSight.SightCheckNoReturn(!m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FieldOfView : m_FieldOfView,
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FOVDistance : m_FOVDistance,
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_AmountOfRays : m_AmountOfRays);
        }
        */

        if (Vector3.Angle(transform.forward, (m_Player.transform.position - transform.position).normalized) < 180)
        {
            RaycastHit Hit;
            bool HitSomething = Physics.Raycast(transform.position, (m_Player.transform.position - transform.position).normalized,
                                                out Hit, 1000);

            if (HitSomething)
            {
                //Debug.DrawRay(transform.position, (m_Player.transform.position - transform.position).normalized * Hit.distance, Color.cyan);

                if (Hit.transform.gameObject == m_Player)
                {
                    if (!m_Player.GetComponent<CubeMovement>().PlayerHidden())
                    {
                        m_CurrentState = eEnemyState.EES_ChasingPlayer;
                        Debug.Log(transform.name + ": Current AI State: " + m_CurrentState);
                    }
                }
            }
            else
            {
                //Debug.DrawRay(transform.position, (m_Player.transform.position - transform.position).normalized * 1000, Color.cyan);
            }
        }


        //Improve later
        Vector2 A = new Vector2(transform.position.x, transform.position.z);
        Vector2 B = new Vector2(m_LastPlayerSighting.x, m_LastPlayerSighting.z);

        if (Vector2.Distance(A, B) < 3)
        {
            m_NMA.SetDestination(transform.position);
            m_NMA.speed = 0.0f;
            //m_CurrentState = eEnemyState.EES_Patrol;
            m_AngryRoarTimer = 5.0f;
            m_CurrentState = eEnemyState.EES_AngryRoar;
            Debug.Log(transform.name + ": Current AI State: " + m_CurrentState);
        }
    }

    void AngryRoar()
    {
        m_AC.SetBool("AngryRoar", true);

        if (m_AngryRoarTimer <= 0)
        {
            m_NMA.SetDestination(m_CurrentPP.m_GotoPosition);
            m_CurrentState = eEnemyState.EES_Patrol;
            Debug.Log(transform.name + ": Current AI State: " + m_CurrentState);
        }
        else
        {
            m_AngryRoarTimer -= 1 * Time.deltaTime;
        }
    }

    void SetNextPathPoint()
    {
        m_CurrentPP = m_MyPath.GetNextPoint(in m_MyPathDetails.m_CurrentPathIndex);
        
        if (m_MyPathDetails.m_MaxPathIndex > 0)
        {
            m_MyPathDetails.m_CurrentPathIndex = m_MyPathDetails.m_bReversePath ?
                                                 m_MyPathDetails.m_CurrentPathIndex - 1 :
                                                 m_MyPathDetails.m_CurrentPathIndex + 1;

            if (m_MyPathDetails.m_CurrentPathIndex > m_MyPathDetails.m_MaxPathIndex - 1)
            {
                if (m_MyPathDetails.m_bPathLoops)
                    m_MyPathDetails.m_CurrentPathIndex = 0;
                else
                {
                    m_MyPathDetails.m_bReversePath = !m_MyPathDetails.m_bReversePath;
                    m_MyPathDetails.m_CurrentPathIndex -= 2;
                }
            }
            else if (m_MyPathDetails.m_CurrentPathIndex < 0)
            {
                if (m_MyPathDetails.m_bPathLoops)
                    m_MyPathDetails.m_CurrentPathIndex = m_MyPathDetails.m_MaxPathIndex - 1;
                else
                {
                    m_MyPathDetails.m_bReversePath = !m_MyPathDetails.m_bReversePath;
                    m_MyPathDetails.m_CurrentPathIndex += 2;
                }
            }
        }
    }

    void SetStartingPathIndex()
    {
        if(m_StartingPathIndex >= m_MyPathDetails.m_MaxPathIndex)
        {
            m_MyPathDetails.m_CurrentPathIndex = m_MyPathDetails.m_MaxPathIndex - 1;
            Debug.Log(transform.name +  ": Path point starting index is bigger than the max path index!");
        }
        else
        {
            m_MyPathDetails.m_CurrentPathIndex = m_StartingPathIndex;
        }

        SetNextPathPoint();

        if(m_TeleportToStartPoint)
            transform.position = m_CurrentPP.m_GotoPosition;
    }

    void ResetAnimationControllerToDefault()
    {
        m_AC.SetBool("Walking", false);
        m_AC.SetBool("Running", false);
        m_AC.SetBool("AngryRoar", false);
    }

    //private void FixedUpdate()
    //{
    //    if (m_InRadiusOfPlayer && m_CurrentState == eEnemyState.EES_Patrol)//m_PRC.PlayerInRadius())
    //    {
    //        List<GameObject> ObjectsHit = m_LineOfSight.SightCheck(!m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FieldOfView : m_FieldOfView,
    //                                 !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FOVDistance : m_FOVDistance,
    //                                 !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_AmountOfRays : m_AmountOfRays);

    //        foreach(GameObject GO in ObjectsHit)
    //        {
    //            if(GO == m_Player)
    //            {
    //                m_Player.GetComponent<CubeMovement>().PlayerFound();
    //                m_CurrentState = eEnemyState.EES_ChasingPlayer;
    //                break;
    //            }
    //        }
    //    }
    //    else if(Vector3.Angle((transform.position - Camera.main.transform.position).normalized, Camera.main.transform.forward) <= Camera.main.fieldOfView)
    //    {
    //        m_LineOfSight.SightCheckNoReturn(!m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FieldOfView : m_FieldOfView,
    //                                 !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FOVDistance : m_FOVDistance,
    //                                 !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_AmountOfRays / 2 : m_AmountOfRays / 2);
    //    }

    //    //Debug.Log(Camera.main.fieldOfView);
    //}

    public void SetInRadius(bool InRadius)
    {
        m_InRadiusOfPlayer = InRadius;
    }

}
