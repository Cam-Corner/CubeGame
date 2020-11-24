using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoad;

    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }

}
