using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;
using Opsive.UltimateCharacterController;
using Opsive.UltimateCharacterController.Input;

namespace RTSPrototype
{
    public class RTSPlayerInput : UnityInput
    {
        #region RTSFieldsAndProps
        public static RTSPlayerInput thisInstance { get; protected set; }

        RTSGameMaster gameMaster
        {
            get { return RTSGameMaster.thisInstance; }
        }

        //Used for input
        bool isMovingCamera = false;
        #endregion

        #region UnityMessages
        protected override void OnEnable()
        {
            base.OnEnable();

            if (thisInstance != null)
                Debug.Log("More than one instance of rtsplayer input exists");
            else
                thisInstance = this;

        }

        protected void Start()
        {
            gameMaster.EventHoldingRightMouseDown += DisableMouseCursor;
            gameMaster.OnAllySwitch += OnAllySwitchEnableHandler;
        }

        protected void OnDisable()
        {
            gameMaster.EventHoldingRightMouseDown -= DisableMouseCursor;
            gameMaster.OnAllySwitch -= OnAllySwitchEnableHandler;
        }

        //protected void LateUpdate()
        //{
            
        //}

        //protected void OnMouseDown()
        //{
            
        //}
        #endregion

        #region Overrides
        protected override void EnableGameplayInput(bool allow)
        {
            m_AllowInput = allow;
        }

        #endregion

        #region RTSHandlers
        void DisableMouseCursor(bool disable)
        {
            Cursor.lockState = (disable ? CursorLockMode.Locked : CursorLockMode.None);
            Cursor.visible = !disable;
            isMovingCamera = disable;
        }

        void OnAllySwitchEnableHandler(PartyManager _party, AllyMember _toSet, AllyMember _current)
        {
            DisableMouseCursor(false);
        }
        #endregion
    }
}