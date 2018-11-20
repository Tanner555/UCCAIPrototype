using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCoreFramework
{
    /// <summary>
    /// Used To Simplify The UiTarget Registering Process.
    /// Inherit To Add Specific Functionality.
    /// </summary>
    public class RTSUITargetRegister : MonoBehaviour
    {
        #region Fields
        //UiTargetInfo
        protected AllyMember currentUiTarget = null;
        protected bool bHasRegisteredTarget = false;
        #endregion

        #region Properties
        protected RTSUiMaster uiMaster
        {
            get { return RTSUiMaster.thisInstance; }
        }

        protected RTSGameMaster gameMaster
        {
            get { return RTSGameMaster.thisInstance; }
        }

        protected AllyEventHandler uiTargetHandler
        {
            get { return currentUiTarget.allyEventHandler; }
        }
        #endregion

        #region UnityMessages
        protected virtual void OnEnable()
        {
            SubToEvents();
        }

        protected virtual void OnDisable()
        {
            UnsubFromEvents();
        }
        #endregion

        #region GameMasterHandlers/RegisterUiTarget
        protected virtual void OnRegisterUiTarget(AllyMember _target, AllyEventHandler _handler, PartyManager _party)
        {
            currentUiTarget = _target;
            bHasRegisteredTarget = true;
        }

        protected virtual void OnCheckToDeregisterUiTarget(AllyMember _target, AllyEventHandler _handler, PartyManager _party)
        {
            if (_target == currentUiTarget && bHasRegisteredTarget)
            {
                OnDeregisterUiTarget(_target, _handler);
            }
        }

        protected virtual void OnDeregisterUiTarget(AllyMember _target, AllyEventHandler _handler)
        {
            bHasRegisteredTarget = false;
        }

        #endregion

        #region Initialization
        protected virtual void SubToEvents()
        {
            gameMaster.OnRegisterUiTarget += OnRegisterUiTarget;
            gameMaster.OnDeregisterUiTarget += OnCheckToDeregisterUiTarget;
        }

        protected virtual void UnsubFromEvents()
        {
            ///Temporary Hides Error When Exiting Playmode
            if (gameMaster == null) return;
            gameMaster.OnRegisterUiTarget -= OnRegisterUiTarget;
            gameMaster.OnDeregisterUiTarget -= OnCheckToDeregisterUiTarget;

            //UnRegister From UiTarget If It Still Exists
            if(currentUiTarget != null && uiTargetHandler != null && bHasRegisteredTarget)
            {
                OnDeregisterUiTarget(currentUiTarget, uiTargetHandler);
            }
        }
        #endregion
    }
}