using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoad;

    [SerializeField]
    private BoolVar m_IsLootGame;

    [SerializeField]
    private GlobalMissionSettings m_MissionSettings;
    
    private const string tutorialKey = "tutorial";
    private bool hasCheckedTutorial = false;

#if UNITY_EDITOR
    [SerializeField]
    private bool clearTutorialOnStart = false;
#endif

    private void Start() {
        
#if UNITY_EDITOR
        if(clearTutorialOnStart)
        {
            ClearTutorial();
        }
#endif
    }
    private void Update() 
    {
        if(hasCheckedTutorial)
        {
            return;
        }

        if(!PlayerPrefs.HasKey(tutorialKey) && !MenuHelper.Instance.InTutorial)
        {
            MenuHelper.Instance.OpenTutorial();
            PlayerPrefs.SetInt(tutorialKey, 1);                
        }

        hasCheckedTutorial = true;
    }

    [AddComponentMenu("RobberDuck/Clear Tutorial")]
    public static void ClearTutorial()
    {
        if(PlayerPrefs.HasKey(tutorialKey))
        {
            PlayerPrefs.DeleteKey(tutorialKey);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);

        m_MissionSettings.GameStart(m_IsLootGame.Value ?eMissionType.EMT_Loot : eMissionType.EMT_Destructable);
    }

}
