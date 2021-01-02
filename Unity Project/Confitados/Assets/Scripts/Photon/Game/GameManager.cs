using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Photon.Game
{
    public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public static GameManager Instance;
        public GameObject playerPrefab;

        private PlayerController playerController;

        private PlayerInfo[] playerInfos;
        private List<int> alivePlayers = new List<int>();

        //Event codes
        //Use events when the sender does not need to know what is going to happen next
        //    and multiple actions have to be made in consequence, in order to save RPC calls.
        //public const byte RequestRespawnCode = 1;

        #region Unity Callbacks

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            this.transform.SetParent(GameObject.Find("--Managers--").GetComponent<Transform>(), false);

            playerInfos = new PlayerInfo[PhotonNetwork.PlayerList.Length];
        }

        private void Start()
        {
            Instance = this;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                alivePlayers.Add(player.ActorNumber);
            }

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
                Transform initPos = LevelInfo.GetInitPos(i);
                photonView.RPC("RpcInstantiatePlayer", player, initPos.position, initPos.rotation);
            }
        }

        public override void OnEnable()
        {
            //base.OnEnable();
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            //base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        #endregion

        #region Photon Callbacks

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                //stream.SendNext(alivePlayers);
            }
            else
            {
                // Network player, receive data
                //this.alivePlayers = (List<int>) stream.ReceiveNext();
            }
        }

        #endregion

        //Executed in masterClient
        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;

            switch (eventCode)
            {
                //Implement here the desired events
                default:
                    break;
            }
        }

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

            if (!PhotonNetwork.IsMasterClient) return;

            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}",
                PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            //LoadArena();

            //Check if he was alive
            if (playerInfos[otherPlayer.ActorNumber].Lives == 0)
                CmdPlayerDied(otherPlayer.ActorNumber);
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

        //Called on client side only
        [PunRPC]
        private void RpcInstantiatePlayer(Vector3 initPos, Quaternion initRot)
        {
            Debug.Log("Instantiating my player...");
            PhotonNetwork.Instantiate(this.playerPrefab.name, initPos, initRot,
                0);
        }

        //Both client and server
        [PunRPC]
        private void CmdRegisterPlayer(int viewId, int actorNumber)
        {
            PlayerInfo playerInfo = PhotonView.Find(viewId).gameObject.GetComponent<PlayerInfo>();
            playerInfos[actorNumber - 1] = playerInfo;
        }

        #region Respawn

        //Executed in client
        public void RequestRespawn(PlayerController playerController)
        {
            if (this.playerController == null)
                this.playerController = playerController;

            photonView.RPC("CmdRespawnPlayer", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer);
        }

        //Executed in server
        [PunRPC]
        private void CmdRespawnPlayer(Player player)
        {
            Debug.Log("Cmd: Checking for free respawn slots...");
            Transform initPos = LevelInfo.GetInstance().GetRandomFreeInitPos();

            //Devuelve la posición de respawn al player que la solicitó
            photonView.RPC("RpcSetRespawnPos", player, initPos.position, initPos.rotation);
        }

        //Executed in client
        [PunRPC]
        private void RpcSetRespawnPos(Vector3 respawnPos, Quaternion respawnRot)
        {
            playerController.SetRespawnPos(respawnPos, respawnRot);
        }

        #endregion

        //Executed in server
        [PunRPC]
        private void CmdPlayerDied(int actorNumber)
        {
            playerInfos[actorNumber - 1].RankPosition = (byte) alivePlayers.Count;
            alivePlayers.Remove(actorNumber);
            photonView.RPC("RpcRemoveAlivePlayer", RpcTarget.Others, actorNumber);

            if (alivePlayers.Count == 1)
                GameEnd();
        }

        //Only in client side
        [PunRPC]
        private void RpcRemoveAlivePlayer(int actorNumber)
        {
            //Can't guarantee that alivePlayers is going to be sync!
            alivePlayers.Remove(actorNumber);
        }

        //Server side only
        [PunRPC]
        public void CmdGetAlivePlayer(int index, int viewId, PhotonMessageInfo photonMessageInfo)
        {
            Debug.Log("Alive players: " + alivePlayers.Count);
            //Revisar actor numbers
            if (alivePlayers.Count > index)
                PhotonView.Find(viewId).gameObject.GetComponent<PlayerController>()
                    .photonView.RPC("RpcSetCameraDied", photonMessageInfo.Sender,
                        playerInfos[alivePlayers[index]-1].photonView.ViewID);
            else
            {
                GameObject aux = PhotonView.Find(viewId).gameObject;
                aux.GetComponent<PlayerController>().photonView.RPC("RpcSetCameraDied", photonMessageInfo.Sender, -1);
            }

            Debug.Log("Donete");
        }

        private void GameEnd()
        {
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion
    }
}