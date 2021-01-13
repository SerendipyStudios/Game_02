using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCameraWinUI : MonoBehaviour
{
    [SerializeField] private Text winPlayerText;
    
    // Start is called before the first frame update
    void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    // Update is called once per frame
    public void Initialize(PlayerController _playerController)
    {
        winPlayerText.text = "¡" + _playerController.GetComponent<PlayerInfo>().photonView.Owner.NickName + " ha ganado!";
    }
}
