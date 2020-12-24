using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Platforms { PC, Mobile }

public class InputPlayer : MonoBehaviour
{

    public Platforms platform;

    public Joystick joystick;
    public float inputX; //{ get; private set; }
    public float inputZ; //{ get; private set; }
    public bool dashInput { get; private set; }
    public bool superDashInput { get; private set; }

    private bool dashMobile, superDashMobile; //Mobile phone dash inputs

    //[HideInInspector]
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
                //Inputs check
                inputX = Input.GetAxis("Horizontal");
                inputZ = Input.GetAxis("Vertical");
                dashInput = Input.GetButtonDown("Dash");
                superDashInput = Input.GetButtonDown("SuperDash");

                //Face direction update
                if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                {
                    faceDirection.x = Input.GetAxisRaw("Horizontal");
                    faceDirection.z = Input.GetAxisRaw("Vertical");
                }

                break;

            case Platforms.Mobile:
                //Inputs check
                inputX = joystick.Horizontal;
                inputZ = joystick.Vertical;
                dashInput = dashMobile;
                superDashInput = superDashMobile;

                //Face direction update
                joystick.SnapX = true;
                joystick.SnapY = true;
                if (joystick.Horizontal != 0 || joystick.Vertical != 0)
                {
                    faceDirection.x = joystick.Horizontal;
                    faceDirection.z = joystick.Vertical;
                    joystick.SnapX = false;
                    joystick.SnapY = false;
                }
                break;
        }


    }

    private float FaceSideRaw(float value)
    {
        if (value > 0)
            return 1;
        else if (value < 0)
            return -1;
        else
            return 0;
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
