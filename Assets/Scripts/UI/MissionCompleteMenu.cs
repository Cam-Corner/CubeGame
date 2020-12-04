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

    [SerializeField]
    private GlobalMissionSettings m_MissionSettings;

    private void Start() {
        GameSoundBoard.Instance.PlayLevelComplete();
    }
    public void Exit()
    {
        SceneManager.LoadScene(exitScene, LoadSceneMode.Single);
    }

    public void Replay()
    {
        SceneManager.LoadScene(replayScene, LoadSceneMode.Single);
        m_MissionSettings.GameStart(m_MissionSettings.GetMissionType());
    }
}
