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

        #region Unity Callbacks

        private void Start()
        {
            Instance = this;
            
            //Instantiate a player if it is the first load
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
            }
            else
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
                
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        #endregion
        
        #region Photon Callbacks

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", newPlayer.NickName);
            
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
                
                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", otherPlayer.NickName); // seen when other disconnects


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
                
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
            if(!PhotonNetwork.IsMasterClient)
                Debug.Log("Error. Tried to load a level from a non master client.");
            else
            {
                //Debug.LogFormat("PhotonNetwork: Loading Level {0}", PhotonNetwork.CurrentRoom.PlayerCount);
                Debug.LogFormat("PhotonNetwork: Loading Level {0}", 1);
                PhotonNetwork.LoadLevel("PhotonMultiplayerScene");
            }
            
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        
        #endregion
    }
}
