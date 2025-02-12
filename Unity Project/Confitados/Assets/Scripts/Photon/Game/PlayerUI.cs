﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Game
{
    public class PlayerUI : MonoBehaviour
    {
        #region Variables

        [Tooltip("UI Text to display Player's Name")] [SerializeField]
        private Text playerNameText;


        [Tooltip("UI Slider to display Player's Health")] [SerializeField]
        private Slider playerHealthSlider;
        
        private PlayerInfo targetInfo;
        
        [Tooltip("Pixel followOffset from the player target")]
        [SerializeField]
        private Vector3 screenOffset = new Vector3(0f,30f,0f);
        
        float characterControllerHeight = 0f;
        Transform targetTransform;
        Renderer targetRenderer;
        CanvasGroup _canvasGroup;
        Vector3 targetPosition;

        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();
            this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        }

        private void Update()
        {
            // Reflect the Player Health
            if (playerHealthSlider != null)
            {
                playerHealthSlider.value = ((float)targetInfo.Lives)/3f;
            }
            
            // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
            if (targetInfo == null)
            {
                Debug.Log("Delete");
                Destroy(this.gameObject);
                return;
            }
        }
        
        void LateUpdate()
        {
            // Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
            if (targetRenderer!=null)
            {
                this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
            }
            
            // #Critical
            // Follow the Target GameObject on screen.
            if (targetTransform != null)
            {
                targetPosition = targetTransform.position;
                targetPosition.y += characterControllerHeight;
                this.transform.position = Camera.main.WorldToScreenPoint (targetPosition) + screenOffset;
            }
        }

        #endregion


        #region Public Methods

        public void SetTarget(PlayerInfo _targetInfo)
        {
            Debug.Log("HOLAAAA" +  _targetInfo.photonView.Owner.NickName);
            if (_targetInfo == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }
            
            targetInfo = _targetInfo;
            if (playerNameText != null)
            {
                playerNameText.text = targetInfo.photonView.Owner.NickName;
            }
            
            targetTransform = this.targetInfo.GetComponent<Transform>();
            targetRenderer = this.targetInfo.GetComponent<Renderer>();
            CharacterController characterController = _targetInfo.GetComponent<CharacterController> ();
            // Get data from the Player that won't change during the lifetime of this Component
            if (characterController != null)
            {
                characterControllerHeight = characterController.height;
            }
        }

        #endregion
    }
}