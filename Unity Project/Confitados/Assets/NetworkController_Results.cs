using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Photon.Game;
using UnityEngine;

public class NetworkController_Results : MonoBehaviour
{
    private void Start()
    {
        //Se corre localmente este script

        //Coger el local player
        int viewId = GameManager.Instance.GetPlayerViewId(PhotonNetwork.LocalPlayer.ActorNumber);
        GameObject localPlayer = PhotonView.Find(viewId).gameObject;
        int position = localPlayer.GetComponent<PlayerInfo>().RankPosition;

        PlayerPrefs.SetInt("Cookies", PlayerPrefs.GetInt("Cookies", 0) + AssignRewards(position));

        //Si quieres que solo pase el servidor (cliente que hace de servidor)
        //if (PhotonNetwork.IsMasterClient) ;

        //Si quieres que solo pase el que posee el objeto actual (pista, solo erres propietario si lo instancias tu con PhotonNetwork.Instantiate)
        //if (photonView.IsMine) ;

        //Pasarselo
    }

    private int AssignRewards(int rank)
    {
        switch (rank)
        {
            case 1:
                return 20;
            case 2:
                return 15;
            case 3:
                return 10;
            default:
                return 5;
        }
    }
}
