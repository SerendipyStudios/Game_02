using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Photon.Game
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance;
        public GameObject playerPrefab;

        private PlayerController playerController;

        #region Unity Callbacks

        private void Awake()
        {
            this.transform.SetParent(GameObject.Find("--Managers--").GetComponent<Transform>(), false);
        }

        private void Start()
        {
            Instance = this;
            if (!PhotonNetwork.IsMasterClient) return;

            Debug.Log("Connected Players: " + PhotonNetwork.PlayerList.Length);

            //Instantiate a player if it is the first load
            if (playerPrefab == null)
            {
                Debug.LogError(
                    "<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",
                    this);
                return;
            }
            
            //Instantiate players
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);

            for (var i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                Player player = PhotonNetwork.PlayerList[i];
                Transform pos = LevelInfo.GetInitPos(i);
                photonView.RPC("RpcInstantiatePlayer", player, pos);
            }
        }

        #endregion

        #region Photon Callbacks

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}",
                    PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", otherPlayer.NickName); // seen when other disconnects


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}",
                    PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
            }
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("Screen_02_0_anteroom");
        }

        #endregion

        #region Methods

        private void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
                Debug.Log("Error. Tried to load a level from a non master client.");
            else
            {
                //Debug.LogFormat("PhotonNetwork: Loading Level {0}", PhotonNetwork.CurrentRoom.PlayerCount);
                Debug.LogFormat("PhotonNetwork: Loading Level {0}", 1);
                //PhotonNetwork.LoadLevel("PhotonMultiplayerScene");
            }
        }

        [PunRPC]
        private void RpcInstantiatePlayer(Transform initPos)
        {
            Debug.Log("Instantiating my player...");
            PhotonNetwork.Instantiate(this.playerPrefab.name, initPos.position, initPos.rotation,
                0);
        }
        
        #region Respawn

        public void RequestRespawn(PlayerController playerController)
        {
            if(this.playerController == null)
                this.playerController = playerController;
            Player player = PhotonNetwork.LocalPlayer;
            //if(!player.IsMasterClient)
                photonView.RPC("CmdRespawnPlayer", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
            //else
            //{
            //    playerController.SetRespawnPos(LevelInfo.GetInstance().GetRandomFreeInitPos());
            //}
        }

        [PunRPC]
        public void CmdRespawnPlayer(Player player)
        {
            Debug.Log("Cmd: Checking for free respawn slots...");
            Transform initPos = LevelInfo.GetInstance().GetRandomFreeInitPos();
            
            //Devuelve la posición de respawn al player que la solicitó
            photonView.RPC("RpcSetRespawnPos", player, initPos);
        }

        [PunRPC]
        private void RpcSetRespawnPos(Transform respawnTransform)
        {
            playerController.SetRespawnPos(respawnTransform);
        }
        
        #endregion

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion
    }
}