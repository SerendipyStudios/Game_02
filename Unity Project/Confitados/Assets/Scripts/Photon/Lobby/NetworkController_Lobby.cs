using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController_Lobby : MonoBehaviourPunCallbacks
{
    #region Variables
    
    [SerializeField] private Text log;
    
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField] private byte maxPlayersInRoom = 4;
    [SerializeField] private byte minPlayersInRoom = 2;
    [SerializeField] private int currentPlayersInRoom = 0;
    [SerializeField] private Text playerCount;

    #endregion
    
    #region UnityCallbacks

    private void FixedUpdate()
    {
        //Check players and update UI
        if (PhotonNetwork.CurrentRoom != null)
        {
            currentPlayersInRoom = PhotonNetwork.CurrentRoom.PlayerCount;
            playerCount.text = currentPlayersInRoom + "/" + maxPlayersInRoom;

            foreach (var player in PhotonNetwork.CurrentRoom.Players)
            {
                playerCount.text += "\nPlayer " + player.Key + ": " + player.Value.NickName;
            }
        }
        else
        {
            playerCount.text = currentPlayersInRoom + "/" + maxPlayersInRoom;
        }
    }
    
    #endregion

    #region Photon Callbacks

    public override void OnDisconnected(DisconnectCause cause)
    {
        //base.OnDisconnected(cause);

        log.text += "\nDisconnected.";

        ScreenManager.GoToScreen("Screen_01_0_MainMenu");
    }

    #endregion

    #region Methods

    public void StartGame()
    {
        //When multiple levels are made, select between them here
        PhotonNetwork.LoadLevel("Screen_02_2_game");
    }
    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(true);
        ScreenManager.GoToScreen("Screen_02_0_anteroom");
    }
    
    #endregion
}
