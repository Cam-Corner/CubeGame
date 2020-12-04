using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;



public class MissionManager : MonoBehaviour
{
    /*Make Singleton*/
    public static MissionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("References")]
    [SerializeField] private CameraFollowScript m_MainCamera;
    [SerializeField] private CubeMovement m_Player;
    [SerializeField] private GlobalMissionSettings m_MissionSettings;
    [SerializeField] private TurnBasedSystem m_TurnBasedSystem;

    [Header("Mission Settings")]
    [SerializeField] private List<MissionObjective> m_MissionObjects = new List<MissionObjective>();
    [SerializeField] private eMissionState m_StartingMissionState = eMissionState.EMS_MissionBrief;

    public bool IsInMissionBrief => m_MissionSettings.GetMissionState() == eMissionState.EMS_MissionBrief;
    private int objectsToStealCount;
    private int objectsStolenCount;
    public int ObjectsToSteal => objectsToStealCount;
    public int ObjectsStolen => objectsStolenCount;
  
    private int m_CameraPanCurrentObjectIndex = 0;
    private float m_CurrentCameraRotationValue = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (m_MissionSettings.GetMissionType() == eMissionType.EMT_Loot)
        {
            if (m_MissionObjects.Count <= 0 && m_StartingMissionState == eMissionState.EMS_MissionBrief)
                m_MissionSettings.SetMissionState(eMissionState.EMS_PlayingMission);
            else
                m_MissionSettings.SetMissionState(m_StartingMissionState);
        }
        else if (m_MissionSettings.GetMissionType() == eMissionType.EMT_Destructable)
        {
            m_MissionSettings.SetMissionState(eMissionState.EMS_PlayingMission);
        }

        m_MissionSettings.StartingAmountOfMissionObjectives((uint)m_MissionObjects.Count);

        //Debug.Log("Game Started! Gamemode = " + m_MissionSettings.GetMissionType());

        m_CameraPanCurrentObjectIndex = 0;

        objectsToStealCount = m_MissionObjects.Count;
        objectsStolenCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_MissionSettings.GetMissionState())
        {
            default:
                break;
            case eMissionState.EMS_MissionBrief:
                MissionBriefCameraPan();
                break;
            case eMissionState.EMS_PlayingMission:
                PlayingMission();
                break;
        }

        //Debug.Log(m_MissionSettings.GetMissionState() +" : " + m_MissionObjects.Count);
        UpdateTimer();
    }

    private void PlayingMission()
    {       
        if (m_MissionObjects.Count > 0)
        {
            //foreach(MissionObjective MO in m_MissionObjects)
            for(int i = 0; i < m_MissionObjects.Count; i++)
            {
                if (m_MissionObjects[i].ObjectStolen())
                {
                    CollectedItem(m_MissionObjects[i]);
                }
            }
        }
    }

    void UpdateTimer()
    {
        m_MissionSettings.UpdateMissionTimer();
    }

    private void MissionBriefCameraPan()
    {
        if (m_CameraPanCurrentObjectIndex > m_MissionObjects.Count)
        {
            m_MissionSettings.SetMissionState(eMissionState.EMS_PlayingMission);
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            m_CameraPanCurrentObjectIndex = 0;
            m_MissionSettings.SetMissionState(eMissionState.EMS_PlayingMission);
            m_MainCamera.transform.position = m_Player.transform.position;
            return;
        }

        if (m_CameraPanCurrentObjectIndex == m_MissionObjects.Count)
        {
            Vector3 ObjectPos = m_Player.transform.position;
            Vector3 CamPos = m_MainCamera.transform.position;

            Vector3 Dir = (ObjectPos - CamPos).normalized;

            m_MainCamera.transform.position += (Dir * m_MissionSettings.GetCameraPanMoveSpeed()) * Time.deltaTime;

            if (Vector3.Distance(ObjectPos, CamPos) < 1)
            {
                m_CameraPanCurrentObjectIndex = 0;
                m_MissionSettings.SetMissionState(eMissionState.EMS_PlayingMission);
                return;
            }
        }
        else
        {
            Vector3 ObjectPos = m_MissionObjects[m_CameraPanCurrentObjectIndex].transform.position;
            Vector3 CamPos = m_MainCamera.transform.position;

            Vector3 Dir = (ObjectPos - CamPos).normalized;

            if (Vector3.Distance(ObjectPos, CamPos) < 1)
            {
                m_CurrentCameraRotationValue += m_MissionSettings.GetCameraPanRotationSpeed() * Time.deltaTime;

                Quaternion Rotation = Quaternion.Euler(0, m_MissionSettings.GetCameraPanRotationSpeed() * Time.deltaTime, 0);

                m_MainCamera.transform.rotation = m_MainCamera.transform.rotation * Rotation;

                if (m_CurrentCameraRotationValue >= 360)
                {
                    m_CurrentCameraRotationValue = 0;
                    m_CameraPanCurrentObjectIndex++;
                }

            }
            else
                m_MainCamera.transform.position += (Dir * m_MissionSettings.GetCameraPanMoveSpeed()) * Time.deltaTime;
        }

    }

    private void CollectedItem(MissionObjective ItemCollected)
    {
        GameObject ThisObjective = ItemCollected.gameObject;
        m_MissionObjects.Remove(ItemCollected);
        Debug.Log(ThisObjective.name + " has been stolen!");
        m_MissionSettings.CollectedAMissionObjective();
        objectsStolenCount = Mathf.Clamp(objectsStolenCount +1, 0, objectsToStealCount);
        Destroy(ThisObjective);
    }

    private void ResetLevelStart()
    {

    }

    public bool MissionComplete()
    {
        if (m_MissionObjects.Count == 0)
        {
            return true;
        }

        return false;
    }

    //public eMissionState GetMissionState() => m_MissionSettings.GetMissionState();

    
}
