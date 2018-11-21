using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController;
using Opsive.UltimateCharacterController.Camera;
using Opsive.UltimateCharacterController.Input;
using RTSCoreFramework;
using Opsive.UltimateCharacterController.Events;

namespace RTSPrototype
{
    public class RTSCameraHandler : CameraControllerHandler
    {
        #region RTSFieldsAndProps
        RTSGameMaster gamemaster
        {
            get { return RTSGameMaster.thisInstance; }
        }

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
                EventHandler.UnregisterEvent<Vector3, Vector3, GameObject>(m_Character, "OnDeath", OnDeath);
                EventHandler.UnregisterEvent(m_Character, "OnRespawn", OnRespawn);
                EventHandler.UnregisterEvent<bool>(m_Character, "OnEnableGameplayInput", OnEnableGameplayInput);
                EventHandler.UnregisterEvent<bool>(m_Character, "OnCharacterActivate", OnActivate);
            }

            m_Character = character;

            if (character != null)
            {
                EventHandler.RegisterEvent<Vector3, Vector3, GameObject>(character, "OnDeath", OnDeath);
                EventHandler.RegisterEvent(character, "OnRespawn", OnRespawn);
                EventHandler.RegisterEvent<bool>(character, "OnEnableGameplayInput", OnEnableGameplayInput);
                EventHandler.RegisterEvent<bool>(character, "OnCharacterActivate", OnActivate);
                m_AllowGameplayInput = true;
                enabled = character.activeInHierarchy;
                if (enabled)
                {
                    m_PlayerInput = RTSPlayerInput.thisInstance;
                }
            }
        }

        #region UnityMessages
        protected void OnDisable()
        {
            //Temp Fix For Party Man Delay Init Methods
            //gamemaster.EventHoldingRightMouseDown -= ToggleMoveCamera;
            gamemaster.EventEnableCameraZoom -= ToggleZoomCamera;
        }

        private void OnEnable()
        {
            
        }

        protected void Start()
        {
            gamemaster.EventHoldingRightMouseDown += ToggleMoveCamera;
            gamemaster.EventEnableCameraZoom += ToggleZoomCamera;
        }

        protected override void Update()
        {
            if (moveCamera)
            {
                base.Update();
            }
            //TODO: RTSPrototype Implement Zoom Functionality
            //if (zoomCamera)
            //{
            //    m_StepZoom = zoomAxisIsPositive ? 0.1f : -0.1f;
            //}
            //else
            //{
            //    m_StepZoom = 0.0f;
            //}
        }
        #endregion



    }
}