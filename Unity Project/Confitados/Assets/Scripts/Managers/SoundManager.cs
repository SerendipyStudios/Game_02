using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    //Singleton
    public static SoundManager sharedInstance { get; private set; }

    //References
    public PlayerController player;
    private float musicVolumeVariable = 0.5f;
    private float sfxVolumeVariable = 0.5f;

    //Themes
    [Header("Themes")]
    public AudioSource inGameMusic_MUSIC;
    public AudioSource menuMusic_MUSIC;

    //Sounds
    [Header("SFX")]
    public AudioSource basicButton_SND;
    public AudioSource breakWalls_SND;
    public AudioSource cakeStep_SND;
    public AudioSource caramelStep_SND;
    public AudioSource collisionConfites_SND;
    public AudioSource dash_SND;
    public AudioSource endCooldown_SND;
    public AudioSource endGame_SND;
    public AudioSource fallDown_SND;
    public AudioSource iceStep_SND;
    public AudioSource levelUpSuperDash_SND;
    public AudioSource notReadyButton_SND;
    public AudioSource readyButton_SND;
    public AudioSource scoreMusic_SND;
    public AudioSource semaphore_SND;
    public AudioSource startButton_SND;
    public AudioSource superDash_SND;

    //Events
    [HideInInspector]
    public event Action OnBreakWalls; //Observer pattern
    [HideInInspector]
    public event Action OnPlayerSteps;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (sharedInstance == null)
            sharedInstance = this;
       // this.transform.SetParent(GameObject.Find("--Managers--").transform, false);
       
       SetVolumeMusic(musicVolumeVariable);
       SetVolumeSfx(sfxVolumeVariable);
    }
    

    public void Initialize(PlayerController _playerController)
    {
        this.player = _playerController;
    }

    private void Update()
    {
        OnBreakWalls?.Invoke();
        OnPlayerSteps?.Invoke();
    }

    public void PlayBasicButton()
    {
        basicButton_SND.Play();
    }

    public void PlayReadyButton()
    {
        readyButton_SND.Play();
    }

    public void PlayNotReadyButton()
    {
        notReadyButton_SND.Play();
    }

    public void PlayStepSound()
    {
        AudioSource step;
        if (player.rb.drag == player.iceDrag)
            step = iceStep_SND;
        else if (player.rb.drag == player.stickyDrag)
            step = caramelStep_SND;
        else
            step = cakeStep_SND;
        step.PlayOneShot(step.clip);
        OnPlayerSteps -= PlayStepSound;
    }

    public void SetVolumeMusic(float value)
    {
        Debug.Log("Volume " + value);
        musicVolumeVariable = value;
        
        //Set volume values
        inGameMusic_MUSIC.volume = value;
        menuMusic_MUSIC.volume = value;
    }
    
    public void SetVolumeSfx(float value)
    {
        sfxVolumeVariable = value;
        
        //Set volume values
        basicButton_SND.volume = value;
        breakWalls_SND.volume = value;
        cakeStep_SND.volume = value;
        caramelStep_SND.volume = value;
        collisionConfites_SND.volume = value;
        dash_SND.volume = value;
        endCooldown_SND.volume = value;
        endGame_SND.volume = value;
        fallDown_SND.volume = value;
        iceStep_SND.volume = value;
        levelUpSuperDash_SND.volume = value;
        notReadyButton_SND.volume = value;
        readyButton_SND.volume = value;
        scoreMusic_SND.volume = value;
        semaphore_SND.volume = value;
        startButton_SND.volume = value;
        superDash_SND.volume = value;
    }

    public float GetMusicVolume()
    {
        return musicVolumeVariable;
    }

    public float GetSfxVolume()
    {
        return sfxVolumeVariable;
    }
}
