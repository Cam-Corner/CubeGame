using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class MenuHelper : SingletonScriptableObject<MenuHelper>
{
    [SerializeField]
    private Transform SettingsPrefab;

    public Transform openSettings;
    public bool InSettings => openSettings != null;

    [SerializeField]
    private Transform TutorialPrefab;
    private Transform tutorialPrefab;
    public bool InTutorial => tutorialPrefab != null;

    public void OpenSettings()
    {
        Instantiate(SettingsPrefab);
    }

    public void AlternateSettings()
    {
        if(openSettings != null)
        {
            
            openSettings.GetComponent<SettingsScreen>().CloseMenu();
        }
        else
        {
            OpenSettings();
        }
    }

    public void OpenTutorial()
    {
        Instantiate(TutorialPrefab);
    }

    public void AlternateTutorial()
    {
        if(tutorialPrefab != null)
        {
            Destroy(tutorialPrefab.gameObject);
        }
        else
        {
            OpenTutorial();
        }
    }
}
