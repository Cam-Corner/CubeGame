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

    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);

        m_MissionSettings.GameStart(m_IsLootGame.Value ?eMissionType.EMT_Loot : eMissionType.EMT_Destructable);
    }

}
