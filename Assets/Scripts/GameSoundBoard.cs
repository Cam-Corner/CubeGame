using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using System;

public class GameSoundBoard : SingletonScriptableObject<GameSoundBoard>
{

    [SerializeField]
    private SoundSystem soundSystem;

    [SerializeField]
    private AudioClip caughtRestart;
    
    [SerializeField]
    private AudioClip run;
    
    [SerializeField]
    private AudioClip levelComplete;

    [SerializeField]
    private AudioClip steal;

    [SerializeField]
    private AudioClip destructionSound;

    [SerializeField]
    private AudioClip laser;

    private bool IsRunning = false;

    public void PlayRestart()
    {
        PlaySound(caughtRestart);
    }

    public void PlayLaser()
    {
        PlaySound(laser);
    }

    public void PlayRun()
    {
        if(IsRunning)
        {
            return;
        }
        IsRunning = true;
        PlaySound(run, "RunSound", (string identifier) => 
        { 
            if(IsRunning) 
            {
                PlayRun(); 
            } 
        });
    }

    public void StopRun()
    {
        IsRunning = false;
        soundSystem.StopSound("RunSound");
    }

    public void PlayLevelComplete()
    {
        PlaySound(levelComplete);
    }

    public void PlayStealSound()
    {
        PlaySound(steal);
    }

    public void PlayDestructionSound()
    {
        PlaySound(destructionSound);
    }

    private void PlaySound(AudioClip sound, string identifier = "", Action<string> callback = null)
    {
        soundSystem.PlaySound(sound, identifier, true, callback);
    }
}
