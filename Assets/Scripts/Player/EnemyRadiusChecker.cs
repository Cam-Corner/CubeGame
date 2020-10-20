using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRadiusChecker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        HumanEnemy HE = other.GetComponent<HumanEnemy>();

        if (HE != null)
        {
            HE.SetInRadius(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HumanEnemy HE = other.GetComponent<HumanEnemy>();

        if (HE != null)
        {
            HE.SetInRadius(false);
        }
    }
}
