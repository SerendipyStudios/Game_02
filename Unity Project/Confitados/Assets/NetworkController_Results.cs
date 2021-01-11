using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Photon.Game;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController_Results : MonoBehaviour
{
    public Text[] names;
    private GameObject localPlayer;

    private void Start()
    {
        //Se corre localmente este script

        //Coger el local player
        int viewId = GameManager.Instance.GetPlayerViewId(PhotonNetwork.LocalPlayer.ActorNumber);
        localPlayer = PhotonView.Find(viewId).gameObject;
        int position = localPlayer.GetComponent<PlayerInfo>().RankPosition;

        PlayerPrefs.SetInt("Cookies", PlayerPrefs.GetInt("Cookies", 0) + AssignRewards(position));
        PlayerPrefs.SetInt("Experience", PlayerPrefs.GetInt("Experience", 0) + AssignRewards(position));
        if (PlayerPrefs.GetInt("Experience") >= 200) //Every 200xp the player levels up
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);

        //Si quieres que solo pase el servidor (cliente que hace de servidor)
        //if (PhotonNetwork.IsMasterClient) ;

            //Si quieres que solo pase el que posee el objeto actual (pista, solo erres propietario si lo instancias tu con PhotonNetwork.Instantiate)
            //if (photonView.IsMine) ;

            //Pasarselo


        //Poner a cada uno en su lugar
        foreach(var player in PhotonNetwork.PlayerList)
        {
            int viewIdOther = GameManager.Instance.GetPlayerViewId(player.ActorNumber);
            GameObject localPlayerOther = PhotonView.Find(viewIdOther).gameObject;
            int positionOther = localPlayerOther.GetComponent<PlayerInfo>().RankPosition;
            string nameOther = player.NickName;
            names[positionOther - 1].text = positionOther + ". " + nameOther + ": +" + AssignRewards(positionOther) + " Galletas / +" + AssignRewards(positionOther) + " Exp";
        }
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
