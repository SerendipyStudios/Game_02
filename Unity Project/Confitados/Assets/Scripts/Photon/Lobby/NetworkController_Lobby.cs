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

    [SerializeField] private Lobby_PlayerInfo playerLobbyInfoPrefab;
    
    [SerializeField] private Text log;
    
    //Player configurations
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField] private byte maxPlayersInRoom = 6;
    [SerializeField] private byte minPlayersInRoom = 2;
    [SerializeField] private int currentPlayersInRoom = 0;
    [SerializeField] private Text playerCount;
    
    private int readyPlayersCount = 0;
    private List<int> playerViewIds;
    
    //Level
    private Levels.LevelsEnum choosedLevel = Levels.LevelsEnum.Galletown;
    
    #endregion
    
    #region UnityCallbacks

    private void Start()
    {
        playerViewIds = new List<int>();
        playerViewIds.Add(-1);

        PhotonNetwork.Instantiate(playerLobbyInfoPrefab.name, Vector3.zero, Quaternion.identity, 0)
            .GetComponent<Lobby_PlayerInfo>().Initialize(this);
        
        GetComponent<Lobby_UI>().SetLevelText(Levels.GetString(choosedLevel));
    }

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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        //base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("Player Entered room");
        playerViewIds.Add(-1);
        CheckAllReady();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        Debug.Log("Player Left room");
        //base.OnPlayerLeftRoom(otherPlayer);

        bool wasReady = PhotonView.Find(playerViewIds[otherPlayer.ActorNumber - 1]).GetComponent<Lobby_PlayerInfo>().GetReady();

        if (wasReady)
        {
            readyPlayersCount--;
        }
        
        CheckAllReady();
    }

    public override void OnLeftRoom()
    {
        //base.OnLeftRoom();
        
        ScreenManager.GoToScreen("Screen_02_0_anteroom");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //base.OnDisconnected(cause);

        log.text += "\nDisconnected.";

        ScreenManager.GoToScreen("Screen_01_0_MainMenu");
    }

    #endregion

    #region Methods

    //Only server
    public void StartGame()
    {
        //Set the room to not joinable
        PhotonNetwork.CurrentRoom.IsOpen = false;
        
        //When multiple levels are made, select between them here
        PhotonNetwork.LoadLevel(Levels.GetScene(choosedLevel));
    }
    
    public void LeaveRoom()
    {
        //ScreenManager.GoToScreen("Screen_02_0_anteroom");
        PhotonNetwork.LeaveRoom(true);
        //ScreenManager.GoToScreen("Screen_02_0_anteroom");
    }

    public void NextLevel()
    {
        choosedLevel = Levels.GetNextEnum(choosedLevel);
        photonView.RPC("RpcShowLevel", RpcTarget.All, Levels.GetString(choosedLevel));
    }

    public void PreviousLevel()
    {
        choosedLevel = Levels.GetPreviousEnum(choosedLevel);
        photonView.RPC("RpcShowLevel", RpcTarget.All, Levels.GetString(choosedLevel));
    }

    [PunRPC] //All clients
    private void RpcShowLevel(string levelName)
    {
        GetComponent<Lobby_UI>().SetLevelText(levelName);
    }

    [PunRPC] //Only server
    public void CmdRegister(int actorNumber, int viewId)
    {
        playerViewIds[actorNumber-1] = viewId;
    }
    
    [PunRPC] //Only server
    public void CmdSetReady(int actorNumber, bool ready)
    {
        this.readyPlayersCount += ready ? 1 : -1;

        CheckAllReady();
    }

    private void CheckAllReady()
    {
        Debug.Log("CheckAllReady: " + readyPlayersCount + "/" + PhotonNetwork.PlayerList.Length);
        
        if (
            readyPlayersCount == PhotonNetwork.PlayerList.Length &&
            PhotonNetwork.PlayerList.Length != 1
            )
        {
            GetComponent<Lobby_UI>().SetReady(true);
        }
        else
        {
            GetComponent<Lobby_UI>().SetReady(false);
        }
    }
    
    #endregion
    
}
