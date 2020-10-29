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
    }

    private void Update()
    {
        float Distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), 
                                         new Vector2(m_CurrentPP.m_GotoPosition.x, m_CurrentPP.m_GotoPosition.z));
        if(Distance < 0.5f)
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
    }

    private void FixedUpdate()
    {
        if (m_InRadiusOfPlayer)//m_PRC.PlayerInRadius())
        {
            List<GameObject> ObjectsHit = m_LineOfSight.SightCheck(!m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FieldOfView : m_FieldOfView,
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FOVDistance : m_FOVDistance,
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_AmountOfRays : m_AmountOfRays);

            foreach(GameObject GO in ObjectsHit)
            {
                if(GO == m_Player)
                {
                    m_Player.GetComponent<CubeMovement>().PlayerFound();
                    break;
                }
            }
        }
        else if(Vector3.Angle((transform.position - Camera.main.transform.position).normalized, Camera.main.transform.forward) <= Camera.main.fieldOfView)
        {
            m_LineOfSight.SightCheckNoReturn(!m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FieldOfView : m_FieldOfView,
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_FOVDistance : m_FOVDistance,
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_AmountOfRays / 2 : m_AmountOfRays / 2);
        }
        else
        {
            m_LineOfSight.ClearMesh();
        }

        //Debug.Log(Camera.main.fieldOfView);
    }

    public void SetInRadius(bool InRadius)
    {
        m_InRadiusOfPlayer = InRadius;
    }

}
