using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManagerCreator : MonoBehaviourPunCallbacks
{
    //Needed to instantiate the manager with PhotonNetwork.Instantiate Method
    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate("GameManager", Vector3.zero, Quaternion.identity, 0);
        Destroy(this.gameObject);
    }
}
