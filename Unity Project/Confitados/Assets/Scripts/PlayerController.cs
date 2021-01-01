using System.Collections;
using System.Collections.Generic;
using Photon.Game;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(InputPlayer))]
public class PlayerController : MonoBehaviourPunCallbacks
{
    #region Variables
    
    //Movement
    [Header("Player speed")]
    public float moveSpeed;
    private Vector3 movement;
    private Vector3 dashDirection;

    //Rotation
    [Range(0.0f, 1.0f)]
    [SerializeField] private float rotationSpeed;
    private Quaternion targetRotation;

    //Dash flag
    private bool executeDash = false;
    private bool executeSuperDash = false;

    [Header("Dashes impulse")]
    public float dashImpulse;
    public float superDashImpulse;

    [Header("Dashes cooldowns")]
    public float dashAgainCooldown;
    public float superDashAgainCooldown;
    private float dashAgainCooldownAux;
    private float superDashAgainCooldownAux;

    //Surfaces interaction
    [Header("Surfaces interaction")]
    public float iceDrag;
    public float stickyDrag;

    //References
    [HideInInspector]
    public Rigidbody rb;
    private Animator anim;
    private InputPlayer input;
    private PlayerInfo playerInfo;

    //Animations
    private int runHashCode;
    
    //Identity variables
    //public static GameObject LocalPlayerInstance; //Needed to avoid instancing the player again when updating the scene

    //Linked objects
    [Header("Player UI")]
    public PlayerUI PlayerUIPrefab; 
    public PlayerControlUI PlayerInputUIPrefab; 
    
    [Header("Sound")]
    public SoundManager SoundManagerPrefab;

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
            PlayerUI _uiGo =  Instantiate(PlayerUIPrefab);
            _uiGo.SendMessage ("SetTarget", playerInfo, SendMessageOptions.RequireReceiver);
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

        runHashCode = Animator.StringToHash("Movement");
    }

    private void Start()
    {
        //Setup movement
        dashDirection = input.faceDirection;
        
        //Set camera follow
        if (Camera.main != null) Camera.main.gameObject.GetComponent<CameraFollow>().player = this.gameObject.transform;

        //Instantiate Controls
        if (PlayerInputUIPrefab != null)
        {
            PlayerControlUI _inputUiGo =  Instantiate(PlayerInputUIPrefab);
            input.Initialize(_inputUiGo.GetComponent<PlayerControlUI>());
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerInputUIPrefab reference on player Prefab.", this);
        }
        
        //Instantiate SoundManager
        if (SoundManagerPrefab != null)
        {
            SoundManager _soundManager =  Instantiate(SoundManagerPrefab);
            _soundManager.Initialize(this);
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
        targetRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(input.faceDirection), rotationSpeed);

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
    }

    private void FixedUpdate()
    {
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
        if (other.gameObject.tag == "IceFloor")
            rb.drag = iceDrag;
        if (other.gameObject.tag == "StickyFloor")
            rb.drag = stickyDrag;
    }

    void OnTriggerExit(Collider other)
    {
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
}
