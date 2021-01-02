using System.Collections;
using System.Collections.Generic;
using Photon.Game;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[RequireComponent(typeof(InputPlayer))]
public class PlayerController : MonoBehaviourPunCallbacks
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
    private GameManager gameManager;

    //Animations
    [Header("Animations")] private int runHashCode;
    public float fallTime = 1f;

    //Identity variables
    //public static GameObject LocalPlayerInstance; //Needed to avoid instancing the player again when updating the scene

    //Linked objects
    [Header("Player UI")] public PlayerUI PlayerUIPrefab;
    public PlayerControlUI PlayerInputUIPrefab;

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

        //Get global references
        gameManager = FindObjectOfType<GameManager>();

        runHashCode = Animator.StringToHash("Movement");
    }

    private void Start()
    {
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
        if(executeFall)
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

    #region Methods

    private void Fall()
    {
        executeFall = false;
        
        //Desconectar cámara
        cameraFollow.SetFollowing(false);

        //Rutina de reacción
        StartCoroutine(FallResponse());
    }

    IEnumerator FallResponse()
    {
        //Espera un tiempo para que se vea la animación
        yield return new WaitForSeconds(fallTime);

        if (playerInfo.Lives > 1)
        {
            //Restar una vida
            playerInfo.Lives--;
            
            //Linkar cámara
            cameraFollow.SetFollowing(true);
            
            //Respawn
            gameManager.RequestRespawn(this);
        }
        else
        {
            //Modo observador si no (Linkar cámara a otro jugador)
            
        }
        
        playerInfo.IsFalling = false;
    }

    //Client side only
    public void SetRespawnPos(Transform respawnTransform)
    {
        Debug.Log("Rpc: Respawning.");
        transform.position = respawnTransform.position;
        transform.rotation = respawnTransform.rotation;
    }

    #endregion
}