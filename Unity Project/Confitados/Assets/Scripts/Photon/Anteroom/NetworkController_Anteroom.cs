using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkController_Anteroom : MonoBehaviourPunCallbacks
{
    #region Variables

    [SerializeField] private GameObject connectingDialog;
    
    [SerializeField] private Button createPrivateButton;
    [SerializeField] private Button joinPrivateButton;
    
    [SerializeField] private InputField if_roomCode_create;
    [SerializeField] private InputField if_roomCode_join;

    [SerializeField] private Text log;

    [SerializeField] private byte maxPlayersInRoom = 4;
    [SerializeField] private byte minPlayersInRoom = 2;
    
    private static System.Random random = new System.Random();

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        //this makes sure we can use PhotonNetwork.LoadLevel() on the master client and
        //    all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Disable the buttons
        createPrivateButton.interactable = false;
        joinPrivateButton.interactable = false;
        
        //Connect
        if (!PhotonNetwork.IsConnected)
        {
            Connect();
            connectingDialog.SetActive(true);
        }
        else
        {
            connectingDialog.SetActive(false);
            createPrivateButton.interactable = true;
            joinPrivateButton.interactable = true;
        }
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Ahora estamos conectados al servidor de la región: " + PhotonNetwork.CloudRegion);

        //Enable the join room button
        createPrivateButton.interactable = true;
        joinPrivateButton.interactable = true;
        connectingDialog.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();

        log.text += "\nJoined to room " + PhotonNetwork.CurrentRoom.Name + ".";

        //Disable the join room button to prevent the user from joining multiple rooms.
        createPrivateButton.interactable = false;
        joinPrivateButton.interactable = false;

        // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("Loading lobby.");

            // #Critical
            // Load the Room Level.
            PhotonNetwork.LoadLevel("Screen_02_1_lobby");
        }
    }

    //Private room join failed
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //base.OnJoinRoomFailed(returnCode, message);

        log.text += "\nError. Unable to join the desired room.";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //base.OnDisconnected(cause);

        log.text += "\nDisconnected.";

        ScreenManager.GoToScreen("Screen_01_0_MainMenu");
    }

    #endregion

    #region Methods

    public void Connect()
    {
        if (PhotonNetwork.ConnectUsingSettings())
            log.text += "\nConnection to server established";
        else
            log.text += "\nError. Couldn't connect to server...";
    }

    //Private room create
    public void CreatePrivateRoom()
    {
        //string _roomCode = if_roomCode_create.text;
        //if (_roomCode.Length < 4)
        //{
        //    log.text += "\nEnter a roomCode with a length greater than 4.";
        //    return;
        //}
        
        string _roomCode;
        bool done = false;

        while (!done)
        {
            _roomCode = GetRandomRoomCode(6);
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = false;
            roomOptions.MaxPlayers = maxPlayersInRoom;
            done = PhotonNetwork.CreateRoom(_roomCode, roomOptions, TypedLobby.Default);
        }
    }

    private string GetRandomRoomCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    //Private room join
    public void JoinPrivateRoom()
    {
        string _roomCode = if_roomCode_join.text;
        if (_roomCode.Length == 0)
            log.text += "\nEnter a valid room code";
        else
            PhotonNetwork.JoinRoom(_roomCode);
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    #endregion
}