using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public enum Platforms { PC, Mobile }

public class InputPlayer : MonoBehaviourPunCallbacks
{
    //public Platforms platform;

    private Joystick joystick;
    public float inputX{ get; private set; }
    public float inputZ{ get; private set; }
    public float inputX_RAW { get; private set; }
    public float inputZ_RAW { get; private set; }
    public bool dashInput { get; private set; }
    public bool superDashInput { get; private set; }

    private bool dashMobile, superDashMobile; //Mobile phone dash inputs

    [HideInInspector]
    public Vector3 faceDirection;

    void Start()
    {
        if (!photonView.IsMine) this.enabled = false;
        faceDirection = new Vector3(0.0f, 0.0f, 1.0f);
    }

    public void Initialize(PlayerControlUI playerControlUi)
    {
        joystick = playerControlUi.joystick;
        playerControlUi.dashButton.onClick.AddListener(this.Dash_MobileInput);
        playerControlUi.superDashButton.onClick.AddListener(this.SuperDash_MobileInput);
    }

    public void Dash_MobileInput() => StartCoroutine(UseDashMobile()); //Methods to use dashes with mobile controls
    public void SuperDash_MobileInput() => StartCoroutine(UseSuperDashMobile());

    private void Update()
    {
        switch (SystemInfo.deviceType)
        {
            case  (DeviceType.Desktop):
                //Inputs check
                inputX = Input.GetAxis("Horizontal");
                inputZ = Input.GetAxis("Vertical");
                inputX_RAW = Input.GetAxisRaw("Horizontal");
                inputZ_RAW = Input.GetAxisRaw("Vertical");
                dashInput = Input.GetButtonDown("Dash");
                superDashInput = Input.GetButtonDown("SuperDash");

                //Face direction update
                if (inputX_RAW != 0 || inputZ_RAW != 0)
                {
                    faceDirection.x = inputX_RAW;
                    faceDirection.z = inputZ_RAW;
                }
                break;

            //case Platforms.Mobile:
            case (DeviceType.Handheld):
                //Inputs check
                inputX = joystick.Horizontal;
                inputZ = joystick.Vertical;
                inputX_RAW = joystick.Horizontal;
                inputZ_RAW = joystick.Vertical;
                if (joystick.SnapX && joystick.SnapY)
                {
                    inputX_RAW = joystick.Horizontal;
                    inputZ_RAW = joystick.Vertical;
                }

                dashInput = dashMobile;
                superDashInput = superDashMobile;

                //Face direction update
                joystick.SnapX = true;
                joystick.SnapY = true;

                if (inputX_RAW != 0 || inputZ_RAW != 0)
                {
                    faceDirection.x = joystick.Horizontal;
                    faceDirection.z = joystick.Vertical;
                    joystick.SnapX = false;
                    joystick.SnapY = false;
                }
                break;
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
