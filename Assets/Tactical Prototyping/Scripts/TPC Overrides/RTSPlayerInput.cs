using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.ThirdPersonController.Input;
using Opsive.ThirdPersonController;
using RTSCoreFramework;

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
        private void OnEnable()
        {
            if (thisInstance != null)
                Debug.Log("More than one instance of rtsplayer input exists");
            else
                thisInstance = this;

        }

        protected override void Start()
        {
            base.Start();
            gameMaster.EventHoldingRightMouseDown += DisableMouseCursor;
            gameMaster.OnAllySwitch += OnAllySwitchEnableHandler;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            gameMaster.EventHoldingRightMouseDown -= DisableMouseCursor;
            gameMaster.OnAllySwitch -= OnAllySwitchEnableHandler;
        }

        protected override void LateUpdate()
        {
            
        }

        protected override void OnMouseDown()
        {
            
        }
        #endregion

        #region Overrides
        protected override void AllowGameplayInput(bool allow)
        {
            m_AllowGameplayInput = allow;
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