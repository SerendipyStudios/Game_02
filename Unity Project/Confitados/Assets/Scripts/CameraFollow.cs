using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //References
    public PlayerController player;
    
    //State
    public bool isFollowing = false;

    //Camera follow
    public Vector3 offset;
    public Vector3 rotation;

    private Vector3 cameraPosition;
    private Vector3 camVel;
    private readonly float dampTime = 0.001f;

    private Vector3 tracking;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }

    public void Initialize(PlayerController player)
    {
        this.player = player;
        SetFollowing(true);
    }

    private void Update()
    {
        if (!isFollowing) return;
        
        var position = player.transform.position;
        cameraPosition = new Vector3(position.x + offset.x, position.y + offset.y, position.z + offset.z);
        tracking = Vector3.SmoothDamp(transform.position, cameraPosition, ref camVel, dampTime);
    }

    private void FixedUpdate()
    {
        if (!isFollowing) return;
        
        transform.position = tracking;
    }
    
    #region Methods

    public void SetFollowing(bool _isFollowing)
    {
        isFollowing = _isFollowing;
        
        if(isFollowing)
        {
            var position = player.transform.position;
            transform.position = new Vector3(position.x + offset.x, position.y + offset.y, position.z + offset.z);
        }
    }
    
    #endregion

}
