using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Game;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Random = System.Random;

[RequireComponent(typeof(InputPlayer))]
public class PlayerController : MonoBehaviourPunCallbacks, IOnEventCallback
{
    #region Variables

    //Movement
    [Header("Player speed")] public float moveSpeed;
    private Vector3 movement;
    private Vector3 dashDirection;

    //Rotation
    [Range(0.0f, 1.0f)] [SerializeField] private float rotationSpeed;
    private Quaternion targetRotation;

    //Action Execution Flags
    private bool executeDash = false;
    private bool executeSuperDash = false;
    private bool executeFall = false;

    [Header("Dashes impulse")] public float dashImpulse;
    public float superDashImpulse;
    public float superDashIncrease;
    private int superDashImprovements = 0;

    [Header("Dashes cooldowns")] public float dashAgainCooldown;
    public float superDashAgainCooldown;
    private float dashAgainCooldownAux;
    private float superDashAgainCooldownAux;

    //Surfaces interaction
    [Header("Surfaces interaction")] public float iceDrag;
    public float stickyDrag;

    //References
    [HideInInspector] public Rigidbody rb;
    private Animator anim;
    private InputPlayer input;
    private PlayerInfo playerInfo;
    private CameraFollow cameraFollow;

    //Animations
    [Header("Animations")] private int runHashCode;
    public float fallTime = 1f;

    //Identity variables
    //public static GameObject LocalPlayerInstance; //Needed to avoid instancing the player again when updating the scene

    //Linked objects
    [Header("Player UI")] 
    public PlayerUI PlayerUIPrefab;
    public PlayerControlUI PlayerInputUIPrefab;
    public PlayerCameraSpectatorUI PlayerCameraSpectatorUIPrefab;

    [Header("Sound")] public SoundManager SoundManagerPrefab;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);

        //Instantiate UI
        if (PlayerUIPrefab != null)
        {
            playerInfo = GetComponent<PlayerInfo>();
            PlayerUI _uiGo = Instantiate(PlayerUIPrefab);
            _uiGo.SendMessage("SetTarget", playerInfo, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }

        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (!photonView.IsMine) this.enabled = false;
        PlayerManager.LocalPlayerInstance = this.gameObject;

        //Get local references
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        input = GetComponent<InputPlayer>();
        if (Camera.main != null)
            cameraFollow = Camera.main.GetComponent<CameraFollow>();

        runHashCode = Animator.StringToHash("Movement");
    }

    private void Start()
    {
        //Register player on gameManager
        if (photonView.IsMine)
            GameManager.Instance.photonView.RPC("CmdRegisterPlayer", RpcTarget.All,
                PhotonNetwork.LocalPlayer.ActorNumber,
                photonView.ViewID
            );

        //Setup movement
        dashDirection = input.faceDirection;

        //Set camera follow
        cameraFollow.Initialize(this);

        //Instantiate Controls
        if (PlayerInputUIPrefab != null)
        {
            PlayerControlUI inputUiGo = Instantiate(PlayerInputUIPrefab);
            input.Initialize(inputUiGo.GetComponent<PlayerControlUI>());
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerInputUIPrefab reference on player Prefab.", this);
        }

        //Instantiate SoundManager
        if (SoundManagerPrefab != null)
        {
            SoundManager soundManager = Instantiate(SoundManagerPrefab);
            soundManager.Initialize(this);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerInputUIPrefab reference on player Prefab.", this);
        }
    }

    private void Update()
    {
        //Calculate movement
        movement = new Vector3(input.inputX, 0f, input.inputZ) * moveSpeed;

        //Calculate rotation
        targetRotation =
            Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(input.faceDirection), rotationSpeed);

        //Check the player's animation parameters
        anim.SetFloat(runHashCode, Mathf.Max(Mathf.Abs(input.inputX), Mathf.Abs(input.inputZ)));

        //If the player uses a dash
        if (input.dashInput && !playerInfo.IsDashing)
        {
            executeDash = true;
            playerInfo.IsDashing = true;
            dashDirection = input.faceDirection;
            dashAgainCooldownAux = dashAgainCooldown;
            SoundManager.sharedInstance.dash_SND.Play();
        }

        //If the player uses a superDash
        if (input.superDashInput && !playerInfo.IsSuperDashing)
        {
            executeSuperDash = true;
            playerInfo.IsSuperDashing = true;
            dashDirection = input.faceDirection;
            superDashAgainCooldownAux = superDashAgainCooldown;
            SoundManager.sharedInstance.superDash_SND.Play();
        }

        //Dashes countdowns and speed reduction
        if (playerInfo.IsDashing)
            DashCountdown();

        if (playerInfo.IsSuperDashing)
            SuperDashCountdown();

        //Check if the player is falling off the stage
        if (transform.position.y < LevelInfo.worldLimit
            && !playerInfo.IsFalling)
        {
            executeFall = true;
            playerInfo.IsFalling = true;
        }
    }

    private void FixedUpdate()
    {
        //Check if I'm dead

        //Check if I'm falling
        if (executeFall)
            Fall();

        //Check if I have to do a dash
        if (executeDash)
        {
            rb.AddForce(dashDirection * dashImpulse, ForceMode.Impulse);
            executeDash = false;
        }

        if (executeSuperDash)
        {
            rb.AddForce(dashDirection * superDashImpulse, ForceMode.Impulse);
            executeSuperDash = false;
        }

        rb.AddForce(movement);
        transform.rotation = targetRotation;
    }
    
    public void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    #endregion
    
    #region Photon Callbacks
    
    public void OnEvent(EventData photonEvent)
    {
        //Debug.Log("Camera Spectator: EventCode received.");
        byte eventCode = photonEvent.Code;
        object[] datas;
        int _actorNumber;
            
        switch (eventCode)
        {
            case GameManager.PlayerDeadCode:
                Debug.Log("Camera Spectator: PlayerDeadCode received.");
                if (cameraFollow.GetCameraMode() != CameraFollow.CameraModeEnum.Win)
                {
                    datas = ((object[]) photonEvent.CustomData);
                    _actorNumber = (int) datas[0];
                    if (_actorNumber == cameraFollow.GetPlayer())
                        ChangeCameraSpectatorPlayer(true);
                }
                break;
            case GameManager.WinCode:
                Debug.Log("Camera Spectator: PlayerDeadCode received.");
                datas = ((object[]) photonEvent.CustomData);
                _actorNumber = (int) datas[0];
                
                //Paralyze all players
                rb.constraints = RigidbodyConstraints.FreezeAll;
                
                //Set close-up camera
                cameraFollow.SetPlayer(PhotonView.Find(GameManager.Instance.GetPlayerViewId(_actorNumber)).GetComponent<PlayerController>());
                cameraFollow.SetCameraMode(CameraFollow.CameraModeEnum.Win);
                break;
            default:
                break;
        }
    }
    
    #endregion

    #region Triggers

    void OnTriggerEnter(Collider other)
    {
        //Floors
        if (other.gameObject.tag == "IceFloor")
            rb.drag = iceDrag;
        if (other.gameObject.tag == "StickyFloor")
            rb.drag = stickyDrag;

        if (other.gameObject.tag == "Limit")
            return;
    }

    void OnTriggerExit(Collider other)
    {
        //Floors
        //En realidad, el suelo por defecto debería ser un tipo de suelo también
        if (other.gameObject.tag == "IceFloor" || other.gameObject.tag == "StickyFloor")
            rb.drag = 2.5f;
    }

    #endregion

    #region CountDowns

    private void DashCountdown()
    {
        if (dashAgainCooldownAux >= 0)
            dashAgainCooldownAux -= Time.deltaTime;
        else
        {
            playerInfo.IsDashing = false;
            SoundManager.sharedInstance.endCooldown_SND.Play();
        }
    }

    private void SuperDashCountdown()
    {
        if (superDashAgainCooldownAux >= 0)
            superDashAgainCooldownAux -= Time.deltaTime;
        else
        {
            playerInfo.IsSuperDashing = false;
            SoundManager.sharedInstance.endCooldown_SND.Play();
        }
    }

    #endregion

    #region Sound

    public void PlayCurrentStepsSound()
    {
        SoundManager.sharedInstance.OnPlayerSteps += SoundManager.sharedInstance.PlayStepSound;
        //SoundManager.sharedInstance.PlayStepSound();
    }

    #endregion

    #region Fall Methods

    private void Fall()
    {
        executeFall = false;

        //Quitarle el control al jugador
        input.enabled = false;

        //Desconectar cámara
        cameraFollow.SetCameraMode(CameraFollow.CameraModeEnum.Disconnected);

        //Rutina de reacción
        StartCoroutine(FallResponse());
    }

    IEnumerator FallResponse()
    {
        //Espera un tiempo para que se vea la animación
        yield return new WaitForSeconds(fallTime);

        //Restar una vida
        playerInfo.Lives--;

        if (playerInfo.Lives > 0)
        {
            //Devolver el control al jugador
            input.enabled = true;

            //Linkar cámara
            cameraFollow.SetCameraMode(CameraFollow.CameraModeEnum.Following);

            //Respawn
            GameManager.Instance.photonView.RPC("CmdRespawnPlayer", RpcTarget.MasterClient,
                PhotonNetwork.LocalPlayer.ActorNumber);

            //Other options for respawn
            //RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient};
            //PhotonNetwork.RaiseEvent(GameManager.RequestRespawnCode, null, raiseEventOptions, SendOptions.SendReliable);
            //GameManager.Instance.RequestRespawn(this);
        }
        else
        {
            //Dejar de sincronizar su posición
            Random random = new Random();
            photonView.RPC("RpcSetRagdoll", RpcTarget.All,
                ((float) random.Next(0, 100)) / 100f,
                ((float) random.Next(0, 100)) / 100f,
                ((float) random.Next(0, 100)) / 100f
            );

            //Modo observador si no (Linkar cámara a otro jugador)
            GameManager.Instance.photonView.RPC("CmdPlayerDied", RpcTarget.MasterClient,
                PhotonNetwork.LocalPlayer.ActorNumber);

            //Setup camera mode
            GameManager.Instance.photonView.RPC("CmdGetAlivePlayer", RpcTarget.MasterClient,
                PhotonNetwork.LocalPlayer.ActorNumber, 
                -1,
                null
                );
            
            //Show camera spectator GUI
            Instantiate(PlayerCameraSpectatorUIPrefab).Initialize(this);
        }
    }

    //Client side only
    [PunRPC]
    public void RpcSetRespawnPos(Vector3 respawnPos, Quaternion respawnRot)
    {
        Debug.Log("Rpc: Respawning.");
        transform.position = respawnPos;
        transform.rotation = respawnRot;
        rb.velocity = Vector3.zero;
        playerInfo.IsFalling = false;
    }

    #region Camera Methods
    
    //Client side only
    [PunRPC]
    public void RpcSetCameraDied(int _goViewId)
    {
        if (_goViewId != -1)
        {
            cameraFollow.SetPlayer(PhotonView.Find(_goViewId).GetComponent<PlayerController>());
            if(cameraFollow.GetCameraMode() == CameraFollow.CameraModeEnum.Disconnected)
                cameraFollow.SetCameraMode(CameraFollow.CameraModeEnum.Spectator);
        }
        else
        {
            cameraFollow.SetCameraMode(CameraFollow.CameraModeEnum.Disconnected);
        }
    }

    public void ChangeCameraSpectatorPlayer(bool forward)
    {
        Debug.Log("ChangeCameraSpectatorPlayer");
        
        //Setup camera mode
        GameManager.Instance.photonView.RPC("CmdGetAlivePlayer", RpcTarget.MasterClient,
            PhotonNetwork.LocalPlayer.ActorNumber, 
            cameraFollow.GetPlayer(),
            forward
            );
    }
    
    #endregion

    //Clients
    [PunRPC]
    private void RpcSetRagdoll(float torqueX, float torqueY, float torqueZ)
    {
        int strength = 10000;
        rb.constraints = RigidbodyConstraints.None;

        //No se como hacer que se comporte como un ragdoll. [HERE]
        Vector3 movementDir = rb.velocity.normalized;
        rb.AddTorque(
            movementDir[0] * strength,
            movementDir[1] * strength,
            movementDir[2] * strength
        );

        //rb.AddTorque(
        //    torqueX * strength, 
        //    torqueY * strength, 
        //    torqueZ * strength
        //    );
        //GetComponent<PhotonTransformView>().enabled = false; //Execute it for all instances
    }

    #endregion

    #region Interaction Methods
    public void ImproveSuperDash()
    {
        if (superDashImprovements < 5)
        {
            superDashImpulse += superDashIncrease;
            superDashImprovements++;
        }
    }
    #endregion
}