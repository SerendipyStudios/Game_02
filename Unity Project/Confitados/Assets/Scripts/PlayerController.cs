using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputPlayer))]
public class PlayerController : MonoBehaviour
{
    //Movement
    [Header("Player speed")]
    public float moveSpeed;
    private Vector3 movement;
    private Vector3 dashDirection;

    //Rotation
    [Range(0.0f, 1.0f)]
    public float rotationSpeed;
    private Quaternion targetRotation;

    //Dashes
    private bool dashing = false;
    private bool superDashing = false;
    private bool useDash = false;
    public bool useSuperDash = false;

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
    private Rigidbody rb;
    private Animator anim;
    private InputPlayer input;

    //Animations
    private int runHashCode;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        input = GetComponent<InputPlayer>();

        runHashCode = Animator.StringToHash("Movement");
    }

    private void Start()
    {
        dashDirection = input.faceDirection;
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
        if (input.dashInput && !dashing)
        {
            useDash = true;
            dashing = true;
            dashDirection = input.faceDirection;
            dashAgainCooldownAux = dashAgainCooldown;
        }

        //If the player uses a superDash
        if (input.superDashInput && !superDashing)
        {
            useSuperDash = true;
            superDashing = true;
            dashDirection = input.faceDirection;
            superDashAgainCooldownAux = superDashAgainCooldown;
        }

        //Dashes countdowns and speed reduction
        if (dashing)
            DashCountdown();

        if (superDashing)
            SuperDashCountdown();
    }

    private void FixedUpdate()
    {
        if (useDash)
        {
            rb.AddForce(dashDirection * dashImpulse, ForceMode.Impulse);
            useDash = false;
        }
        if (useSuperDash)
        {
            rb.AddForce(dashDirection * superDashImpulse, ForceMode.Impulse);
            useSuperDash = false;
        }
        rb.AddForce(movement);        
        transform.rotation = targetRotation;
    }

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

    private void DashCountdown()
    {
        if (dashAgainCooldownAux >= 0)
            dashAgainCooldownAux -= Time.deltaTime;
        else
            dashing = false;
    }

    private void SuperDashCountdown()
    {
        if (superDashAgainCooldownAux >= 0)
            superDashAgainCooldownAux -= Time.deltaTime;
        else
            superDashing = false;
    }
}
