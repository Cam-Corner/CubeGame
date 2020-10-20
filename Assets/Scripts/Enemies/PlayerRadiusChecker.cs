using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRadiusChecker : MonoBehaviour
{
    [SerializeField] private uint m_RadiusCheck = 10;

    private GameObject m_Player;
    private Rigidbody m_RB;
    private SphereCollider m_SC;
    bool m_PlayerInRadius = false;

    // Start is called before the first frame update
    void Start()
    {
        m_SC = GetComponent<SphereCollider>();
        m_SC.isTrigger = true;

        m_Player = GameObject.FindGameObjectWithTag("Player");

        m_RB = GetComponent<Rigidbody>();
        m_RB.isKinematic = true;
        m_RB.useGravity = false;
    }

    private void Update()
    {
        m_SC.radius = m_RadiusCheck;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_Player != null)
        {
            if (other.gameObject == m_Player)
            {
                m_PlayerInRadius = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_Player != null)
        {
            if (other.gameObject == m_Player)
            {
                m_PlayerInRadius = false;
            }
        }
    }

    public bool PlayerInRadius()
    {
        return m_PlayerInRadius;
    }
}
