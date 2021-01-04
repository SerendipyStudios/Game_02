using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultsManager : MonoBehaviourPunCallbacks
{
    #region Photon Callbacks
    
    public override void OnLeftRoom()
    {
        //base.OnLeftRoom();
        SceneManager.LoadScene("Screen_02_0_anteroom");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //base.OnDisconnected(cause);
        SceneManager.LoadScene("Screen_01_0_MainMenu");
    }

    #endregion
    
    #region Methods
    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }
    
    #endregion
}
