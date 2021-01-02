using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerInfo : MonoBehaviourPunCallbacks
{
    //Player variables
    [SerializeField] private byte lives = 3;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private bool isSuperDashing = false;
    [SerializeField] private bool isFalling = false;

    #region Getters/Setters

    public byte Lives
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
        }
        else
        {
            // Network player, receive data
            this.lives = (byte) stream.ReceiveNext();
            this.isDashing = (bool) stream.ReceiveNext();
            this.isSuperDashing = (bool) stream.ReceiveNext();
            this.isFalling = (bool) stream.ReceiveNext();
        }
    }

    #endregion
}
