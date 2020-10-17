using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    ///////////////////
    //public variables
    [Header("Default Camera Settings"), SerializeField]
    private float m_CameraDistanceFromCube = 5;
    
    [SerializeField]
    private float m_CameraSpeed = 500;
    
    [SerializeField]
    private float m_CameraRotationMaxSpeed = 10;

    [SerializeField]
    private AnimationCurve m_CameraRotationSpeedCurve =  new AnimationCurve();
    private float m_timeInRotation = 0;
    private float m_firstRotationInst = 0;
    private float m_lastRotationInst = 0;
    
    [SerializeField]
    private float m_CameraSensitivity = 1.5f;

    [SerializeField]
    private bool m_InvertRotX = false;
    [SerializeField]
    private bool m_InvertRotY = false;

    [Header("Camera Rotation Clamp"), 
    Range(-89, 89), 
    SerializeField]
    private float m_RotationMinClamp = -45;

    [Range(-89, 89), 
    SerializeField]
    private float m_RotationMaxClamp = 45;

    [Header("PID Controller Settings"), 
    SerializeField]
    private float m_Proportional = 0;

    [SerializeField]
    private float m_Integral = 0;

    [SerializeField]
    private float m_Derivative = 0;


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
        SetCameraRotation();
        SetCameraDistance();
    }


    // Update is called once per frame
    void Update()
    {
        CameraRotation();
        Vector3 CurrentPosition = transform.position;

        Vector3 GoToPosition = m_PlayerTransform.position; //WorkOutGotoPosition();


        //X value
        CurrentPosition.x += (PIDControllerFunction(ref CurrentPosition.x, GoToPosition.x, ref m_ErrorPrior.x, ref m_IntegralPrior.x) * m_CameraSpeed) * Time.deltaTime;

        //Y value
        CurrentPosition.y += (PIDControllerFunction(ref CurrentPosition.y, GoToPosition.y, ref m_ErrorPrior.y, ref m_IntegralPrior.y) * m_CameraSpeed) * Time.deltaTime;

        //Z value
        CurrentPosition.z += (PIDControllerFunction(ref CurrentPosition.z, GoToPosition.z, ref m_ErrorPrior.z, ref m_IntegralPrior.z) * m_CameraSpeed) * Time.deltaTime;

        transform.position = CurrentPosition;
    }

    void SetCameraDistance()
    {
        //convert cube position to screen position
        Vector3 DefaultPos = transform.position;

        //work out direction
        float Distance = Vector3.Distance(m_Camera.transform.position, DefaultPos);

        Vector3 UnitV = (m_Camera.transform.position - DefaultPos) / Distance;
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

    private void CameraRotation()
    {
        if(Input.GetButton("Look Around"))
        {
            Vector2 mousPos = new Vector2(Input.GetAxis("Mouse X"), 
                                          Input.GetAxis("Mouse Y"));
            ApplyMouseRotation(mousPos);
        } 
        else
        {
            Vector2 joystickRot = new Vector2(Input.GetAxis("Horizontal Camera"),
                                             Input.GetAxis("Vertical Camera"));
            ApplyCameraRotation(joystickRot);       
        }
    }

    private void ApplyMouseRotation(Vector2 mousPos)
    {
        Quaternion QOldRotation = transform.rotation;
        Vector3 VOldRotation = QOldRotation.eulerAngles;

            Vector2 MouseMovementThisFrame = mousPos 
                                             * m_CameraSensitivity 
                                             * 1000 
                                             * Time.deltaTime;

            if(m_InvertRotX)
                VOldRotation.y += MouseMovementThisFrame.x;
            else
                VOldRotation.y -= MouseMovementThisFrame.x;

            if(!m_InvertRotY)
                VOldRotation.x -= MouseMovementThisFrame.y;
            else
                VOldRotation.x += MouseMovementThisFrame.y;
            VOldRotation.z = 0;
           // VOldRotation.x = Mathf.Clamp(VOldRotation.x, m_RotationMinClamp, m_RotationMaxClamp);
            QOldRotation = Quaternion.Euler(VOldRotation);

            transform.rotation = QOldRotation;
    }
    
    private void SetCameraRotation()
    {
       // transform.LookAt( m_PlayerTransform, transform.up);

        int len = m_CameraRotationSpeedCurve.keys.Length;
        if(len == 0)
        {
            m_firstRotationInst = 0;
            m_lastRotationInst = 0;
        }
        else
        {
            m_firstRotationInst = m_CameraRotationSpeedCurve.keys[0].time;
            m_firstRotationInst = m_CameraRotationSpeedCurve.keys[len-1].time;
        }
        m_timeInRotation = m_firstRotationInst;
    }

    private void ApplyCameraRotation(Vector2 axisInput)
    {
        if(axisInput.magnitude <= 0)
        {
            m_timeInRotation = m_firstRotationInst;
            return;
        }

        Debug.Log("Rot input" + axisInput);

        m_timeInRotation = Mathf.Clamp(m_timeInRotation + Time.deltaTime, m_firstRotationInst, m_lastRotationInst); 
        float rotSpeed = m_CameraRotationSpeedCurve.Evaluate(m_timeInRotation) * m_CameraRotationMaxSpeed;

        Vector2 rotDir = (Vector2.right * axisInput.x + Vector2.up * axisInput.y).normalized;
        rotDir.x = m_InvertRotX? -rotDir.x : rotDir.x;
        rotDir.y = m_InvertRotY? -rotDir.y : rotDir.y;

        Vector3 eulerRotation = transform.rotation.eulerAngles 
                                + (Vector3)rotDir * rotSpeed * Time.deltaTime;
   //     eulerRotation.x = Mathf.Clamp(eulerRotation.x, m_RotationMinClamp, m_RotationMaxClamp);
        transform.rotation = Quaternion.Euler(eulerRotation);
    } 
}

