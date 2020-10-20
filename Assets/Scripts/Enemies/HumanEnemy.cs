using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanEnemy : MonoBehaviour
{
    //=========================================
    //Inspector Properties
    [SerializeField] protected EnemySettings m_EnemySettings;
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

    //=========================================

    private void Start()
    {
        //Line Of Sight
        m_LineOfSight = GetComponentInChildren<LineOfSight>();
        //m_PRC = GetComponentInChildren<PlayerRadiusChecker>();
    }

    private void Update()
    {
        
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
                                     !m_OverrideDefaultLineOfSightSettings ? m_EnemySettings.m_AmountOfRays / 4 : m_AmountOfRays / 4);
        }
        else
        {
            m_LineOfSight.ClearMesh();
        }

        Debug.Log(Camera.main.fieldOfView);
    }

    public void SetInRadius(bool InRadius)
    {
        m_InRadiusOfPlayer = InRadius;
    }

}
