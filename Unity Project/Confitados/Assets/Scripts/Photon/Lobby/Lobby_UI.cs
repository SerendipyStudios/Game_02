using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Lobby_UI : MonoBehaviour
{
    [Header("Play Buttons")]
    [SerializeField] private Button readyButton; 
    [SerializeField] private Button playButton;

    [Header("Chose Level")] 
    [SerializeField] private Text levelName;
    [SerializeField] private Button previousLevelButton;
    [SerializeField] private Button nextLevelButton;
    
    private void Start()
    {
        playButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        playButton.interactable = false;
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
