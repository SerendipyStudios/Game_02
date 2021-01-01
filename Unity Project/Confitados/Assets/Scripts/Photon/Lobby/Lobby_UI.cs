using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Lobby_UI : MonoBehaviour
{
    [SerializeField] private Button readyButton; 
    [SerializeField] private Button playButton; 
    
    private void Start()
    {
        playButton.enabled = PhotonNetwork.IsMasterClient;
    }

    public void NewPlayer()
    {
        
    }

    public void RemovePlayer()
    {
        
    }

    public void SetReady()
    {
        //Access to the characterInfo to set its ready variable to true.
        //That object will be Networked so that other players can update their UIs too.
    }
}
