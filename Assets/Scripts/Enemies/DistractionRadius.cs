using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class DistractionRadius : MonoBehaviour
{
    //private SphereCollider m_SphereCollider;
    //private bool m_bTriggerHappened = false;
    //private float m_CurrentGotoRadius = 0;
   
    public TransformArrVar m_GaurdInstances;
    public float m_SmashedSoundDistance;

    /* this will change depending on the magnitude of the object */
    public float m_SoundDisPerMagnitudeOfForce;


    private void Start()
    {
        //m_SphereCollider = GetComponent<SphereCollider>();
        //m_SphereCollider.radius = 0.1f;
        //m_SphereCollider.isTrigger = true;

    }

    public void MakeNoise(float ObjectMagnitude, bool ObjectSmashed)
    {

        float SoundDis = 0;
        if (ObjectSmashed)
            SoundDis = m_SmashedSoundDistance * m_SmashedSoundDistance;
        else
        {
            SoundDis = (m_SoundDisPerMagnitudeOfForce * m_SoundDisPerMagnitudeOfForce) * ObjectMagnitude;
            SoundDis = Mathf.Clamp(SoundDis, 0, m_SmashedSoundDistance * m_SmashedSoundDistance);
        }

        foreach (Transform T in m_GaurdInstances.Value)
        {
            //distance check with sqaure rooting
            Vector3 A = transform.position;
            Vector3 B = T.transform.position;
            float Dis = Vector3.SqrMagnitude(A - B);

            if (Dis < SoundDis)
                T.gameObject.GetComponent<HumanEnemy>().HeardANoise(transform.position);
        }

        //if (ObjectSmashed)
        //    m_CurrentGotoRadius = m_SmashedRadius;
        //else
        //    m_CurrentGotoRadius = m_RadiusValePerMagnitudeOfForce * ObjectMagnitude;



        //Debug.Log("Made Noise: Destroyed " + ObjectSmashed + ": Radius = " + m_CurrentGotoRadius);
        //m_SphereCollider.radius = m_CurrentGotoRadius;       
        //StartCoroutine(NextFrameReset());

        //m_bTriggerHappened = true;
    }


    //private IEnumerator NextFrameReset()
    //{
    //yield return new WaitForEndOfFrame();

    //m_CurrentGotoRadius = 0.1f;
    //m_SphereCollider.radius = m_CurrentGotoRadius;
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Enemy")
    //    {
    //        Debug.Log("Enemy in radius!");
    //        RaycastHit Hit;
    //        Vector3 Dir = (other.transform.position - transform.position).normalized;
    //        Vector3 StartPos = transform.position;
    //        StartPos.y += 5;
    //        bool bHitEnemy = Physics.Raycast(StartPos, Dir, out Hit, m_CurrentGotoRadius * 2);
    //        Debug.DrawRay(StartPos, Dir * m_CurrentGotoRadius * 2, Color.red, 5.0f);

    //        if (bHitEnemy)
    //        {
    //            if (Hit.transform.tag == "Enemy")
    //            {
    //                HumanEnemy HE = Hit.transform.gameObject.GetComponent<HumanEnemy>();
    //                HE.HeardANoise(transform.position);
    //            }
    //            else
    //            {
    //                Debug.Log("HIT: " + Hit.transform.name);
    //            }
    //        }

    //    }
    //    else
    //    {
    //        Debug.Log("Trigger Hit with " + other.transform.name);
    //    }
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_SmashedSoundDistance);
    }
}
