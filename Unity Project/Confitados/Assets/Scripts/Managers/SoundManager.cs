using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //Singleton
    public static SoundManager sharedInstance { get; private set; }

    //References
    public PlayerController player;

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
}
