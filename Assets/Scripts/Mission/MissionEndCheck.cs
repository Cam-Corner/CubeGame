using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionEndCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (MissionManager.Instance != null)
            {
                if (MissionManager.Instance.MissionComplete())
                {
                    Debug.Log("Mission: MISSION COMPLETE!");
                    SceneManager.LoadScene("MissionComplete");
                }
                else
                {
                    Debug.Log("MISSION: MISSING OBJECTIVES!");
                }
            }
        }
    }
}
