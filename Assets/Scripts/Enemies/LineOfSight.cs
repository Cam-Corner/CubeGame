using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

public class LineOfSight : MonoBehaviour
{
    
    public List<GameObject> SightCheck(in uint FieldOfView, in uint FOVDistance, in uint NumberOfRays)
    {
        float DistanceBetweenRays = (float)FieldOfView / ((float)NumberOfRays - 1);
        Vector3 Direction = Quaternion.AngleAxis(-(FieldOfView / 2), Vector3.up) * transform.parent.transform.forward;
        List<GameObject> ObjectsHit = new List<GameObject>();

        //Debug.DrawRay(transform.position, transform.forward * FOVDistance, Color.green, 0.1f);

        List<Vector3> Vertices = new List<Vector3>();
        Vertices.Add(new Vector3(0, 0, 0));

        for (int i = 0; i < NumberOfRays; i++)
        {
            RaycastHit Hit;
            LayerMask Mask =~ LayerMask.GetMask("Destructables");
            bool HitSomething = Physics.Raycast(transform.position, Direction, out Hit, FOVDistance, Mask);

            if (HitSomething)
            {
                Vertices.Add(Direction * Hit.distance);
                //Debug.DrawRay(transform.position, Direction * Hit.distance, Color.red, 0.1f);
                ObjectsHit.Add(Hit.transform.gameObject);
            }
            else
            {
                Vertices.Add(Direction * FOVDistance);
                //Debug.DrawRay(transform.position, Direction * FOVDistance, Color.red, 0.1f);
            }

            if (i != NumberOfRays - 1)
                Direction = Quaternion.AngleAxis(DistanceBetweenRays, Vector3.up) * Direction;
        }

        DrawLineOfSight(Vertices, NumberOfRays);

        return ObjectsHit;
    }

    public void SightCheckNoReturn(in uint FieldOfView, in uint FOVDistance, in uint NumberOfRays)
    {
        float DistanceBetweenRays = (float)FieldOfView / ((float)NumberOfRays - 1);
        Vector3 Direction = Quaternion.AngleAxis(-(FieldOfView / 2), Vector3.up) * transform.parent.transform.forward;

        //Debug.DrawRay(transform.position, transform.forward * FOVDistance, Color.green, 0.1f);

        List<Vector3> Vertices = new List<Vector3>();
        Vertices.Add(new Vector3(0, 0, 0));

        for (int i = 0; i < NumberOfRays; i++)
        {
            RaycastHit Hit;
            LayerMask Mask = ~LayerMask.GetMask("Destructables");
            bool HitSomething = Physics.Raycast(transform.position, Direction, out Hit, FOVDistance, Mask);

            if (HitSomething)
            {
                Vertices.Add(Direction * Hit.distance);
                //Debug.DrawRay(transform.position, Direction * Hit.distance, Color.red, 0.1f);
            }
            else
            {
                Vertices.Add(Direction * FOVDistance);
                //Debug.DrawRay(transform.position, Direction * FOVDistance, Color.red, 0.1f);
            }

            if (i != NumberOfRays - 1)
                Direction = Quaternion.AngleAxis(DistanceBetweenRays, Vector3.up) * Direction;
        }

        DrawLineOfSight(Vertices, NumberOfRays);
    }

    void DrawLineOfSight(in List<Vector3> Vertices, in uint NumberOfRays)
    {

        MeshRenderer meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        //meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        MeshFilter meshFilter = gameObject.GetComponentInChildren<MeshFilter>();

        Mesh SightMesh = new Mesh();
        //SightMesh.vertices = Vertices.ToArray();

        //calculateVertices
        SightMesh.vertices = Vertices.ToArray();

        //calculate Triangles
        int AmountOfTriangles = Vertices.Count - 2;
        int[] Triangles = new int[(AmountOfTriangles * 3)];
        for (int i = 0; i < AmountOfTriangles; i++)
        {
            int V = i * 3;
            Triangles[V] = 0;
            Triangles[V + 1] = i + 1;
            Triangles[V + 2] = i + 2;
        }
        SightMesh.triangles = Triangles;

        //calculate normals
        Vector3[] Normals = new Vector3[Vertices.Count];
        for (int i = 0; i < Normals.Length; i++)
        {
            Normals[i] = Vector3.up;
        }
        SightMesh.normals = Normals;

        //calculate UVs
        Vector2[] UVs = new Vector2[Vertices.Count];
        for (int i = 0; i < UVs.Length; i++)
        {
            UVs[i] = new Vector2(0, 0);

            if (i + 1 < UVs.Length)
                UVs[i + 1] = new Vector2(0, 1);

            if (i + 2 < UVs.Length)
                UVs[i + 2] = new Vector2(1, 0);

            if (i + 3 < UVs.Length)
                UVs[i + 3] = new Vector2(1, 1);

            i += 4;
        }
        SightMesh.uv = UVs;

        Vector3 NewScale = new Vector3(1, 1, 1);
        NewScale.x = NewScale.x / 30;
        NewScale.y = NewScale.y / 30;
        NewScale.z = NewScale.z / 30;
        transform.localScale = NewScale;
        meshFilter.mesh = SightMesh;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        
    }

    public void ClearMesh()
    {
        MeshFilter meshFilter = gameObject.GetComponentInChildren<MeshFilter>();
        meshFilter.mesh = null;
    }

    //private uint m_FieldOfView = 45;
    //private uint m_FOVDistance = 45;   
    //private uint m_NumberOfRays = 50;

    //public void SetFieldOfView(in uint FieldOfView) 
    //{
    //    m_FieldOfView = FieldOfView;
    //}

    //public void SetFOVDistance(in uint FOVDistance)
    //{
    //    m_FOVDistance = FOVDistance;
    //}

    //public void SetAmountOfRays(in uint AmountOfRays)
    //{
    //    m_NumberOfRays = AmountOfRays;
    //}

    //public void SightCheck(in uint FieldOfView, in uint FOVDistance, in uint AmountOfRays)
    //{
    //    //Vector3 Facing = transform.forward;

    //    //Vector3 Dir = (m_EnemyManager.m_Player.transform.position - transform.position).normalized;
    //    //float Angle = Vector3.Angle(Dir, Facing);

    //    //if (Angle < m_EnemyManager.m_FieldOfView * 0.5f)
    //    //{

    //    //}

    //    RaycastCheck();
    //}


    //void RaycastCheckOld()
    //{
    //    List<Vector3> PlayerPositions = new List<Vector3>();
    //    Vector3 PlayerPosition = m_EnemyManager.m_Player.transform.position;

    //    PlayerPositions.Add(PlayerPosition);
    //    PlayerPositions.Add(new Vector3(PlayerPosition.x - 0.4f, PlayerPosition.y - 0.4f, PlayerPosition.z + 0.4f));
    //    PlayerPositions.Add(new Vector3(PlayerPosition.x + 0.4f, PlayerPosition.y - 0.4f, PlayerPosition.z + 0.4f));
    //    PlayerPositions.Add(new Vector3(PlayerPosition.x - 0.4f, PlayerPosition.y + 0.4f, PlayerPosition.z + 0.4f));
    //    PlayerPositions.Add(new Vector3(PlayerPosition.x + 0.4f, PlayerPosition.y + 0.4f, PlayerPosition.z + 0.4f));
    //    PlayerPositions.Add(new Vector3(PlayerPosition.x - 0.4f, PlayerPosition.y - 0.4f, PlayerPosition.z - 0.4f));
    //    PlayerPositions.Add(new Vector3(PlayerPosition.x + 0.4f, PlayerPosition.y - 0.4f, PlayerPosition.z - 0.4f));
    //    PlayerPositions.Add(new Vector3(PlayerPosition.x - 0.4f, PlayerPosition.y + 0.4f, PlayerPosition.z - 0.4f));
    //    PlayerPositions.Add(new Vector3(PlayerPosition.x + 0.4f, PlayerPosition.y + 0.4f, PlayerPosition.z - 0.4f));

    //    foreach(Vector3 Pos in PlayerPositions)
    //    {
    //        Vector3 Dir = (Pos - transform.position).normalized;
    //        RaycastHit Hit;
    //        bool HitSomething = Physics.Raycast(transform.position, Dir, out Hit, Mathf.Infinity);

    //        if(HitSomething)
    //        {
    //            if(Hit.transform.gameObject.tag == "Player")
    //            {
    //                Debug.DrawRay(transform.position, Dir * 100, Color.red, 0.1f);
    //            }
    //            else
    //            {
    //                Debug.DrawRay(transform.position, Dir * Hit.distance, Color.yellow, 0.1f);
    //            }
    //        }
    //        else
    //        {
    //            Debug.DrawRay(transform.position, Dir * 100, Color.yellow, 0.1f);
    //        }



    //    }
    //}     
}
