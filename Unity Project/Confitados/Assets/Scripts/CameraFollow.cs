using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //References
    public PlayerController player;
    
    //State
    public enum CameraModeEnum
    {
        Disconnected = 0,
        Following = 1,
        Spectator = 2,
    }

    [SerializeField] private CameraModeEnum cameraMode = CameraModeEnum.Disconnected;

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
        SetCameraMode(CameraModeEnum.Following);
    }
    
    public void SetPlayer(PlayerController player)
    {
        Debug.Log("Changing focus to " + player.photonView.Owner.NickName);
        this.player = player;
    }

    private void Update()
    {
        switch (cameraMode)
        {
            case CameraModeEnum.Disconnected:
                break;
            case CameraModeEnum.Following:
                UpdateFollow();
                break;
            case CameraModeEnum.Spectator:
                UpdateFollow();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void FixedUpdate()
    {
        if (cameraMode == CameraModeEnum.Disconnected) return;
        
        transform.position = tracking;
    }
    
    #region Methods

    public void SetCameraMode(CameraModeEnum _cameraMode)
    {
        cameraMode = _cameraMode;

        switch (cameraMode)
        {
            case CameraModeEnum.Disconnected:
                break;
            case CameraModeEnum.Following:
                TeleportCamera();
                break;
            case CameraModeEnum.Spectator:
                TeleportCamera();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateFollow()
    {
        var position = player.transform.position;
        cameraPosition = new Vector3(position.x + offset.x, position.y + offset.y, position.z + offset.z);
        tracking = Vector3.SmoothDamp(transform.position, cameraPosition, ref camVel, dampTime);
    }

    private void TeleportCamera()
    {
        var position = player.transform.position;
        transform.position = new Vector3(position.x + offset.x, position.y + offset.y, position.z + offset.z);
    }

    #endregion

}
