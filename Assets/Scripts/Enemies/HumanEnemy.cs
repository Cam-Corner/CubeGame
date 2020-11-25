using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AmoaebaUtils;

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
        public int m_MaxPathIndex;
        public int m_CurrentPathIndex;
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
    [Header("Line of Sight (Only takes affect when overriding the default settings)")]
    [SerializeField] protected bool m_OverrideDefaultLineOfSightSettings = false;
    [SerializeField] protected uint m_FieldOfView = 45;
    [SerializeField] protected uint m_FOVDistance = 45;
    [SerializeField] protected uint m_AmountOfRays = 50;
    

    private Vector3 storedVelocity = Vector3.zero;
    private float m_CurrentDetectionTimer = 0;
    private bool m_bDetectingPlayer = false;
    private float m_CheckForPlayerRandNumTimer = 3;

    //=========================================
    enum eEnemyState
    {
        EES_Patrol = 0,
        EES_ChasingPlayer = 1,
        EES_CheckingLastLocation = 2,
        EES_AngryRoar = 3,
        EES_CaughtPlayer = 4,
        EES_GoingToInvestigatingArea = 5,
        EES_InvestigatingArea = 6,
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
        //m_NMA.velocity = Vector3.zero;
        //m_NMA.isStopped = !newVal;
        if(newVal)
        {
            m_NMA.velocity = storedVelocity;
            m_NMA.isStopped = false;
        }
        else
        {
            storedVelocity = m_NMA.velocity; 
            m_NMA.velocity = Vector3.zero;
            m_NMA.isStopped = true;
        }
    }

    private void Update()
    {
        if(!TurnBasedSystem.Instance.IsTimeActive)
        {
            m_AC.enabled = false;
            return;
        }
        m_AC.enabled = true;
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
            case eEnemyState.EES_InvestigatingArea:
                InvestigationArea();
                break;
            case eEnemyState.EES_GoingToInvestigatingArea:
                GoingToInvestigatingArea();
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

                if (m_EnemySettings.GetSuspicionLevel() < 3)
                {
                    SetNextPathPoint();
                    m_NMA.SetDestination(m_CurrentPP.m_GotoPosition);
                }
                else
                {
                    int RandomNum = Random.Range(0, 100);
                    //Debug.Log(RandomNum);
                    m_CheckForPlayerRandNumTimer = 3.0f;

                    if (RandomNum > 30 && RandomNum < 70 && m_EnemySettings.GetBrokenPositions().Count > 0)
                    {
                        int RandomTransform = Random.Range(0, m_EnemySettings.GetBrokenPositions().Count - 1);

                        HeardANoise(m_EnemySettings.GetBrokenPositions()[RandomTransform]);
                    }
                    else
                    {
                        SetNextPathPoint();
                        m_NMA.SetDestination(m_CurrentPP.m_GotoPosition);
                    }
                }
            }
        }
        else
        {
            m_AC.SetBool("Walking", true);
            m_NMA.speed = m_EnemySettings.m_WalkSpeed;
        }

        if (m_EnemySettings.GetSuspicionLevel() >= 2)
        {
            //distance check with sqaure rooting
            Vector3 A = transform.position;
            Vector3 B = m_Player.transform.position;
            float UnSqauredDistanceX = (A.x - B.x) * (A.x - B.x);
            float UnSqauredDistanceY = (A.y - B.y) * (A.y - B.y);
            float UnSqauredDistanceZ = (A.z - B.z) * (A.z - B.z);
            float UnSqauredFinalDistance = UnSqauredDistanceX + UnSqauredDistanceY + UnSqauredDistanceZ;

            if (m_CheckForPlayerRandNumTimer >= 0)
            {
                m_CheckForPlayerRandNumTimer -= Time.deltaTime;
            }

            if (UnSqauredFinalDistance < 450 && m_CheckForPlayerRandNumTimer <= 0)
            {

                int RandomNum = Random.Range(0, 100);
                Debug.Log(RandomNum);
                m_CheckForPlayerRandNumTimer = 3.0f;

                if (RandomNum > 45 && RandomNum < 55)
                {
                    //m_NMA.SetDestination(m_Player.transform.position);
                    //m_NMA.speed = m_EnemySettings.m_WalkSpeed;
                    //m_CurrentState = eEnemyState.EES_RandomlyCheckingNearPlayer;
                    //Debug.Log(transform.name + ": Current AI State: " + m_CurrentState);
                    //m_LastPlayerSighting = m_Player.transform.position;
                    //ResetAnimationControllerToDefault();
                    //m_AC.SetBool("Walking", true);

                    HeardANoise(m_Player.transform.position);
                }
            }
        }

        PlayerSightCheck();
    }

    void ChasingPlayer()
    {
        m_NMA.speed = m_EnemySettings.m_RunSpeed;
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
        m_NMA.speed = m_EnemySettings.m_RunSpeed;
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
        //Debug.Log(transform.name + ": Current Path Index is " + m_MyPathDetails.m_CurrentPathIndex);

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

    void RandomlyCheckingNearPlayer()
    {

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
            m_MyPathDetails.m_CurrentPathIndex = (int)m_StartingPathIndex;
        }

        SetNextPathPoint();

        if(m_TeleportToStartPoint)
            transform.position = m_CurrentPP.m_GotoPosition;
    }

    void GoingToInvestigatingArea()
    {
        m_AC.SetBool("Walking", true);
        //Improve later
        Vector2 A = new Vector2(transform.position.x, transform.position.z);
        Vector2 B = new Vector2(m_LastPlayerSighting.x, m_LastPlayerSighting.z);

        if (Vector2.Distance(A, B) < 3)
        {
            m_NMA.SetDestination(transform.position);
            m_NMA.speed = 0.0f;
            m_AngryRoarTimer = 5.0f;
            m_CurrentState = eEnemyState.EES_InvestigatingArea;
        }

        PlayerSightCheck();
    }

    void InvestigationArea()
    {
        m_AC.SetBool("InvestigatingNoise", true);

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

        PlayerSightCheck();
    }

    void PlayerSightCheck()
    {
        m_bDetectingPlayer = false;

        if (m_InRadiusOfPlayer || !m_InRadiusOfPlayer)//m_PRC.PlayerInRadius())
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
                        if (m_CurrentDetectionTimer <= 0)
                        {
                            m_CurrentDetectionTimer = m_EnemySettings.GetCorrectDetectionTime();
                        }

                        m_bDetectingPlayer = true;
                        //m_CurrentState = eEnemyState.EES_ChasingPlayer;
                        //Debug.Log(transform.name + ": Current AI State: " + m_CurrentState);
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
        
        if(m_bDetectingPlayer)
        {
            m_CurrentDetectionTimer -= Time.deltaTime;
        }
        else if(m_CurrentDetectionTimer <= m_EnemySettings.GetCorrectDetectionTime())
        {
            m_CurrentDetectionTimer += Time.deltaTime;
        }

        if(m_CurrentDetectionTimer > m_EnemySettings.GetCorrectDetectionTime())
        {
            m_CurrentDetectionTimer = -1;
            m_bDetectingPlayer = false;
        }
        else if (m_CurrentDetectionTimer <= 0 && m_bDetectingPlayer)
        {
            m_CurrentState = eEnemyState.EES_ChasingPlayer;
        }

    }

    void ResetAnimationControllerToDefault()
    {
        m_AC.SetBool("Walking", false);
        m_AC.SetBool("Running", false);
        m_AC.SetBool("AngryRoar", false);
        m_AC.SetBool("InvestigatingNoise", false);
    }

    public void SetInRadius(bool InRadius)
    {
        m_InRadiusOfPlayer = InRadius;
    }

    public void HeardANoise(Vector3 NoiseLocation)
    {
        m_NMA.SetDestination(NoiseLocation);
        m_NMA.speed = m_EnemySettings.m_WalkSpeed;
        m_CurrentState = eEnemyState.EES_GoingToInvestigatingArea;
        Debug.Log(transform.name + ": Current AI State: " + m_CurrentState);
        m_LastPlayerSighting = NoiseLocation;
        ResetAnimationControllerToDefault();
        m_AC.SetBool("Walking", true);
    }
}
