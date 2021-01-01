using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

public class PlayerManager_Test : MonoBehaviourPun, IPunObservable
{
    #region Variables

    //Identity variables
    public static GameObject LocalPlayerInstance; //Needed to avoid instancing the player again when updating the scene

    //Referenced objects
    public GameObject PlayerUIPrefab; 
    
    //Movement variables
    private Animator animator;
    [SerializeField] private float directionDampTime = 0.25f;

    //Action variables
    public bool isDashing = false;

    //State variables
    public byte lives = 3;

    #endregion

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(lives);
            stream.SendNext(isDashing);
        }
        else
        {
            // Network player, receive data
            this.lives = (byte) stream.ReceiveNext();
            this.isDashing = (bool) stream.ReceiveNext();
        }
    }

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            PlayerManager.LocalPlayerInstance = this.gameObject;
        }

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Setup movement
        //animator = GetComponent<Animator>();
        //if (!animator)
        //{
        //    Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
        //}
        
        //Verify UI
        if (PlayerUIPrefab != null)
        {
            GameObject _uiGo =  Instantiate(PlayerUIPrefab);
            _uiGo.SendMessage ("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }

        //Camera work
        CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();
        if (_cameraWork != null)
        {
            if (photonView.IsMine)
                _cameraWork.OnStartFollowing();
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) return;
        if (!animator) return;

        //Handle movement
        ProcessInputs();
    }

    #endregion

    #region Methods

    private void ProcessInputs()
    {
        ProcessMovement();
        ProcessDash();
        ProcessLiveLoss();
    }

    private void ProcessMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (v < 0)
        {
            v = 0;
        }

        animator.SetFloat("Speed", h * h + v * v);
        animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
    }

    private void ProcessDash()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Dash down");
            isDashing = true;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            Debug.Log("Dash up");
            isDashing = false;
        }
    }

    private void ProcessLiveLoss()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            lives--;
            Debug.Log("Oh no! I fell! Live count: " + lives);
        }
    }

    #endregion
}