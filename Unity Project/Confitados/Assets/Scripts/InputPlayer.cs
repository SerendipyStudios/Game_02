using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Platforms { PC, Mobile }

public class InputPlayer : MonoBehaviour
{

    public Platforms platform;

    public Joystick joystick;
    public float inputX { get; private set; }
    public float inputZ { get; private set; }
    public bool dashInput { get; private set; }
    public bool superDashInput { get; private set; }

    private bool dashMobile, superDashMobile; //Mobile phone dash inputs

[HideInInspector]
    public Vector3 faceDirection;

    void Start()
    {
        faceDirection = new Vector3(0.0f, 0.0f, 1.0f);
    }

    public void Dash_MobileInput() => StartCoroutine(UseDashMobile()); //Methods to use dashes with mobile controls
    public void SuperDash_MobileInput() => StartCoroutine(UseSuperDashMobile());

    private void Update()
    {
        switch (platform)
        {
            case Platforms.PC:
                inputX = Input.GetAxis("Horizontal");
                inputZ = Input.GetAxis("Vertical");
                dashInput = Input.GetButtonDown("Dash");
                superDashInput = Input.GetButtonDown("SuperDash");
                break;

            case Platforms.Mobile:
                inputX = joystick.Horizontal;
                inputZ = joystick.Vertical;
                dashInput = dashMobile;
                superDashInput = superDashMobile;
                break;
        }

        if (inputX != 0 || inputZ != 0)
        {
            faceDirection.x = inputX;
            faceDirection.z = inputZ;
        }
    }

    IEnumerator UseDashMobile()
    {
        dashMobile = true;
        yield return new WaitForEndOfFrame();
        dashMobile = false;
    }

    IEnumerator UseSuperDashMobile()
    {
        superDashMobile = true;
        yield return new WaitForEndOfFrame();
        superDashMobile = false;
    }
}
