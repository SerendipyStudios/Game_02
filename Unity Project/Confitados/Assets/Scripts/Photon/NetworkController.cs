using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    /****************************
     *
     * Documentation: https://doc-api.photonengine.com/en-us/current/getting-started/pun-intro
     * Scripting API: https://doc-api.photonengine.com/en/pun/v2/index.html
     **********************/    
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); //Connect to Photon servers
        
        //There are other ways to connect the game to server, which can be founded here:
        //     https://doc-api.photonengine.com/en/pun/v2/class_photon_1_1_pun_1_1_photon_network.html
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Ahora estamos conectados al servidor de la región: " + PhotonNetwork.CloudRegion);
    }
}
