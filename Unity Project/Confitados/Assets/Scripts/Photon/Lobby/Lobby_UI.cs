using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Lobby_UI : MonoBehaviourPunCallbacks
{
    [Header("Play Buttons")]
    [SerializeField] private Button readyButton; 
    [SerializeField] private Button playButton;

    [Header("Chose Level")] 
    [SerializeField] private Text levelName;
    [SerializeField] private Button previousLevelButton;
    [SerializeField] private Button nextLevelButton;
    
    [Header("Room Code")]
    [SerializeField] private Text roomCode;
    
    private void Start()
    {
        playButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        previousLevelButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        nextLevelButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        playButton.interactable = false;
        
        roomCode.text = PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //base.OnPlayerLeftRoom(otherPlayer);
        playButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        previousLevelButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        nextLevelButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void NewPlayer()
    {
        
    }

    public void RemovePlayer()
    {
        
    }

    public void SetReady(bool ready)
    {
        //Access to the characterInfo to set its ready variable to true.
        //That object will be Networked so that other players can update their UIs too.
        playButton.interactable = ready;
    }

    public void SetLevelText(string text)
    {
        levelName.text = text;
    }

    public void AddReadyCallback(UnityAction call)
    {
        readyButton.onClick.AddListener(call);
    }
}
