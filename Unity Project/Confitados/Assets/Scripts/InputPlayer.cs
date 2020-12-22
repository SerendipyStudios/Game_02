using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPlayer : MonoBehaviour
{
    public float inputX { get; private set; }
    public float inputZ { get; private set; }
    public bool dashInput { get; private set; }
    public bool superDashInput { get; private set; }

    [HideInInspector]
    public Vector3 faceDirection;

    void Start()
    {
        faceDirection = new Vector3(0.0f, 0.0f, 1.0f);
    }

    private void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
        dashInput = Input.GetButtonDown("Dash");
        superDashInput = Input.GetButtonDown("SuperDash");

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            faceDirection.x = inputX;
            faceDirection.z = inputZ;
        }
    }
}
