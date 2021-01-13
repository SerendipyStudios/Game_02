using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    
    private void Start()
    {
        musicSlider.value = SoundManager.sharedInstance.GetMusicVolume();
        sfxSlider.value = SoundManager.sharedInstance.GetSfxVolume();
    }

    public void SetMusicVolume(float value)
    {
        SoundManager.sharedInstance.SetVolumeMusic(value);
    }

    public void SetSfxVolume(float value)
    {
        SoundManager.sharedInstance.SetVolumeSfx(value);
    }

    public void MuteMusic()
    {
        musicSlider.value = 0;
        SetMusicVolume(0);
    }

    public void MuteSfx()
    {
        sfxSlider.value = 0;
        SetSfxVolume(0);
    }
}
