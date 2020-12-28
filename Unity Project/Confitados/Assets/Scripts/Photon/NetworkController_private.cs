using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController_private : MonoBehaviourPunCallbacks
{
    /****************************
     *
     * Documentation: https://doc-api.photonengine.com/en-us/current/getting-started/pun-intro
     * Scripting API: https://doc-api.photonengine.com/en/pun/v2/index.html
     * 
     **********************/

    #region Variables

    [SerializeField] private Button connectButton;
    [SerializeField] private Button joinRandomButton;
    [SerializeField] private Text log;
    
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField] private byte maxPlayersInRoom = 4;
    [SerializeField] private byte minPlayersInRoom = 2;
    [SerializeField] private int currentPlayersInRoom = 0;
    [SerializeField] private Text playerCount;

    [HideInInspector]
    public int jeje = 0;
    
    #endregion
    
    #region UnityCallbacks

    private void Awake()
    {
        //this makes sure we can use PhotonNetwork.LoadLevel() on the master client and
        //    all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true; 
    }

    void Start()
    {
        //PhotonNetwork.ConnectUsingSettings(); //Connect to Photon servers

        //There are other ways to connect the game to server, which can be founded here:
        //     https://doc-api.photonengine.com/en/pun/v2/class_photon_1_1_pun_1_1_photon_network.html
    }

    private void FixedUpdate()
    {
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

    #region Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Ahora estamos conectados al servidor de la región: " + PhotonNetwork.CloudRegion);

        //Enable the join room button
        connectButton.interactable = false;
        joinRandomButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();

        log.text += "\nJoined to room " + PhotonNetwork.CurrentRoom.Name + ".";
        
        //Disable the join room button to prevent the user from joining multiple rooms.
        joinRandomButton.interactable = false;
    }

    //Private room join failed
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        
        //log.text += "\nError. Unable to join the desired room.";
    }

    //Public room join failed
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //base.OnJoinRandomFailed(returnCode, message);

        log.text += "\nThere are no rooms available. Creating a new one";

        //Create a new room
        if (PhotonNetwork.CreateRoom(null, new RoomOptions()
            {MaxPlayers = maxPlayersInRoom}))
        {
            log.text += "\nRoom created.";
        }
        else
        {
            log.text += "\nError. Unable to create a new room";
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //base.OnDisconnected(cause);
        
        log.text += "\nDisconnected.";
        
        //Enable the connect button
        connectButton.interactable = true;
    }

    #endregion

    #region Methods

    public void Connect()
    {
        if (PhotonNetwork.ConnectUsingSettings())
            log.text += "\nConnection to server established";
        else
            log.text += "\nError. Couldn't connect to server...";
    }
    
    //Public room join
    public void JoinRandom()
    {
        if (!PhotonNetwork.JoinRandomRoom())
            log.text += "\nError. Couldn't join any room.";
    }
    
    //Private room create
    public void CreatePrivateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = false;
        roomOptions.MaxPlayers = maxPlayersInRoom;
        PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
    }
    
    //Private room join
    public void JoinPrivateRoom(string name)
    {
        PhotonNetwork.JoinRoom(name);
    }

    #endregion
}