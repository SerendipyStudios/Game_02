using System.Collections;
using System.Collections.Generic;
using Photon.Game;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInterfaceUI : MonoBehaviour
{
    private PlayerController playerController;
    
    
    //Interface elements
    [SerializeField] public Button exitGameButton;
    [SerializeField] private Text countdownText;
    
    private void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }
    
    public void Initialize(PlayerController _playerController)
    {
        playerController = _playerController;
        
        exitGameButton.onClick.AddListener(() => GameManager.Instance.LeaveGame());
    }
    
    public void SetCountdown(int time)
    {
        if (time == -1) countdownText.enabled = false;
        countdownText.text = time.ToString();
    }
}
