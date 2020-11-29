using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using AmoaebaUtils;

public enum eFoundPlayerType
{
    EFPT_HumanEnemy = 0,
    EFPT_CCTV_Camera = 1,
    EFPT_Laser = 2
}

public class CubeMovement : MonoBehaviour
{

    ////////////////////
    //Public Variables

    [Tooltip("Max force that can be applied to the cube")]
    public float m_MinForce = 20;
    public float m_MaxForce = 20;

    [Range(0,1), Tooltip("How much displacement needs to happen for the force to be considered")]
    public float m_ForceDeadZonePercent = 0.1f;


    //////////////////////////////
    //Private Variables
    [Tooltip("Unit Vector for the direction the force will be applied")]
    private Vector3 m_ForceToApply = new Vector3(0, 0, 0);

    private Vector3 m_DisplayForce = new Vector3(0, 0, 0);

    private bool IsElapsingTime = false;

    [Tooltip("should the force be applied this frame")]
    private bool m_bApplyForce = false;

    [SerializeField]
    private float m_StopMagnitude = 0.5f;

    [SerializeField]
    private GlobalMissionSettings m_MissionSettings;
    
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

    [SerializeField]
    private Transform m_PlayerStart;

    [SerializeField]
    private MenuHelper menuHelper;

    [SerializeField]
    private MissionObjectiveVar objectBeingStolen;

    private Animator moveAnimator;
    private ParticleSystem sweatParticles;

    private IEnumerator moveStartRoutine = null;

    private TurnBasedSystem turnBasedSystem => TurnBasedSystem.Instance;
    private BoolVar isTurnBasedGame => turnBasedSystem.IsTurnBasedGameVar;
    private BoolVar isTimeActive => turnBasedSystem.IsTimeActiveVar;

    private bool isMoving = false;
    private bool isUp = false;

    private float storedSpeed = 1.0f;
    public bool IsUp 
    {
        get{ return isUp; }
        set
        {
            isUp = value;
            
            isTimeActive.Value = isUp 
                                 || !turnBasedSystem.IsTurnBasedGame 
                                 || turnBasedSystem.IsWaitingForPhysicsEntities
                                 || IsElapsingTime;

            moveAnimator.SetBool("IsUp", value);  
        }
    }

    private bool ShouldShowArrow => !IsUp;

    public bool IsMoving
    {
        get { return isMoving; }
        set 
        { 
            isMoving = value;
            moveAnimator.SetBool("IsMoving", value);

            if(isMoving)
            {
                sweatParticles.Play();
            }
            else
            {
                sweatParticles.Stop();
            }
        }
    }
 
    // Start is called before the first frame update
    void Start()
    {
        isTimeActive.OnChange += OnTimeActiveChange;
        IsMoving = false;
        m_RB = GetComponent<Rigidbody>();

        turnBasedSystem.OnWaitForPhysicsEntitiesChangedEvent += OnEntitiesWaitChange;
    }

    //called when the game object is spawned
    void Awake()
    {
        m_PlayerStart = GameObject.FindGameObjectWithTag("Start").transform;
        transform.position = m_PlayerStart.position;
        moveAnimator = GetComponentInChildren<Animator>();
        sweatParticles = GetComponentInChildren<ParticleSystem>();
    }

    private void OnTimeActiveChange(bool oldVal, bool newVal)
    {
        if(newVal == oldVal)
        {
            return;
        }
        Debug.Log("SPEED" + moveAnimator.speed);
        if(!newVal)
        {
            storedSpeed = moveAnimator.speed;
        }
        moveAnimator.speed = newVal? storedSpeed : 0.0f;
    }

    private void OnDestroy() 
    {
        isTimeActive.OnChange -= OnTimeActiveChange;
        turnBasedSystem.OnWaitForPhysicsEntitiesChangedEvent -= OnEntitiesWaitChange;    
    }

    private void OnEntitiesWaitChange(bool isWaiting)
    {
        if(!turnBasedSystem.IsTurnBasedGame || !turnBasedSystem.WaitForPhysicsEntities)
        {
            return;
        }

        if(!isUp)
        {
            isTimeActive.Value = isWaiting && !IsElapsingTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_MissionSettings.GetMissionState() != eMissionState.EMS_PlayingMission)
            return;
            

        if(inputMap.GetSettingsButton())
        {
            menuHelper.AlternateSettings();
        }

        if(menuHelper.InSettings)
        {
            return;
        }
        
        if(ShouldShowArrow)
        {
            CalculateForceToBeApplied();
            UpdateForceArrow();
        }
        else if(IsMoving && IsUp)
        {
            IsUp = IsMoving = m_RB.velocity.magnitude > m_StopMagnitude;
            if(!isUp)
            {
                GameSoundBoard.Instance.StopRun();
                m_RB.velocity = new Vector3(0,0,0);
            }
        }
        
        if(transform.position.y <= -20.0f)
        {
            transform.position = m_PlayerStart.position;
            m_RB.velocity = new Vector3(0, 0, 0);
            Debug.Log("Cube Died: Fell of the map!");
        }

        if(isUp)
        {
            moveAnimator.SetBool("IsStealing", false);
        }
        else
        {
            moveAnimator.SetBool("IsStealing", objectBeingStolen.Value != null && objectBeingStolen.Value.IsBeingStolen);
        }
    }

    //Fixed update is called the same time as the physics engine update
    void FixedUpdate()
    {
        if(m_bApplyForce && !IsUp)
        {
            Vector3 forceDirection = new Vector3(m_ForceToApply.x, 0, m_ForceToApply.y);
            ForceArrow.gameObject.SetActive(false);
            
            if(moveStartRoutine != null)
            {
                StopCoroutine(moveStartRoutine);
            }
            IsUp = true;
            moveStartRoutine = MoveStartRoutine(forceDirection);
            StartCoroutine(moveStartRoutine);
            GameSoundBoard.Instance.PlayRun();
        }
        else if(!IsUp && IsElapsingTime)
        {
            isTimeActive.Value = true; 
        }
    }

    private IEnumerator MoveStartRoutine(Vector3 forceDirection)
    {
        float startRunDelay = 0.3f;
        float runMinimumTime = 0.1f;
        yield return new WaitForSeconds(startRunDelay);
        AddForceToObject(forceDirection); 

        yield return new WaitForSeconds(runMinimumTime);      
        IsMoving = true;
        moveStartRoutine = null;
    }


    //add the force thats been calculated
    void AddForceToObject(Vector3 forceDirection)
    {
        transform.forward = m_ForceToApply;
        m_RB.AddForce(m_ForceToApply * (m_MaxForce-m_MinForce) + m_MinForce*m_ForceToApply.normalized);

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
        
        IsElapsingTime = false;
        if(inputMap.GetRunningButton() && !IsMoving)
        {
            if(m_DisplayForce.magnitude > m_ForceDeadZonePercent)
            {
                m_ForceToApply = m_DisplayForce;
                m_bApplyForce = true;
                IsElapsingTime = false;
            }
            else if(isTurnBasedGame.Value)
            {
                IsElapsingTime = true;
            }
        }
        else if(inputMap.GetElapseTimeButton() && !IsMoving)
        {
            IsElapsingTime = true;
        }

        if(isTurnBasedGame.Value && !IsElapsingTime && !IsMoving)
        {
            isTimeActive.Value = false;
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
                ForceArrow.UpdateArrow(transform.position, m_DisplayForce, (m_DisplayForce.magnitude-m_ForceDeadZonePercent)/(1.0f-m_ForceDeadZonePercent));
            }
        }
    }

    public void PlayerFound(eFoundPlayerType FoundType)
    {
        //switch(FoundType)
        //{
        //    case eFoundPlayerType.EFPT_HumanEnemy:
        //        if (m_RB.velocity != new Vector3(0, 0, 0)) ResetPlayer();
        //        break;
        //    case eFoundPlayerType.EFPT_CCTV_Camera:
        //        ResetPlayer();
        //        break;
        //}

        Debug.Log("Player Found: " + FoundType);
        if(FoundType == eFoundPlayerType.EFPT_Laser)
        {
            GameSoundBoard.Instance.PlayLaser();
        }
        else
        {
            GameSoundBoard.Instance.PlayRestart();
        }
        
        ResetPlayer();
    }

    public bool PlayerHidden()
    {
        if (m_RB.velocity == new Vector3(0, 0, 0))
        {
            return true;
        }

        return false;
    }
    

    private void ResetPlayer()
    {
        transform.position = m_PlayerStart.position;
        m_RB.velocity = new Vector3(0, 0, 0);
    }
}
