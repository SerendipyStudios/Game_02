using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //References
    public Transform player;

    //Camera follow
    public Vector3 offset;
    public Vector3 rotation;

    private Vector3 cameraPosition;
    private Vector3 camVel;
    private float dampTime = 0.1f;

    private Vector3 tracking;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }

    private void Update()
    {
        cameraPosition = new Vector3(player.position.x + offset.x, player.position.y + offset.y, player.position.z + offset.z);
        tracking = Vector3.SmoothDamp(transform.position, cameraPosition, ref camVel, dampTime);
    }

    private void FixedUpdate()
    { 
        transform.position = tracking;
    }

}
