using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController;
using Opsive.UltimateCharacterController.Camera;
using Opsive.UltimateCharacterController.Input;
using RTSCoreFramework;
using Opsive.UltimateCharacterController.Game;
using uccEventHelper = UtilitiesAndHelpersForUCC.UCCEventsControllerUtility;

namespace RTSPrototype
{
    public class RTSCameraHandler : CameraControllerHandler
    {
        #region RTSFieldsAndProps
        RTSGameMaster gamemaster
        {
            get { return RTSGameMaster.thisInstance; }
        }

        //Used To Store Current Look Vector
        Vector2 currentLookVector = Vector2.zero;
        //Limits Mouse Movement
        private bool moveCamera = false;
        //Zooming
        private bool zoomCamera = false;
        private bool zoomAxisIsPositive = false;
        #endregion

        #region RTSHandlers
        void ToggleMoveCamera(bool enable)
        {
            moveCamera = enable;
        }

        void ToggleZoomCamera(bool enable, bool isPositive)
        {
            zoomCamera = enable;
            zoomAxisIsPositive = isPositive;
        }
        #endregion

        protected override void OnAttachCharacter(GameObject character)
        {
            enabled = character != null;

            if (m_Character != null)
            {
                uccEventHelper.UnregisterOnDeath(m_Character, OnDeath);
                uccEventHelper.UnregisterOnRespawn(m_Character, OnRespawn);
                uccEventHelper.UnregisterOnEnableGameplayInput(m_Character, OnEnableGameplayInput);
                uccEventHelper.UnregisterOnCharacterActivate(m_Character, OnActivate);
            }

            m_Character = character;

            if (character != null)
            {
                uccEventHelper.RegisterOnDeath(character, OnDeath);
                uccEventHelper.RegisterOnRespawn(character, OnRespawn);
                uccEventHelper.RegisterOnEnableGameplayInput(character, OnEnableGameplayInput);
                uccEventHelper.RegisterOnCharacterActivate(character, OnActivate);

                m_AllowGameplayInput = true;
                enabled = character.activeInHierarchy;
                if (enabled)
                {
                    m_PlayerInput = RTSPlayerInput.thisInstance;
                }
            }

            //Update Look Vector When Setting Player Ally In Command
            currentLookVector = m_PlayerInput.GetLookVector(true);
        }

        #region UnityMessages
        protected void OnDisable()
        {
            //Temp Fix For Party Man Delay Init Methods
            gamemaster.EventHoldingRightMouseDown -= ToggleMoveCamera;
            gamemaster.EventEnableCameraZoom -= ToggleZoomCamera;
        }

        private void OnEnable()
        {
            gamemaster.EventHoldingRightMouseDown += ToggleMoveCamera;
            gamemaster.EventEnableCameraZoom += ToggleZoomCamera;
        }

        protected void Start()
        {
            
        }

        protected override void FixedUpdate()
        {
            if (moveCamera)
            {
                //Modify FixedUpdate Functionality To Stop Momentum 
                //When Letting Go Of Right Mouse Button
                currentLookVector = m_PlayerInput.GetLookVector(true);
                KinematicObjectManager.SetCameraLookVector(m_CameraController.KinematicObjectIndex, currentLookVector);
            }
            else
            {
                KinematicObjectManager.SetCameraLookVector(m_CameraController.KinematicObjectIndex, Vector2.zero);
            }
        }
        #endregion



    }
}