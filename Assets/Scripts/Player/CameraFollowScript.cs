using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    ///////////////////
    //public variables
    [Header("Default Camera Settings")]
    public float m_CameraDistanceFromCube = 5;

    public float m_CameraSpeed = 500;

    public float m_CameraSensitivity = 1.5f;

    public bool m_InvertMouseX = false;
    public bool m_InvertMouseY = false;

    [Header("Camera Rotation Clamp")]
    [Range(-89, 89)]
    public float m_RotationMinClamp = -45;

    [Range(-89, 89)]
    public float m_RotationMaxClamp = 45;

    [Header("PID Controller Settings")]
    public float m_Proportional = 0;
    public float m_Integral = 0;
    public float m_Derivative = 0;


    ///////////////////
    //private variables
    private Transform m_PlayerTransform;

    private Vector3 m_ErrorPrior = new Vector3(0, 0, 0);
    private Vector3 m_IntegralPrior = new Vector3(0, 0, 0);

    private Camera m_Camera;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        m_Camera = GetComponentInChildren<Camera>();
        SetCameraDistance();
    }


    // Update is called once per frame
    void Update()
    {
        Vector3 CurrentPosition = transform.position;

        Vector3 GoToPosition = m_PlayerTransform.position; //WorkOutGotoPosition();


        //X value
        CurrentPosition.x += (PIDControllerFunction(ref CurrentPosition.x, GoToPosition.x, ref m_ErrorPrior.x, ref m_IntegralPrior.x) * m_CameraSpeed) * Time.deltaTime;

        //Y value
        CurrentPosition.y += (PIDControllerFunction(ref CurrentPosition.y, GoToPosition.y, ref m_ErrorPrior.y, ref m_IntegralPrior.y) * m_CameraSpeed) * Time.deltaTime;

        //Z value
        CurrentPosition.z += (PIDControllerFunction(ref CurrentPosition.z, GoToPosition.z, ref m_ErrorPrior.z, ref m_IntegralPrior.z) * m_CameraSpeed) * Time.deltaTime;

        transform.position = CurrentPosition;

        CameraRotation();
    }

    void SetCameraDistance()
    {
        //convert cube position to screen position
        Vector3 DefaultPos = transform.position;

        //work out direction
        float Distance = Vector3.Distance(m_Camera.transform.position, DefaultPos);

        Vector3 UnitV = m_Camera.transform.position;
        UnitV.x -= DefaultPos.x;
        UnitV.y -= DefaultPos.y;
        UnitV.z -= DefaultPos.z;

        UnitV = UnitV / Distance;
        
        Vector3 FinalPosition = (UnitV * m_CameraDistanceFromCube);

        m_Camera.transform.localPosition = FinalPosition;
    }

    float PIDControllerFunction(ref float CurrentValue, float DesiredValue, ref float ErrorPrior, ref float IntegralPrior)
    {
        float Error = DesiredValue - CurrentValue;
        float Integral = IntegralPrior + Error * Time.deltaTime;
        float Derivative = (Error - ErrorPrior) / Time.deltaTime;
        float Output = (m_Proportional * ErrorPrior) + (m_Integral * Integral) + (m_Derivative * Derivative);

        ErrorPrior = Error;
        IntegralPrior = Integral;

        return Output;
    }

    void CameraRotation()
    {
        if(Input.GetButton("Look Around"))
        {
            Vector2 MouseMovementThisFrame = (new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * (m_CameraSensitivity * 1000)) * Time.deltaTime;

            Quaternion QOldRotation = transform.rotation;
            Vector3 VOldRotation = QOldRotation.eulerAngles;

            if(m_InvertMouseX)
                VOldRotation.y += MouseMovementThisFrame.x;
            else
                VOldRotation.y -= MouseMovementThisFrame.x;

            if(!m_InvertMouseY)
                VOldRotation.x -= MouseMovementThisFrame.y;
            else
                VOldRotation.x += MouseMovementThisFrame.y;

            QOldRotation = Quaternion.Euler(VOldRotation);

            transform.rotation = QOldRotation;
        }
    }
}
