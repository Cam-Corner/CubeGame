using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class CubeMovement : MonoBehaviour
{
    public enum ECubeForceDirection
    {
        Mouse,
        Camera
    }

    ////////////////////
    //Public Variables
    [Header("Force Settings")]
    [Tooltip("What Determends the direction the cube will go")]
    public ECubeForceDirection m_ForceDirectionStyle = ECubeForceDirection.Mouse;

    [Tooltip("Max force that can be applied to the cube")]
    public float m_MaxForce = 20;

    [Tooltip("How fast should the force increase when holding down the button")]
    public float m_ForceIncreaseSpeed = 1;


    //////////////////////////////
    //Private Variables
    [Tooltip("Unit Vector for the direction the force will be applied")]
    private Vector3 m_ForceDirection = new Vector3(0, 0, 0);

    [Tooltip("Amount of force that will be applied to the cube")]
    private float m_AmountOfForce = 0;

    [Tooltip("Should the amount of force increase or decrease")]
    private bool m_bIncreaseForce = true;

    [Tooltip("should the force be applied this frame")]
    private bool m_bApplyForce = false;

    //////////////////////////////
    //Private components
    [Tooltip("The RigidBody component attached to this object")]
    private Rigidbody m_RB;

    [Tooltip("The UI Image that shows the current force amount")]
    private Image Image_ForceApplied;

    [Tooltip("The UI Image that shows the current force amount")]
    private GameObject GO_ForceArrow;

    // Start is called before the first frame update
    void Start()
    {
        m_RB = GetComponent<Rigidbody>();

        Image_ForceApplied = GameObject.Find("CurrentForce_Amount").GetComponent<Image>();
        GO_ForceArrow = GameObject.Find("ForceDirectionArrow");

        //Cursor.lockState = CursorLockMode.Locked;
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
        WorkOutDirection();
        DirectionVectorToQuaternion();

        
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
        Vector3 ForceDirection = new Vector3(m_ForceDirection.x, 0, m_ForceDirection.y);
        m_RB.AddForce(ForceDirection * (m_MaxForce * m_AmountOfForce));

        m_AmountOfForce = 0;
        m_bIncreaseForce = true;
        m_bApplyForce = false;

        if(Image_ForceApplied != null)
            Image_ForceApplied.fillAmount = m_AmountOfForce / 100;
    }

    //Calculates how much force should be applied to the object when the button is being held    
    void CalculateForceToBeApplied()
    {
        if(Input.GetMouseButton(0) && (m_RB.velocity == new Vector3(0, 0, 0)))
        {
            if(m_bIncreaseForce)
            {
                m_AmountOfForce += m_ForceIncreaseSpeed * Time.deltaTime;

                if(m_AmountOfForce >= 100)
                {
                    m_AmountOfForce = 100;
                    m_bIncreaseForce = false;
                }
            }
            else
            {
                m_AmountOfForce -= m_ForceIncreaseSpeed * Time.deltaTime;

                if (m_AmountOfForce <= 1)
                {
                    m_AmountOfForce = 1;
                    m_bIncreaseForce = true;
                }
            }

            if (Image_ForceApplied != null)
                Image_ForceApplied.fillAmount = m_AmountOfForce / 100;
        }
        else if(m_AmountOfForce > 0)
        {
            m_bApplyForce = true;
        }
    }

    //works out the direction the cube will go
    void WorkOutDirection()
    {
        if(m_ForceDirectionStyle == ECubeForceDirection.Mouse)
        {
            //convert cube position to screen position
            Vector3 CubeScreenPos = Camera.allCameras[0].WorldToScreenPoint(transform.position);

            //work out direction
            float Distance = Vector3.Distance(Input.mousePosition, CubeScreenPos);

            Vector3 UnitV = Input.mousePosition;
            UnitV.x -= CubeScreenPos.x;
            UnitV.y -= CubeScreenPos.y;
            UnitV.z -= CubeScreenPos.z;

            UnitV = UnitV / Distance;
            m_ForceDirection = UnitV;    
        }
        else if(m_ForceDirectionStyle == ECubeForceDirection.Camera)
        {
            
        }
    }

    //convert the direction vector to a quaternion
    void DirectionVectorToQuaternion()
    {
        if (GO_ForceArrow != null)
        {
            //set arrow location to the cube
            GO_ForceArrow.transform.position = transform.position;
            float ArrowDistance = 1.5f;//distance the arrow will come away from the cube
            
            //rotate the arrow so it faces the forward direction
            //Y Is the up vector in unity so we will use the Y as the Z Force
            Vector3 ForceDirection = new Vector3(m_ForceDirection.x, 0, m_ForceDirection.y);
            Quaternion DirectionRotation = Quaternion.LookRotation(ForceDirection, new Vector3(0, 1, 0));

            //apply rotatation
            GO_ForceArrow.transform.rotation = DirectionRotation;

            //move arrow out of cube
            GO_ForceArrow.transform.position += ForceDirection * ArrowDistance;

            //transform.rotation = DirectionRotation;


            /*
            //test code          
            GameObject UIObject = GameObject.Find("UI_CurrentForce");
            Vector3 CubeScreenPos = Camera.allCameras[0].WorldToScreenPoint(transform.position);
            UIObject.transform.position = CubeScreenPos;
            Vector2 ForceDirection2D = new Vector2(m_ForceDirection.x, m_ForceDirection.y);

            float Angle = Vector2.Angle(ForceDirection2D, new Vector2(0, 1));
            Vector3 CrossAngle = Vector3.Cross(ForceDirection2D, new Vector2(0, 1));

            if(CrossAngle.z > 0)
                Angle = 360 - Angle;

            Quaternion NewRotatation = Quaternion.Euler(0, 0, Angle);
            UIObject.transform.rotation = NewRotatation;
            */
        }
    }
}
