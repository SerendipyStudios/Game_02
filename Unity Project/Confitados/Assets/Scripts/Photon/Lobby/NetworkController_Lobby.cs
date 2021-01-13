using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private ScrollRect scrollViewObject;
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
        for(int i=0; i<PhotonNetwork.CurrentRoom.PlayerCount; i++)
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
                playerCount.text += "\nPlayer " + player.Key + " (" + player.Value.NickName + "): " 
                                    + (PhotonView.Find(playerViewIds[player.Value.ActorNumber-1]).
                                        gameObject.GetComponent<Lobby_PlayerInfo>().GetReady() ? "Listo" : "No listo");
                Canvas.ForceUpdateCanvases();
                scrollViewObject.verticalNormalizedPosition = 1f;
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
        //base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("Nuevo jugador se unió a la sala: " + newPlayer.NickName);
        log.text += "Nuevo jugador se unió a la sala: " + newPlayer.NickName;
        playerViewIds.Add(-1);
        CheckAllReady();
        
        photonView.RPC("RpcSyncInfo", RpcTarget.All, playerViewIds.ToArray(), readyPlayersCount);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //if (!PhotonNetwork.IsMasterClient) return;
        
        Debug.Log("Un jugador dejó la sala: " + otherPlayer.NickName);
        log.text += "Un jugador dejó la sala: " + otherPlayer.NickName;
        //base.OnPlayerLeftRoom(otherPlayer);

        bool wasReady = PhotonView.Find(playerViewIds[otherPlayer.ActorNumber - 1]).GetComponent<Lobby_PlayerInfo>().GetReady();

        if (wasReady)
        {
            readyPlayersCount--;
        }
        
        photonView.RPC("RpcShowLevel", RpcTarget.All, Levels.GetString(choosedLevel));

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

        log.text += "\nDesconectado del servidor.";

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
        photonView.RPC("RpcSyncInfo", RpcTarget.All, playerViewIds.ToArray(), readyPlayersCount);
    }
    
    [PunRPC] //Only server
    public void CmdSetReady(int actorNumber, bool ready)
    {
        this.readyPlayersCount += ready ? 1 : -1;
        photonView.RPC("RpcSyncInfo", RpcTarget.All, playerViewIds.ToArray(), readyPlayersCount);

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

    //Clients
    [PunRPC]
    private void RpcSyncInfo(int[] viewIds, int readyPlayers)
    {
        Debug.Log("RpcSyncPlayerViews.");
        playerViewIds = new List<int>(viewIds);
        readyPlayersCount = readyPlayers;
    }
    
    #endregion
    
}
