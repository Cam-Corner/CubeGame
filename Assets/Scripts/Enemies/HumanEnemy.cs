using System.Collections;
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

    //=========================================
    //Inspector Properties
    [SerializeField] protected EnemySettings m_EnemySettings;
    [Space(3)]
    [SerializeField] protected EnemyPath m_MyPath;
    [Space(5)]
    [SerializeField] private GameObject m_Player;
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
    private bool m_InRadiusOfPlayer = false;
    private sPathPoint m_CurrentPP;
    private NavMeshAgent m_NMA;
    private Vector3 m_LastPlayerSighting;
    private eEnemyState m_CurrentState = eEnemyState.EES_Patrol;
    //=========================================

    private void Start()
    {
        //Line Of Sight
        m_LineOfSight = GetComponentInChildren<LineOfSight>();
        //m_PRC = GetComponentInChildren<PlayerRadiusChecker>();

        m_NMA = GetComponent<NavMeshAgent>();

        if (m_MyPath != null)
        {
            transform.position = m_MyPath.GetStartPoint();
            m_CurrentPP = m_MyPath.GetNextPoint();
            //m_CurrentGotoPatrolPoint = m_MyPath.GetNextPoint();
            m_NMA.SetDestination(m_CurrentPP.m_GotoPosition);
        }

        TurnBasedSystem.Instance.IsTimeActiveVar.OnChange += TimeActiveChange;
    }

    private void OnDestroy() 
    {
        TurnBasedSystem.Instance.IsTimeActiveVar.OnChange -= TimeActiveChange;
    }

    private void TimeActiveChange(bool oldVal, bool newVal)
    {
        if(oldVal == newVal)
        {
            return;
        }

        m_NMA.isStopped = !newVal;
    }

    private void Update()
    {
        if(!TurnBasedSystem.Instance.IsTimeActive)
        {
            return;
        }
        
        switch(m_CurrentState)
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
            default:
                break;              
        }

        Debug.Log("Current AI State: " + m_CurrentState);

        //m_LineOfSight.ClearMesh();
    }

    private void Patroling()
    {
        float Distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                                         new Vector2(m_CurrentPP.m_GotoPosition.x, m_CurrentPP.m_GotoPosition.z));
        if (Distance < 0.5f)
        {
            if (m_CurrentPP.m_WaitTime > 0)
            {
                m_CurrentPP.m_WaitTime -= Time.deltaTime;

                Vector3 LerpRotation = Vector3.Lerp(transform.rotation.eulerAngles, m_CurrentPP.m_Rotation, 5.0f * Time.deltaTime);
                transform.rotation = Quaternion.Euler(LerpRotation);
            }
            else
            {
                m_CurrentPP = m_MyPath.GetNextPoint();

                //m_CurrentGotoPatrolPoint = m_MyPath.GetNextPoint();
                m_NMA.SetDestination(m_CurrentPP.m_GotoPosition);
            }
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
        m_NMA.SetDestination(m_Player.transform.position);

        if (Vector3.Angle((transform.position - Camera.main.transform.position).normalized, Camera.main.transform.forward) <= Camera.main.fieldOfView)
        {
            m_LineOfSight.SightCheckNoReturn(!m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FieldOfView : m_FieldOfView,
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FOVDistance : m_FOVDistance,
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_AmountOfRays : m_AmountOfRays);
        }

        RaycastHit Hit;
        bool HitSomething = Physics.Raycast(transform.position, (m_Player.transform.position - transform.position).normalized,
                                            out Hit, 1000);

        if (HitSomething)
        {

            Debug.DrawRay(transform.position, (m_Player.transform.position - transform.position).normalized * Hit.distance, Color.cyan);

            if (Hit.transform.gameObject == m_Player)
            {
                m_LastPlayerSighting = m_Player.transform.position;
            }
            else
            {
                m_LastPlayerSighting = m_Player.transform.position;
                m_CurrentState = eEnemyState.EES_CheckingLastLocation;
            }
        }
        else
        {           
            Debug.DrawRay(transform.position, (m_Player.transform.position - transform.position).normalized * 1000, Color.cyan);
            m_LastPlayerSighting = m_Player.transform.position;
            m_CurrentState = eEnemyState.EES_CheckingLastLocation;
        }

        //Improve Later
        Vector2 A = new Vector2(transform.position.x, transform.position.z);
        Vector2 B = new Vector2(m_Player.transform.position.x, m_Player.transform.position.z);

        if (Vector2.Distance(A, B) < 3)
        {
            m_Player.GetComponent<CubeMovement>().PlayerFound(eFoundPlayerType.EFPT_HumanEnemy);
            m_NMA.SetDestination(m_CurrentPP.m_GotoPosition);
            m_CurrentState = eEnemyState.EES_Patrol;
        }
    }

    void CheckingLastLocation()
    {
        m_NMA.SetDestination(m_LastPlayerSighting);

        if (Vector3.Angle((transform.position - Camera.main.transform.position).normalized, Camera.main.transform.forward) <= Camera.main.fieldOfView)
        {
            m_LineOfSight.SightCheckNoReturn(!m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FieldOfView : m_FieldOfView,
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FOVDistance : m_FOVDistance,
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_AmountOfRays : m_AmountOfRays);
        }

        if (Vector3.Angle(transform.forward, (m_Player.transform.position - transform.position).normalized) < m_EnemySettings.m_FieldOfView)
        {
            RaycastHit Hit;
            bool HitSomething = Physics.Raycast(transform.position, (m_Player.transform.position - transform.position).normalized,
                                                out Hit, 1000);

            if (HitSomething)
            {
                Debug.DrawRay(transform.position, (m_Player.transform.position - transform.position).normalized * Hit.distance, Color.cyan);

                if (Hit.transform.gameObject == m_Player)
                {
                    if (!m_Player.GetComponent<CubeMovement>().PlayerHidden())
                    {
                        m_CurrentState = eEnemyState.EES_ChasingPlayer;
                    }
                }
            }
            else
            {
                Debug.DrawRay(transform.position, (m_Player.transform.position - transform.position).normalized * 1000, Color.cyan);
            }
        }


        //Improve later
        Vector2 A = new Vector2(transform.position.x, transform.position.z);
        Vector2 B = new Vector2(m_LastPlayerSighting.x, m_LastPlayerSighting.z);

        if (Vector2.Distance(A, B) < 3)
        {
            m_NMA.SetDestination(m_CurrentPP.m_GotoPosition);
            m_CurrentState = eEnemyState.EES_Patrol;
        }
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
