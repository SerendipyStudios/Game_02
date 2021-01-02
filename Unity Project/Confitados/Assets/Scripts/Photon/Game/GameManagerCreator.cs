using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

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
}
