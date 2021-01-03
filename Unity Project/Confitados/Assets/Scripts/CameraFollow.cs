using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun.Demo.PunBasics;
using UnityEditor;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Variables
    
    //References
    public PlayerController player;
    
    //State
    public enum CameraModeEnum
    {
        Disconnected = 0,
        Following = 1,
        Spectator = 2,
        Win = 3,
    }
    [SerializeField] private CameraModeEnum cameraMode = CameraModeEnum.Disconnected;

    //Camera variables
    public Vector3 rotation;
    
    //Camera follow
    public Vector3 followOffset = new Vector3(0, 15, -14);
    public Vector3 winOffset = new Vector3(0, 3, 7);

    //Transitions
    public float winTransitionSpeed = 10;
    private float deltaTime;

    //Script variables
    private Vector3 cameraPosition;
    private Vector3 trackingPosition;
    private Vector3 camVel;
    private readonly float dampTime = 0.001f;
    
    #endregion    
    
    #region Unity Callbacks

    private void Start()
    {
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }
    
    private void Update()
    {
        deltaTime = Time.deltaTime;
        
        switch (cameraMode)
        {
            case CameraModeEnum.Disconnected:
                break;
            case CameraModeEnum.Following:
                UpdateFollow(followOffset);
                break;
            case CameraModeEnum.Spectator:
                UpdateFollow(followOffset);
                break;
            case CameraModeEnum.Win:
                LerpToCloseUp(winOffset);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void FixedUpdate()
    {
        switch (cameraMode)
        {
            case CameraModeEnum.Disconnected:
                break;
            case CameraModeEnum.Following:
                transform.position = trackingPosition;
                break;
            case CameraModeEnum.Spectator:
                transform.position = trackingPosition;
                break;
            case CameraModeEnum.Win:
                transform.position = trackingPosition;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    #endregion
    
    #region Getters & Setters

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

    public int GetPlayer()
    {
        return player.photonView.Owner.ActorNumber;
    }

    public void SetCameraMode(CameraModeEnum _cameraMode)
    {
        cameraMode = _cameraMode;

        switch (cameraMode)
        {
            case CameraModeEnum.Disconnected:
                break;
            case CameraModeEnum.Following:
                TeleportCamera(followOffset);
                break;
            case CameraModeEnum.Spectator:
                TeleportCamera(followOffset);
                break;
            case CameraModeEnum.Win:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public CameraModeEnum GetCameraMode()
    {
        return cameraMode;
    }
    
    #endregion

    #region Update Camera Methods
    
    private void UpdateFollow(Vector3 offset)
    {
        //Position
        var position = player.transform.position;
        cameraPosition = new Vector3(position.x + offset.x, position.y + offset.y, position.z + offset.z);
        trackingPosition = Vector3.SmoothDamp(transform.position, cameraPosition, ref camVel, dampTime);

        //Look at player
        transform.LookAt(position);
    }

    private void TeleportCamera(Vector3 offset)
    {
        var position = player.transform.position;
        transform.position = new Vector3(position.x + offset.x, position.y + offset.y, position.z + offset.z);
    }

    private void LerpToCloseUp(Vector3 offset)
    {
        //Lerp position
        Vector3 target = player.transform.TransformPoint(offset);
        trackingPosition = Vector3.Lerp(
            transform.position,
            target,
            deltaTime * winTransitionSpeed
        );
        
        //Look at player
        transform.LookAt(player.transform.position);
    }

    #endregion
}
