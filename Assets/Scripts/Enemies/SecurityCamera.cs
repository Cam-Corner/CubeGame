using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("Rotation Points")]
    [Range(-180, 0)]
    [SerializeField] private float m_RotationPointA = 0.0f;
    [Range(0, 180)]
    [SerializeField] private float m_RotationPointB = 90.0f;
    //[SerializeField] private bool m_RotateClockwise = false;
    [Header("Rotation Values")]
    [SerializeField] private float m_RotationSpeed = 50.0f;
    [SerializeField] private float m_WaitTimer = 2.0f;
    //[SerializeField] private float m_FieldOfViewHorizontal = 90.0f;
    //[SerializeField] private float m_FieldOfViewVertical = 10.0f;
    [Header("Camera Sight Values")]
    [Range(1, 180)]
    [SerializeField] private float m_Radius = 25.0f;
    [SerializeField] private float m_RayDistance = 90.0f;
    [Min(10)]
    [SerializeField] private int m_NumberOfRays = 10;
    [Header("Game Objects")]
    [SerializeField] private GameObject m_CameraMesh;
    [SerializeField] private GameObject m_CameraRotationPart;
    [SerializeField] private GameObject m_SightMesh;
    [SerializeField] private GameObject m_SightMeshFloor;

    private bool m_GoClockwise = false;
    private float m_CurrentYRotation = 0;
    private float m_CurrentWaitTime = 0;
    private LineOfSight m_LOS;

    
    // Start is called before the first frame update
    void Start()
    {
        m_CurrentYRotation = 0;
        m_LOS = GetComponent<LineOfSight>();
        m_CurrentWaitTime = m_WaitTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if(!TurnBasedSystem.Instance.IsTimeActive)
        {
             SightCheck(false);
            return;
        }else
        {
            UpdateRotation();
            //SightCheck((int)m_FieldOfView, (int)m_FOVDistance, (int)m_NumberRays);
            //m_LOS.SightCheckNoReturn(90, 90, 50);

            //CameraRays();

            SightCheck();   
        }
    }

    private void UpdateRotation()
    {
        if (m_GoClockwise)
        {                     
            if (m_CurrentYRotation < m_RotationPointA)
            {
                if (m_CurrentWaitTime < 0)
                {
                    m_CurrentWaitTime = m_WaitTimer;


                    m_GoClockwise = !m_GoClockwise;
                }
                else
                {
                    m_CurrentWaitTime -= 1 * Time.deltaTime;
                }
            }
            else
            {
                m_CurrentYRotation -= m_RotationSpeed * Time.deltaTime;
                //CurrentRotation.y -= 
                m_CameraRotationPart.transform.localRotation = Quaternion.Euler(0, m_CurrentYRotation, 0);
            }
        }
        else
        {
            if (m_CurrentYRotation > m_RotationPointB)
            {
                if (m_CurrentWaitTime < 0)
                {
                    m_CurrentWaitTime = m_WaitTimer;

                    m_GoClockwise = !m_GoClockwise;
                }
                else
                {
                    m_CurrentWaitTime -= 1 * Time.deltaTime;
                }
            }
            else
            {
                m_CurrentYRotation += m_RotationSpeed * Time.deltaTime;
                //CurrentRotation.y += m_RotationSpeed * Time.deltaTime;
                m_CameraRotationPart.transform.localRotation = Quaternion.Euler(0, m_CurrentYRotation, 0);
            }

        }
    }

    public void SightCheck(bool detectPlayer = true)
    {
        List<Vector3> Vertices = new List<Vector3>();
        Vertices.Add(new Vector3(0, 0, 0));

        float RadiusDifference = 360 / m_NumberOfRays;
        
        Vector3 RayStartPos = transform.position;
        Vector3 RayDirection = Quaternion.AngleAxis(m_Radius / 2, m_CameraMesh.transform.right) * m_CameraMesh.transform.forward;
        //Debug.DrawRay(RayStartPos, RayDirection * 25, Color.yellow, 0.01f);
        Vector3 NewDirection = RayDirection;

        for (int i = 0; i < m_NumberOfRays; i++)
        {
            NewDirection = Quaternion.AngleAxis(RadiusDifference, m_CameraMesh.transform.forward) * NewDirection;
            
            RaycastHit Hit;
            LayerMask Mask = ~LayerMask.GetMask("Destructables");
            bool HitSomething = Physics.Raycast(m_SightMesh.transform.position, NewDirection, out Hit, m_RayDistance, Mask);//, 1 << LayerMask.NameToLayer("Floor"));
            //Debug.DrawLine(m_SightMesh.transform.position, m_SightMesh.transform.position+ NewDirection*m_RayDistance, Color.red, 10.0f);
            if (HitSomething)
            {
                Vector3 Vertex = (NewDirection * Hit.distance);
                Vertex.y += 0.05f;
                Vertices.Add(Vertex);

                if(Hit.transform.tag == "Player" && detectPlayer)
                {
                    Hit.transform.GetComponent<CubeMovement>().PlayerFound(eFoundPlayerType.EFPT_CCTV_Camera);
                }

                //Debug.DrawRay(m_SightMesh.transform.position, NewDirection * Hit.distance, Color.blue, 0.1f);
            }
            else
            {
                Vertices.Add(NewDirection * m_RayDistance);
                //Debug.DrawRay(transform.position, NewDirection * m_RayDistance, Color.red, 0.1f);
            }
            
        }

        DrawLineOfSight(Vertices, (uint)m_NumberOfRays);
    }

    //void CameraRays()
    //{

    //    List<Vector3> Vertices = new List<Vector3>();
    //    Vertices.Add(new Vector3(0, 0, 0));

    //    List<Vector3> Directions = new List<Vector3>();

    //    Vector3 Direction = Quaternion.AngleAxis(-(m_FieldOfViewHorizontal / 2), m_CameraMesh.transform.up) * m_CameraMesh.transform.forward;
    //    Direction = Quaternion.AngleAxis(-(m_FieldOfViewVertical / 2), m_CameraMesh.transform.right) * Direction;
    //    Directions.Add(Direction);

    //    Direction = Quaternion.AngleAxis(m_FieldOfViewHorizontal, m_CameraMesh.transform.up) * Direction;
    //    Directions.Add(Direction);

    //    Direction = Quaternion.AngleAxis(m_FieldOfViewVertical, m_CameraMesh.transform.right) * Direction;
    //    Directions.Add(Direction);

    //    Direction = Quaternion.AngleAxis(-m_FieldOfViewHorizontal, m_CameraMesh.transform.up) * Direction;
    //    Directions.Add(Direction);

    //    foreach (Vector3 D in Directions)
    //    {
    //        RaycastHit Hit;
    //        bool HitSomething = Physics.Raycast(m_SightMesh.transform.position, D, out Hit, 200, 1 << LayerMask.NameToLayer("Floor"));

    //        if (HitSomething)
    //        {
    //            Vector3 Vertex = (D * Hit.distance);
    //            Vertex.y += 0.05f;
    //            Vertices.Add(Vertex);//  - (m_CameraMesh.transform.forward / 10));
    //            //Debug.DrawRay(m_SightMesh.transform.position, D * Hit.distance, Color.red, 0.1f);
    //        }
    //        else
    //        {
    //            //Vertices.Add(Direction * m_FOVDistance);
    //            //Debug.DrawRay(transform.position, Direction * m_FOVDistance, Color.red, 0.1f);
    //        }
    //    }
    //    DrawLineOfSight(Vertices, 30);
    //}

    void DrawLineOfSight(in List<Vector3> Vertices, in uint NumberOfRays)
    {

        MeshRenderer meshRenderer = m_SightMesh.GetComponent<MeshRenderer>();
        //meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        MeshFilter meshFilter = m_SightMesh.GetComponent<MeshFilter>();

        Mesh SightMesh = new Mesh();
        Mesh SightMeshFloor = new Mesh();
        //SightMesh.vertices = Vertices.ToArray();

        //calculateVertices
        SightMesh.vertices = Vertices.ToArray();
        SightMeshFloor.vertices = Vertices.ToArray();
        //calculate mesh part 1
        int AmountOfTriangles1 = Vertices.Count - 1;
        int[] Triangles1 = new int[(AmountOfTriangles1 * 3)];
        for (int i = 0; i < AmountOfTriangles1; i++)
        {
            if (i == AmountOfTriangles1 - 1)
            {
                int V = i * 3;
                Triangles1[V] = 0;
                Triangles1[V + 1] = 1;
                Triangles1[V + 2] = i + 1;
            }
            else
            {
                int V = i * 3;
                Triangles1[V] = 0;
                Triangles1[V + 1] = i + 2;
                Triangles1[V + 2] = i + 1;
            }
        }

        //calculate mesh part 2
        int AmountOfTriangles2 = Vertices.Count - 2;
        int[] Triangles2 = new int[(AmountOfTriangles2 * 3)]; // * 6
        for(int i = 0; i < AmountOfTriangles2; i++)
        {
            int V = i * 3;
            Triangles2[V] = 1;
            Triangles2[V + 1] = i + 2;
            Triangles2[V + 2] = i + 1;

            //Triangles2[((AmountOfTriangles2 * 6) - 1 - V)] = 1;
            //Triangles2[((AmountOfTriangles2 * 6) - 2 - V)] = i + 2;
            //Triangles2[((AmountOfTriangles2 * 6) - 3 - V)] = i + 1;
        }

        //int[] Triangles =
        //{
        //    0, 4, 1,
        //    0, 2, 3,
        //    0, 3, 4,
        //    0, 1, 2,
        //};

        int[] FinalTriangles = new int[Triangles1.Length + Triangles2.Length];
        Array.Copy(Triangles1, FinalTriangles, Triangles1.Length);
        Array.Copy(Triangles2, 0, FinalTriangles, Triangles1.Length, Triangles2.Length);

        SightMesh.triangles = Triangles1;
        SightMeshFloor.triangles = Triangles2;

        //calculate normals
        Vector3[] Normals = new Vector3[Vertices.Count];
        for (int i = 0; i < Normals.Length; i++)
        {
            Normals[i] = Vector3.up;
        }
        SightMesh.normals = Normals;
        SightMeshFloor.normals = Normals;

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
        SightMeshFloor.uv = UVs;
        m_SightMeshFloor.GetComponent<MeshFilter>().mesh = SightMeshFloor;
        meshFilter.mesh = SightMesh;
           

        m_SightMesh.transform.rotation = Quaternion.Euler(0, 0, 0);
        m_SightMeshFloor.transform.rotation = Quaternion.Euler(0, 0, 0);
        
    }

    private void OnDrawGizmos() 
    {
        float debugRaySize = 2.0f;
        
        Color prevColor = Gizmos.color;
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(m_CameraRotationPart.transform.position, 
                        m_CameraRotationPart.transform.position + Quaternion.Euler(0, m_RotationPointA,0) * transform.forward * debugRaySize);
        Gizmos.DrawLine(m_CameraRotationPart.transform.position, 
                        m_CameraRotationPart.transform.position + Quaternion.Euler(0, m_RotationPointB,0) * transform.forward * debugRaySize);

        if(!Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            float RadiusDifference = 360 / m_NumberOfRays;
            Vector3 RayStartPos = transform.position;
            Vector3 RayDirection = Quaternion.AngleAxis(m_Radius / 2, m_CameraMesh.transform.right) * m_CameraMesh.transform.forward;
            Vector3 NewDirection = RayDirection;

            for (int i = 0; i < m_NumberOfRays; i++)
            {
                NewDirection = Quaternion.AngleAxis(RadiusDifference, m_CameraMesh.transform.forward) * NewDirection;
                Gizmos.DrawLine(m_SightMesh.transform.position, NewDirection * m_RayDistance);
            }
        }

        Gizmos.color = prevColor;
    }
}
