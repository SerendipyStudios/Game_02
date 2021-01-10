using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerInfo : MonoBehaviourPunCallbacks, IPunObservable
{
    //Player variables
    [SerializeField] private int lives = 3;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private bool isSuperDashing = false;
    [SerializeField] private bool isFalling = false;
    
    [SerializeField] private byte rankPosition = Byte.MaxValue;

    #region Getters/Setters

    public int Lives
    {
        get => lives;
        set => lives = value;
    }
    
    public bool IsDashing
    {
        get => isDashing;
        set => isDashing = value;
    }

    public bool IsSuperDashing
    {
        get => isSuperDashing;
        set => isSuperDashing = value;
    }

    public bool IsFalling
    {
        get => isFalling;
        set => isFalling = value;
    }

    public byte RankPosition
    {
        get => rankPosition;
        set => rankPosition = value;
    }

    #endregion
    
    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(lives);
            stream.SendNext(isDashing);
            stream.SendNext(isSuperDashing);
            stream.SendNext(isFalling);
            stream.SendNext(rankPosition);
        }
        else
        {
            // Network player, receive data
            this.lives = (int) stream.ReceiveNext();
            this.isDashing = (bool) stream.ReceiveNext();
            this.isSuperDashing = (bool) stream.ReceiveNext();
            this.isFalling = (bool) stream.ReceiveNext();
            this.rankPosition = (byte) stream.ReceiveNext();
        }
    }

    #endregion
    
    #region Methods

    //Called on owner client
    [PunRPC]
    public void RpcSetRankPosition(byte position)
    {
        RankPosition = position;
    }
    
    #endregion
}
