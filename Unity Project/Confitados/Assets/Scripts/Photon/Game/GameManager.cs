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
        #region Variables

        public static GameManager Instance;
        [SerializeField] public GameObject playerPrefab;

        /* Old player tracking data structure
        //private PlayerInfo[] playerInfos;
        //private List<int> alivePlayers = new List<int>();
        */

        private int[] allPlayers_ViewIds;
        private List<int> alivePlayers_ViewIds;

        //Event codes
        //Use events when the sender does not need to know what is going to happen next
        //    and multiple actions have to be made in consequence, in order to save RPC calls.
        //public const byte RequestRespawnCode = 1;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            //Gameobject configuration
            DontDestroyOnLoad(this.gameObject);
            this.transform.SetParent(GameObject.Find("--Managers--").GetComponent<Transform>(), false);

            //Init player tracking data structure
            allPlayers_ViewIds = new int[PhotonNetwork.PlayerList.Length];
            alivePlayers_ViewIds = new List<int>(PhotonNetwork.PlayerList.Length);
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
            base.OnEnable();
            //PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            //PhotonNetwork.RemoveCallbackTarget(this);
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
            /* Load adaptable arena
            //Load new arena
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName);

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}",
                    PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
            }
            */
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            /*Load adaptable arena
            Debug.LogFormat("OnPlayerLeftRoom() {0}", otherPlayer.NickName); // seen when other disconnects

            if (!PhotonNetwork.IsMasterClient) return;

            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}",
                PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            //LoadArena();
            */

            //Check if he was alive
            PlayerInfo playerInfo = PhotonView.Find(allPlayers_ViewIds[otherPlayer.ActorNumber - 1])
                .GetComponent<PlayerInfo>();
            if (playerInfo.Lives == 0)
                CmdPlayerDied(otherPlayer.ActorNumber);
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("Screen_02_0_anteroom");
        }

        #endregion

        #region SceneMethods

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


        private void GameEnd()
        {
            Debug.Log("Game Ended");
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region PlayerMethods

        [PunRPC] //Called on client side only
        private void RpcInstantiatePlayer(Vector3 initPos, Quaternion initRot)
        {
            Debug.Log("Instantiating my player...");
            PhotonNetwork.Instantiate(this.playerPrefab.name, initPos, initRot,
                0);
        }


        [PunRPC] //Both client and server
        private void CmdRegisterPlayer(int actorNumber, int viewId)
        {
            PlayerInfo playerInfo = PhotonView.Find(viewId).gameObject.GetComponent<PlayerInfo>();
            allPlayers_ViewIds[actorNumber - 1] = viewId;
            alivePlayers_ViewIds.Add(viewId);
        }

        /* Old respawn call system
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
        */


        [PunRPC] //Executed in server
        private void CmdRespawnPlayer(int actorNumber, PhotonMessageInfo photonMessageInfo)
        {
            Debug.Log("Cmd: Checking for free respawn slots...");
            Transform initPos = LevelInfo.GetInstance().GetRandomFreeInitPos();

            //Devuelve la posición de respawn al player que la solicitó
            PhotonView.Find(allPlayers_ViewIds[actorNumber - 1]).GetComponent<PlayerController>()
                .photonView.RPC("RpcSetRespawnPos", photonMessageInfo.Sender,
                    initPos.position, initPos.rotation);
        }


        [PunRPC] //Server side only
        public void CmdGetAlivePlayer(int actorNumber, int currentPlayerIndex, bool forward, PhotonMessageInfo photonMessageInfo)
        {
            //Debug.Log("Get Alive player.");
            Debug.Log("GetAlivePlayer. Alive players: " + alivePlayers_ViewIds.Count);

            if (alivePlayers_ViewIds.Count == 0)
            {
                PhotonView.Find(allPlayers_ViewIds[actorNumber - 1]).gameObject
                    .GetComponent<PlayerController>().photonView.RPC("RpcSetCameraDied", photonMessageInfo.Sender, -1);
                return;
            }

            //Revisar actor numbers
            int index;
            if (currentPlayerIndex == -1)
                index = 0;
            else
            {
                index = (currentPlayerIndex - 1) +
                        (forward ? 1 : -1);
                index %= alivePlayers_ViewIds.Count;

                if (index < 0)
                    index += alivePlayers_ViewIds.Count;
            }

            PhotonView.Find(allPlayers_ViewIds[actorNumber - 1]).gameObject.GetComponent<PlayerController>()
                .photonView.RPC("RpcSetCameraDied", photonMessageInfo.Sender,
                    PhotonView.Find(alivePlayers_ViewIds[index]).GetComponent<PlayerController>()
                        .photonView.ViewID
                );

            //Debug.Log("Donete");
        }


        [PunRPC] //Executed in server
        private void CmdPlayerDied(int actorNumber)
        {
            //Debug.Log("Player Died: " + actorNumber);
            PlayerInfo playerInfo = PhotonView.Find(allPlayers_ViewIds[actorNumber - 1]).GetComponent<PlayerInfo>();
            playerInfo.RankPosition = (byte) alivePlayers_ViewIds.Count;

            alivePlayers_ViewIds.Remove(allPlayers_ViewIds[actorNumber - 1]);
            //photonView.RPC("RpcRemoveAlivePlayer", RpcTarget.Others, actorNumber);

            if (alivePlayers_ViewIds.Count <= 1)
                GameEnd();
        }

        /* Remove alivePlayer also from client
        //Only in client side
        [PunRPC]
        private void RpcRemoveAlivePlayer(int actorNumber)
        {
            //Can't guarantee that alivePlayers is going to be sync!
            //    Execute all related methods in server.
            alivePlayers_ViewIds.Remove(actorNumber);
        }
        */

        #endregion
    }
}