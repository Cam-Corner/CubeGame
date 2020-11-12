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
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }
}
