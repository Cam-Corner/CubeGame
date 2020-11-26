 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class CameraFollowScript : MonoBehaviour
{
    [SerializeField]
    private InputMapping inputMapping;


    [SerializeField]
    private GlobalMissionSettings m_MissionSettings;

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
    private FloatVar m_CameraSensitivity;
    [SerializeField]
    private float minCameraSensitivity = 0.1f;
    [SerializeField]
    private float maxCameraSensitivity = 5.0f;
    private float CameraSensitivity => Mathf.Lerp(minCameraSensitivity, maxCameraSensitivity, m_CameraSensitivity.Value);

    [SerializeField]
    private BoolVar invertCameraX;

    [SerializeField]
    private BoolVar invertCameraY;

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

    [SerializeField]
    private float minZoom = 10.0f;
    [SerializeField]
    private float maxZoom = 10.0f;
    float zoomOffset = 0;

    [SerializeField]
    private FloatVar zoomSensitivity;

    [SerializeField]
    private BoolVar invertZoom;

        [SerializeField]
    private MenuHelper menuHelper;


    ///////////////////
    //private variables
    private Transform m_PlayerTransform;

    private Vector3 m_ErrorPrior = new Vector3(0, 0, 0);
    private Vector3 m_IntegralPrior = new Vector3(0, 0, 0);

    private Camera m_Camera;
    public Camera GameCamera => m_Camera;

    public CameraRotationDefinition[] preDefinedRotations;

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
        if (m_MissionSettings.GetMissionState() != eMissionState.EMS_PlayingMission)
            return;

        //SetCameraDistance();
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

        CameraZoom();
    }

    private void CameraZoom()
    {
        if(menuHelper.InSettings)
        {
            return;
        }

        float dir = inputMapping.GetZoomInDir();
        dir *= invertZoom.Value? -1.0f : 1.0f;
        if(dir != 0)
        {
            float speedDelta = Time.deltaTime * dir * zoomSensitivity.Value;
            float intendedZoom = zoomOffset + speedDelta;
            zoomOffset = Mathf.Clamp(speedDelta + zoomOffset, minZoom, maxZoom);
            speedDelta = intendedZoom == zoomOffset? speedDelta : speedDelta + (zoomOffset - intendedZoom);
            
            m_Camera.transform.position += speedDelta * m_Camera.transform.forward;
        }
    }

    void SetCameraDistance()
    {
        //convert cube position to screen position
        Vector3 DefaultPos = transform.position;


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
        if(inputMapping.m_ControllerType == InputMapping.ControllerType.Mouse)
        {
            bool hasAdjusted = false;
            foreach(CameraRotationDefinition def in preDefinedRotations)
            {
                if(Input.GetKeyDown(def.key))
                {
                    SetPredefinedRotation(def);
                    hasAdjusted = true;
                    break;
                }
            }

            if(!hasAdjusted && Input.GetButton("Look Around"))
            {
                if(!hasAdjusted)
                {
                    Vector2 mousPos = new Vector2(Input.GetAxis("Mouse X"), 
                                                Input.GetAxis("Mouse Y"));
                    ApplyMouseRotation(mousPos);                    
                }
            } 
        }
        else
        {
            Vector2 joystickRot = new Vector2(Input.GetAxis(inputMapping.HorizontalCameraAxis),
                                              Input.GetAxis(inputMapping.VerticalCameraAxis));
            ApplyCameraRotation(joystickRot);       
        }

        
    }

    private void SetPredefinedRotation(CameraRotationDefinition rotationDefinition)
    {
        Quaternion QOldRotation = transform.rotation;
        Vector3 rotation = rotationDefinition.isAbsolute ? 
                            rotationDefinition.rotationEuler 
                            : QOldRotation.eulerAngles + rotationDefinition.rotationEuler;
        rotation.x = ClampRotation(rotation.x);
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void ApplyMouseRotation(Vector2 mousPos)
    {
        Quaternion QOldRotation = transform.rotation;
        Vector3 VOldRotation = QOldRotation.eulerAngles;
  Debug.Log("Sensi" + CameraSensitivity);
        Vector2 MouseMovementThisFrame = mousPos 
                                            * CameraSensitivity
                                            * 1000 
                                            * Time.deltaTime;
                                            
            if(invertCameraX.Value)
                VOldRotation.y += MouseMovementThisFrame.x;
            else
                VOldRotation.y -= MouseMovementThisFrame.x;

            if(!invertCameraY.Value)
                VOldRotation.x -= MouseMovementThisFrame.y;
            else
                VOldRotation.x += MouseMovementThisFrame.y;
            VOldRotation.z = 0;
            VOldRotation.x = ClampRotation(VOldRotation.x);
            QOldRotation = Quaternion.Euler(VOldRotation);
       
            transform.rotation = QOldRotation;

            //Debug.Log("MouseRot " + MouseMovementThisFrame);
    }
    
    private void SetCameraRotation()
    {

        float minRot = Mathf.Min(m_RotationMinClamp, m_RotationMaxClamp);
        float maxRot = Mathf.Max(m_RotationMinClamp, m_RotationMaxClamp);
        m_RotationMinClamp = minRot;
        m_RotationMaxClamp = maxRot;

        int len = m_CameraRotationSpeedCurve.keys.Length;
        if(len == 0)
        {
            m_firstRotationInst = 0;
            m_lastRotationInst = 0;
        }
        else
        {
            m_firstRotationInst = m_CameraRotationSpeedCurve.keys[0].time;
            m_lastRotationInst = m_CameraRotationSpeedCurve.keys[len-1].time;
        }
        m_timeInRotation = m_firstRotationInst;
    }

    private float ClampRotation(float angle)
    {
        if(Mathf.Sign(m_RotationMinClamp) == Mathf.Sign(m_RotationMaxClamp))
        {
            return Mathf.Clamp(angle, m_RotationMinClamp, m_RotationMaxClamp);
        } 
        else if(m_RotationMinClamp < 0 && m_RotationMaxClamp >= 0)
        {
            if(angle > 180)
            {
                angle -= 360;
            }

            return Mathf.Clamp(angle, m_RotationMinClamp, m_RotationMaxClamp);
        }
        else
        {
            Debug.LogError("Should not clamp rotation with max smaller than min");
        }
        return angle;
    }

    private void ApplyCameraRotation(Vector2 axisInput)
    {
        if(axisInput.magnitude <= 0)
        {
            
            m_timeInRotation = m_firstRotationInst;
            return;
        }

        m_timeInRotation = Mathf.Clamp(m_timeInRotation + Time.deltaTime, m_firstRotationInst, m_lastRotationInst);

        float rotSpeed = m_CameraRotationSpeedCurve.Evaluate(m_timeInRotation) * m_CameraRotationMaxSpeed;

        Vector2 rotDir = (Vector2.right * axisInput.x + Vector2.up * axisInput.y).normalized;
        rotDir.x = invertCameraY.Value? -rotDir.x : rotDir.x;
        rotDir.y = invertCameraX.Value? -rotDir.y : rotDir.y;

        Vector3 eulerRotation = transform.rotation.eulerAngles 
                                + (Vector3)rotDir * rotSpeed * Time.deltaTime;

        eulerRotation.x = ClampRotation (eulerRotation.x);
        transform.rotation = Quaternion.Euler(eulerRotation);
    } 
}

