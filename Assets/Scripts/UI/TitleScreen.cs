using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField]
    private BoolVar isLootGameVar;

    [SerializeField]
    private string sceneToLoad;

    [SerializeField]
    private GlobalMissionSettings m_MissionSettings;

    public void StartLootGame()
    {
        StartGame(true);
    }

    public void StartDestructionGame()
    {
        StartGame(false);
    }

    private void StartGame(bool isLootGame)
    {
        isLootGameVar.Value = isLootGame;

        m_MissionSettings.GameStart(isLootGame ? eMissionType.EMT_Destructable : eMissionType.EMT_Loot);

        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);

    }
}
