using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputPlayer))]
public class PlayerController : MonoBehaviour
{
    //Movement
    public float moveSpeed;
    private Vector3 movement;
    private Vector3 dashMovement;

    //Rotation
    public float rotationSpeed;
    private Quaternion targetRotation;

    //Dashes
    public float dashSpeed;
    public float superDashSpeed;

    private bool inputControl = true;
    private bool dashing = false;
    private bool superDashing = false;
    public float dashTime;
    public float superDashTime;

    public float dashAgainCooldown;
    public float superDashAgainCooldown;
    public float dashAgainCooldownAux;
    public float superDashAgainCooldownAux;

    //References
    private Rigidbody rb;
    private InputPlayer input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<InputPlayer>();
    }

    private void Update()
    {
        if (inputControl)
        {
            movement = new Vector3(input.inputX * moveSpeed, rb.velocity.y, input.inputZ * moveSpeed);
            targetRotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(input.faceDirection), rotationSpeed * Time.deltaTime);
        }

        if (input.dashInput && !dashing)
        {
            Dash(dashTime, dashAgainCooldown);
            dashing = true;
            dashAgainCooldownAux = dashAgainCooldown;
            dashMovement = input.faceDirection * dashSpeed;
        }
        if (input.superDashInput && !superDashing)
        {
            Dash(superDashTime, superDashAgainCooldown);
            superDashing = true;
            superDashAgainCooldownAux = superDashAgainCooldown;
            dashMovement = input.faceDirection * superDashSpeed;
        }

        if(dashing)
            DashCountdown();

        if (superDashing)
            SuperDashCountdown();
    }

    private void FixedUpdate()
    {
        if (inputControl)
        {
            rb.velocity = movement;
            transform.rotation = targetRotation;
        }
        else
            rb.velocity = dashMovement;
    }

    private void Dash(float time, float coolDown)
    {
        inputControl = false;
        StartCoroutine(InputControlBack(time));
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

    IEnumerator InputControlBack(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        inputControl = true;
    }
}
