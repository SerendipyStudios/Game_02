using System;
using System.Collections;
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

        //Game
        public static GameManager Instance;
        [SerializeField] public GameObject playerPrefab;
        [SerializeField] private int countdownTime = 3;
        private int countdownTimeActual;
        WaitForSeconds waitASecond = new WaitForSeconds(1);

        /* Old player tracking data structure
        //private PlayerInfo[] playerInfos;
        //private List<int> alivePlayers = new List<int>();
        */

        //Player tracking data structures
        private int[] allPlayers_ViewIds;
        private List<int> alivePlayers_ViewIds;

        //Event codes
        //Use events when the sender does not need to know what is going to happen next
        //    and multiple actions have to be made in consequence, in order to save RPC calls.
        //public const byte RequestRespawnCode = 1;
        public const byte CountdownCode = 1;
        public const byte PlayerDeadCode = 2;
        public const byte WinCode = 3;

        //GameState
        public enum GameStateEnum
        {
            Init = 0,
            Playing = 1,
            Finished = 2,
        }

        public GameStateEnum gameState = GameStateEnum.Init;

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
            gameState = GameStateEnum.Init;

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
            //Does not execute because this is a networked object. Thus will be destroyed on disconnect.
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            //Does not execute because this is a networked object. Thus will be destroyed on disconnect.

            Debug.Log("OnDisconnected");
            //SceneManager.LoadScene("Screen_02_0_anteroom");
            //base.OnDisconnected(cause);
        }

        #endregion

        #region Getters

        public int GetPlayerViewId(int _actorNumber)
        {
            return allPlayers_ViewIds[_actorNumber - 1];
        }

        #endregion

        #region SceneMethods

        /*
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
        */


        private void GameEnd(int actorNumber)
        {
            Debug.Log("Game Ended");
            gameState = GameStateEnum.Finished;

            //Send event
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
            PhotonNetwork.RaiseEvent(GameManager.WinCode, new object[] {(object) actorNumber}, raiseEventOptions,
                SendOptions.SendReliable);

            //Wait x seconds and launch nextLevel method
            StartCoroutine(NextLevelCoroutine());
        }

        private IEnumerator NextLevelCoroutine()
        {
            yield return new WaitForSeconds(3);
            NextLevel();
        }

        private void NextLevel()
        {
            //Implement here a script if we want to support multiple games in a row. [HERE]
            PhotonNetwork.LoadLevel("Screen_02_3_results");
        }

        public void LeaveGame()
        {
            Debug.Log("LeaveGame");

            GameObject player = PhotonView.Find(GetPlayerViewId(PhotonNetwork.LocalPlayer.ActorNumber)).gameObject;
            player.GetComponent<PlayerController>().Delete();

            //Leave
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region Game Countdown

        /* Cuidado! Si mandas demasiados RPCs en el mismo game loop, el navegador excede su límite de memoria!!
        private IEnumerator GameStartCountdownCoroutine()
        {
            //int localCountdown = countdownTimeActual;
            while (countdownTimeActual > 0)
            {
                Debug.Log("Inicio del while");
                //Debug.Log("Countdown local: " + localCountdown);
                Debug.Log("Countdown: " + countdownTimeActual);
                //photonView.RPC("RpcSetCountdownText", RpcTarget.All, countdownTimeActual);
                //localCountdown--;
                countdownTimeActual--;
                //Debug.Log("Wait for seconds enter: " + countdownTimeActual);
                //yield return waitASecond;
                //Debug.Log("Wait for seconds exit: " + countdownTimeActual);
            }
            
            //Zero
            //Debug.Log("Countdown: " + countdownTimeActual);
            //photonView.RPC("RpcSetCountdownText", RpcTarget.All, countdownTimeActual);
            photonView.RPC("RpcStartGame", RpcTarget.All);

            //Deactivate countdown
            //yield return waitASecond;
            countdownTimeActual--;
            Debug.Log("Countdown: " + countdownTimeActual);
            //photonView.RPC("RpcSetCountdownText", RpcTarget.All, countdownTimeActual);
            
            yield break;
        }
        */

        /*
        [PunRPC] //All
        private void RpcSetCountdownText(int time)
        {
            PlayerManager.LocalPlayerInstance
                .GetComponent<PlayerController>().playerInterfaceUI.SetCountdown(time);
        }
        */

        //Usando eventos
        //Only server
        private IEnumerator GameStartCountdownCoroutine()
        {
            //Event init
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};

            while (countdownTimeActual > 0)
            {
                Debug.Log("Inicio del while");
                Debug.Log("Countdown: " + countdownTimeActual);

                //photonView.RPC("RpcSetCountdownText", RpcTarget.All, countdownTimeActual);
                PhotonNetwork.RaiseEvent(GameManager.CountdownCode, new object[] {(object) countdownTimeActual},
                    raiseEventOptions,
                    SendOptions.SendReliable);

                countdownTimeActual--;
                //Debug.Log("Wait for seconds enter: " + countdownTimeActual);
                yield return waitASecond;
                //Debug.Log("Wait for seconds exit: " + countdownTimeActual);
            }

            //Zero
            Debug.Log("Countdown: " + countdownTimeActual);
            PhotonNetwork.RaiseEvent(GameManager.CountdownCode, new object[] {(object) countdownTimeActual},
                raiseEventOptions,
                SendOptions.SendReliable);
            //photonView.RPC("RpcSetCountdownText", RpcTarget.All, countdownTimeActual);
            photonView.RPC("RpcStartGame", RpcTarget.All);
            LevelInfo.Instance.StartGame();

            //Deactivate countdown
            yield return waitASecond;
            countdownTimeActual--;
            Debug.Log("Countdown: " + countdownTimeActual);
            PhotonNetwork.RaiseEvent(GameManager.CountdownCode, new object[] {(object) countdownTimeActual},
                raiseEventOptions,
                SendOptions.SendReliable);
            //photonView.RPC("RpcSetCountdownText", RpcTarget.All, countdownTimeActual);
        }

        [PunRPC] //All
        private void RpcStartGame()
        {
            Debug.Log("Start game: GameManager");
            gameState = GameStateEnum.Playing;
            PhotonView.Find(allPlayers_ViewIds[PhotonNetwork.LocalPlayer.ActorNumber - 1])
                .GetComponent<PlayerController>().StartGame();
            //LevelInfo.Instance.StartGame();
        }

        #endregion

        #region Player Methods

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

            if (!PhotonNetwork.IsMasterClient) return;

            //Si se han registrado todos, iniciar la cuenta atrás
            if (alivePlayers_ViewIds.Count == PhotonNetwork.PlayerList.Length)
            {
                //photonView.RPC("RpcStartGame", RpcTarget.All);
                countdownTimeActual = countdownTime;
                StartCoroutine(GameStartCountdownCoroutine());
            }
        }

        [PunRPC] //Server side only
        public void CmdGetAlivePlayer(int actorNumber, int currentPlayerActorNumber, bool forward,
            PhotonMessageInfo photonMessageInfo)
        {
            //Debug.Log("Get Alive player.");
            Debug.Log("GetAlivePlayer. Alive players: " + alivePlayers_ViewIds.Count);

            //If there are not alive players
            if (alivePlayers_ViewIds.Count == 0)
            {
                PhotonView.Find(allPlayers_ViewIds[actorNumber - 1]).gameObject
                    .GetComponent<PlayerController>().photonView.RPC("RpcSetCameraDied", photonMessageInfo.Sender, -1);
                return;
            }

            //If it is the first time
            if (currentPlayerActorNumber == -1)
            {
                PhotonView.Find(allPlayers_ViewIds[actorNumber - 1]).gameObject.GetComponent<PlayerController>()
                    .photonView.RPC("RpcSetCameraDied", photonMessageInfo.Sender,
                        PhotonView.Find(alivePlayers_ViewIds[0]).GetComponent<PlayerController>()
                            .photonView.ViewID
                    );

                return;
            }

            //Encontrar el índice del actual y avanzar/retroceder al deseado
            int index;
            int currentPlayerViewId = allPlayers_ViewIds[currentPlayerActorNumber - 1];
            int currentPlayerIndex = alivePlayers_ViewIds.FindIndex((value) => value == currentPlayerViewId);
            if (currentPlayerIndex == -1)
                index = 0;
            else
            {
                index = (currentPlayerIndex) +
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
            //playerInfo.RankPosition = (byte) alivePlayers_ViewIds.Count;    //Only updates masterClient's because it has not authority to override owner's value.
            playerInfo.photonView.RPC("RpcSetRankPosition", RpcTarget.All, (byte) alivePlayers_ViewIds.Count);

            //Si no es el último, borrarlo (esto puede pasar cuando la batalla final está muy ajustada)
            if (alivePlayers_ViewIds.Count != 1)
                alivePlayers_ViewIds.Remove(allPlayers_ViewIds[actorNumber - 1]);
            //photonView.RPC("RpcRemoveAlivePlayer", RpcTarget.Others, actorNumber);

            //Send event
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
            PhotonNetwork.RaiseEvent(GameManager.PlayerDeadCode, new object[] {(object) actorNumber}, raiseEventOptions,
                SendOptions.SendReliable);

            if (alivePlayers_ViewIds.Count <= 1)
            {
                //PhotonView.Find(alivePlayers_ViewIds[0]).GetComponent<PlayerInfo>().RankPosition = (byte) alivePlayers_ViewIds.Count; //Only updates masterClient's because it has not authority to override owner's value.
                PhotonView.Find(alivePlayers_ViewIds[0]).GetComponent<PlayerInfo>().photonView
                    .RPC("RpcSetRankPosition", RpcTarget.All, (byte) 1);
                GameEnd(PhotonView.Find(alivePlayers_ViewIds[0]).GetComponent<PlayerController>().photonView.Owner
                    .ActorNumber);
            }
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

        #region Player Respawn

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

        #endregion

        #endregion
    }
}