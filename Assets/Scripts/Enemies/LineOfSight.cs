using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{   
    public EnemyManager m_EnemyManager;


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        WorkOutView();
        
    }

    void WorkOutView()
    {
        Vector3 Facing = transform.forward;

        Vector3 Dir = (m_EnemyManager.m_Player.transform.position - transform.position).normalized;
        float Angle = Vector3.Angle(Dir, Facing);

        if (Angle < m_EnemyManager.m_FieldOfView * 0.5f)
        {
            
            //RaycastHit Hit;
            //bool HitSomething = Physics.Raycast(transform.position, Dir, out Hit, Mathf.Infinity);

            //if (HitSomething)
            //{
            //    if(Hit.transform.gameObject.tag == "Player")
            //        Debug.DrawRay(transform.position, Dir * 100, Color.red, 0.1f);
            //}

            RaycastCheck();
        }
    }

    void RaycastCheck()
    {
        List<Vector3> PlayerPositions = new List<Vector3>();
        Vector3 PlayerPosition = m_EnemyManager.m_Player.transform.position;

        PlayerPositions.Add(PlayerPosition);
        PlayerPositions.Add(new Vector3(PlayerPosition.x - 0.4f, PlayerPosition.y - 0.4f, PlayerPosition.z + 0.4f));
        PlayerPositions.Add(new Vector3(PlayerPosition.x + 0.4f, PlayerPosition.y - 0.4f, PlayerPosition.z + 0.4f));
        PlayerPositions.Add(new Vector3(PlayerPosition.x - 0.4f, PlayerPosition.y + 0.4f, PlayerPosition.z + 0.4f));
        PlayerPositions.Add(new Vector3(PlayerPosition.x + 0.4f, PlayerPosition.y + 0.4f, PlayerPosition.z + 0.4f));
        PlayerPositions.Add(new Vector3(PlayerPosition.x - 0.4f, PlayerPosition.y - 0.4f, PlayerPosition.z - 0.4f));
        PlayerPositions.Add(new Vector3(PlayerPosition.x + 0.4f, PlayerPosition.y - 0.4f, PlayerPosition.z - 0.4f));
        PlayerPositions.Add(new Vector3(PlayerPosition.x - 0.4f, PlayerPosition.y + 0.4f, PlayerPosition.z - 0.4f));
        PlayerPositions.Add(new Vector3(PlayerPosition.x + 0.4f, PlayerPosition.y + 0.4f, PlayerPosition.z - 0.4f));

        foreach(Vector3 Pos in PlayerPositions)
        {
            Vector3 Dir = (Pos - transform.position).normalized;
            RaycastHit Hit;
            bool HitSomething = Physics.Raycast(transform.position, Dir, out Hit, Mathf.Infinity);

            if(HitSomething)
            {
                if(Hit.transform.gameObject.tag == "Player")
                {
                    Debug.DrawRay(transform.position, Dir * 100, Color.red, 0.1f);
                }
                else
                {
                    Debug.DrawRay(transform.position, Dir * Hit.distance, Color.yellow, 0.1f);
                }
            }
            else
            {
                Debug.DrawRay(transform.position, Dir * 100, Color.yellow, 0.1f);
            }
            

            
        }
    }     
}
