using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AmoaebaUtils;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField]
    private SoundSystem soundSystem;

    [SerializeField]
    private Slider slider;
    void Start()
    {
        slider.value = soundSystem.MainVolume;
    }

    
    public void OnSliderChanged()

    {
        soundSystem.MainVolume = slider.value;
    }
}
