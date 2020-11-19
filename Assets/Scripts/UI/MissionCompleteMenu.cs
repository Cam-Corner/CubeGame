using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MissionCompleteMenu : MonoBehaviour
{
    [SerializeField]
    private string exitScene;
    
    [SerializeField]
    private string replayScene;

    public void Exit()
    {
        SceneManager.LoadScene(exitScene, LoadSceneMode.Single);
    }

    public void Replay()
    {
        SceneManager.LoadScene(replayScene, LoadSceneMode.Single);
    }
}
