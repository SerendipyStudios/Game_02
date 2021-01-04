using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Game;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCameraSpectatorUI : MonoBehaviour
{
    private PlayerController playerController;
    
    //Interface elements
    [SerializeField] private Button changeBackwards;
    [SerializeField] private Button changeForwards;

    private void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    public void Initialize(PlayerController _playerController)
    {
        playerController = _playerController;
        
        changeBackwards.onClick.AddListener(() => playerController.ChangeCameraSpectatorPlayer(false));
        changeForwards.onClick.AddListener(() => playerController.ChangeCameraSpectatorPlayer(true));
    }
}
