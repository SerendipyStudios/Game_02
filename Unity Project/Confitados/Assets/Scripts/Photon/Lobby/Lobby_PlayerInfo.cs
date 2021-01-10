using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Lobby_PlayerInfo : MonoBehaviourPunCallbacks, IPunObservable
{
    private string name;
    private bool ready = false;

    private NetworkController_Lobby networkControllerLobby;

    #region Unity Callbacks
    
    private void Start()
    {
        if (PhotonNetwork.LocalPlayer != photonView.Owner) return;

        name = PhotonNetwork.LocalPlayer.NickName;
        ready = false;
    }

    public void Initialize(NetworkController_Lobby networkControllerLobby)
    {
        this.networkControllerLobby = networkControllerLobby;

        this.networkControllerLobby.gameObject.GetComponent<Lobby_UI>().AddReadyCallback(this.SetReady);
        networkControllerLobby.photonView.RPC("CmdRegister", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, photonView.ViewID);
    }
    
    #endregion

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(name);
            stream.SendNext(ready);
        }
        else
        {
            // Network player, receive data
            this.name = (string) stream.ReceiveNext();
            this.ready = (bool) stream.ReceiveNext();
        }
    }

    #endregion
    
    #region Getters & Setters

    private void SetReady()
    {
        this.ready = !this.ready;
        networkControllerLobby.photonView.RPC("CmdSetReady", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, this.ready);
    }

    public bool GetReady()
    {
        return ready;
    }
    
    #endregion
}
