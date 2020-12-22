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
    public float dashTime;
    public float superDashTime;

    private float currentCooldown;
    public float dashAgainCooldown;
    public float superDashAgainCooldown;

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

        if (!dashing)
        {
            if (input.dashInput)
            {
                Dash(dashTime, dashAgainCooldown);
                dashMovement = input.faceDirection * dashSpeed;
            }
            else if (input.superDashInput)
            {
                Dash(superDashTime, superDashAgainCooldown);
                dashMovement = input.faceDirection * superDashSpeed;
            }
        }
        else
            DashCountdown();
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
        currentCooldown = coolDown;
        dashing = true;
        inputControl = false;
        StartCoroutine(InputControlBack(time));
    }

    private void DashCountdown()
    {
        if (currentCooldown >= 0)
            currentCooldown -= Time.deltaTime;
        else
            dashing = false;
    }

    IEnumerator InputControlBack(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        inputControl = true;
    }
}
