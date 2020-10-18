using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class CubeMovement : MonoBehaviour
{
    public enum ForceInput
    {
        Mouse,
        Joystick
    }

    ////////////////////
    //Public Variables
    [Header("Force Settings")]
    [Tooltip("What Determends the direction the cube will go")]
    public ForceInput m_ForceInput = ForceInput.Mouse;

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
    private Camera gameCamera;

    // Start is called before the first frame update
    void Start()
    {
        m_RB = GetComponent<Rigidbody>();
    }

    //called when the game object is spawned
    void Awake()
    {
         transform.position = GameObject.FindGameObjectWithTag("Start").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateForceToBeApplied();
        UpdateForceArrow();

        
        if(transform.position.y <= -20.0f)
        {
            transform.position = GameObject.FindGameObjectWithTag("Start").transform.position;
            Debug.Log("Cube Died: Fell of the map!");
        }
    }

    //Fixed update is called the same time as the physics engine update
    void FixedUpdate()
    {
        if(m_bApplyForce)
        {
            AddForceToObject();
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
        switch(m_ForceInput)
        {
            case ForceInput.Mouse:
                CalculateMouseForce();
                break;
            case ForceInput.Joystick:
                CalculateJoystickForce();
                break;
        }
    }

    private void CalculateMouseForce()
    {
        Vector2 mousPos = new Vector2(Input.GetAxis("Mouse X"), 
                                        Input.GetAxis("Mouse Y"));
        Vector2 currentScreenPos = gameCamera.WorldToScreenPoint(transform.position);
     
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        
        Ray mouseRay = gameCamera.ScreenPointToRay(Input.mousePosition);
        float dist = 0;
        if(playerPlane.Raycast(mouseRay, out dist))
        {
            Vector3 mouseWorldProjection =  (mouseRay.origin + mouseRay.direction * dist);
            Debug.Log((mouseWorldProjection - transform.position).magnitude);
            float magnitude = Mathf.Clamp01((mouseWorldProjection - transform.position).magnitude / m_MaxForceMouseDistance);

            m_DisplayForce = (mouseWorldProjection - transform.position).normalized * magnitude;
        } 
        else
        {
            m_DisplayForce = Vector3.zero;
        }


        if(Input.GetMouseButton(0) 
           && (m_RB.velocity == new Vector3(0, 0, 0))
           && m_DisplayForce.magnitude > m_ForceDeadZonePercent)
        {
            m_ForceToApply = m_DisplayForce;
            m_bApplyForce = true;
            return;
        }
    }

    private void CalculateJoystickForce()
    {

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
}
