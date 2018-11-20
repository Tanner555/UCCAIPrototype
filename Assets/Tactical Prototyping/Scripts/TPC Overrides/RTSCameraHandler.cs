using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.ThirdPersonController;
using Opsive.ThirdPersonController.Input;
using RTSCoreFramework;

namespace RTSPrototype
{
    public class RTSCameraHandler : CameraHandler
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

        protected override void InitializeCharacter(GameObject character)
        {
            if (character == null)
            {
                if (m_Character != null)
                {
                    EventHandler.UnregisterEvent<bool>(m_Character, "OnAllowGameplayInput", AllowGameplayInput);
                    EventHandler.UnregisterEvent<Item>(m_Character, "OnInventoryDualWieldItemChange", OnDualWieldItemChange);
                    m_Character = null;
                    m_PlayerInput = null;
                }
                return;
            }

            m_Character = character;
            m_PlayerInput = RTSPlayerInput.thisInstance;
            EventHandler.RegisterEvent<bool>(m_Character, "OnAllowGameplayInput", AllowGameplayInput);
            EventHandler.RegisterEvent<Item>(m_Character, "OnInventoryDualWieldItemChange", OnDualWieldItemChange);
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
            if (zoomCamera)
            {
                m_StepZoom = zoomAxisIsPositive ? 0.1f : -0.1f;
            }
            else
            {
                m_StepZoom = 0.0f;
            }
        }
        #endregion



    }
}