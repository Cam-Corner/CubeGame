using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class CubeMovement : MonoBehaviour
{

    ////////////////////
    //Public Variables

    [Tooltip("Max force that can be applied to the cube")]
    public float m_MaxForce = 20;

    [Range(0,1), Tooltip("How much displacement needs to happen for the force to be considered")]
    public float m_ForceDeadZonePercent = 0.1f;


    //////////////////////////////
    //Private Variables
    [Tooltip("Unit Vector for the direction the force will be applied")]
    private Vector3 m_ForceToApply = new Vector3(0, 0, 0);

    private Vector3 m_DisplayForce = new Vector3(0, 0, 0);

    [Tooltip("should the force be applied this frame")]
    private bool m_bApplyForce = false;

    
    [SerializeField]
    private float m_MaxForceMouseDistance = 10.0f; 

    //////////////////////////////
    //Private components
    [Tooltip("The RigidBody component attached to this object")]
    private Rigidbody m_RB;

    [Tooltip("The UI Image that shows the current force amount"), SerializeField]
    private ForceArrow ForceArrow;

    [SerializeField]
    private CameraFollowScript gameCameraParent;

    [SerializeField]
    private InputMapping  inputMap;

<<<<<<< HEAD
    private Transform m_PlayerStart;

    private bool IsAiming => (m_RB.velocity == Vector3.zero);
=======
    private bool isMoving = false;
>>>>>>> f14d8d6b836862c902e005e43ec9cfcf8f38f5a1
 
    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        m_RB = GetComponent<Rigidbody>();
    }

    //called when the game object is spawned
    void Awake()
    {
        m_PlayerStart = GameObject.FindGameObjectWithTag("Start").transform;
        transform.position = m_PlayerStart.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMoving)
        {
            CalculateForceToBeApplied();
            UpdateForceArrow();
        }
        else
        {
            isMoving = !Mathf.Approximately(m_RB.velocity.magnitude,0);
        }

        
        if(transform.position.y <= -20.0f)
        {
            transform.position = m_PlayerStart.position;
            m_RB.velocity = new Vector3(0, 0, 0);
            Debug.Log("Cube Died: Fell of the map!");
        }
    }

    //Fixed update is called the same time as the physics engine update
    void FixedUpdate()
    {
        if(m_bApplyForce)
        {
            AddForceToObject();
            ForceArrow.gameObject.SetActive(false);
            isMoving = true;
        }
    }


    //add the force thats been calculated
    void AddForceToObject()
    {
        //Y Is the up vector in unity so we will use the Y as the Z Force
        Vector3 ForceDirection = new Vector3(m_ForceToApply.x, 0, m_ForceToApply.y);
        m_RB.AddForce(m_ForceToApply * m_MaxForce);

        m_bApplyForce = false;
    }

    //Calculates how much force should be applied to the object when the button is being held    
    private void CalculateForceToBeApplied()
    {
        switch(inputMap.m_ControllerType)
        {
            case InputMapping.ControllerType.Mouse:
                CalculateMouseForce();
                break;
            default:
                CalculateJoystickForce();
                break;
        }
        
        if(inputMap.GetRunningButton() 
           && !isMoving
           && m_DisplayForce.magnitude > m_ForceDeadZonePercent)
        {
            m_ForceToApply = m_DisplayForce;
            m_bApplyForce = true;
            return;
        }
    }

    private void CalculateMouseForce()
    {
        Vector2 mousPos = new Vector2(Input.GetAxis("Mouse X"), 
                                        Input.GetAxis("Mouse Y"));
        Vector2 currentScreenPos = gameCameraParent.GameCamera.WorldToScreenPoint(transform.position);
     
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        
        Ray mouseRay = gameCameraParent.GameCamera.ScreenPointToRay(Input.mousePosition);
        float dist = 0;
        if(playerPlane.Raycast(mouseRay, out dist))
        {
            Vector3 mouseWorldProjection =  (mouseRay.origin + mouseRay.direction * dist);
            float magnitude = Mathf.Clamp01((mouseWorldProjection - transform.position).magnitude / m_MaxForceMouseDistance);

            m_DisplayForce = (mouseWorldProjection - transform.position).normalized * magnitude;
        } 
        else
        {
            m_DisplayForce = Vector3.zero;
        }
    }

    private void CalculateJoystickForce()
    {
        Vector3 camRotation = gameCameraParent.transform.rotation.eulerAngles;
        camRotation = Vector3.Scale(camRotation, Vector3.up);
        m_DisplayForce = Quaternion.Euler(camRotation) * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }


    //convert the direction vector to a quaternion
    void UpdateForceArrow()
    {
        if (ForceArrow != null)
        {
            if(m_DisplayForce.magnitude < m_ForceDeadZonePercent)
            {
                ForceArrow.gameObject.SetActive(false);
            }
            else
            {
                ForceArrow.gameObject.SetActive(true);
                ForceArrow.UpdateArrow(transform.position, m_DisplayForce, m_DisplayForce.magnitude);
            }
        }
    }

    public void PlayerFound()
    {
        transform.position = m_PlayerStart.position;
        m_RB.velocity = new Vector3(0, 0, 0);
    }
}
