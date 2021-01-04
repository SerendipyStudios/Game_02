using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerCreator : MonoBehaviourPunCallbacks
{
    public GameObject GameManagerPrefab;
    
    //Needed to instantiate the manager with PhotonNetwork.Instantiate Method
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate(GameManagerPrefab.name, Vector3.zero, Quaternion.identity, 0);
        //Destroy(this.gameObject);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        SceneManager.LoadScene("Screen_02_0_anteroom");
        
        //base.OnLeftRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected");
        SceneManager.LoadScene("Screen_01_0_MainMenu");
        
        //base.OnDisconnected(cause);
    }
}
