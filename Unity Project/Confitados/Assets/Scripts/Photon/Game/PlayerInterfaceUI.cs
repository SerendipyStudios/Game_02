using System.Collections;
using System.Collections.Generic;
using Photon.Game;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInterfaceUI : MonoBehaviour
{
    private PlayerController playerController;
    
    //Interface elements
    [SerializeField] public Button exitGameButton;
    
    private void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }
    
    public void Initialize(PlayerController _playerController)
    {
        playerController = _playerController;
        
        exitGameButton.onClick.AddListener(() => GameManager.Instance.LeaveGame());
    }
}
