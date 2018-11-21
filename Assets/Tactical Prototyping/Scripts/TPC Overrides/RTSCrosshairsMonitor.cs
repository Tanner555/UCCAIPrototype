using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Opsive.UltimateCharacterController.UI;
using RTSCoreFramework;

namespace RTSPrototype
{
    public class RTSCrosshairsMonitor : CrosshairsMonitor
    {
        RTSGameMaster gameMaster
        {
            get { return RTSGameMaster.thisInstance; }
        }

        protected void Start()
        {
            gameMaster.EventHoldingRightMouseDown += DisableCrosshairsHandler;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            gameMaster.EventHoldingRightMouseDown -= DisableCrosshairsHandler;
        }

        void DisableCrosshairsHandler(bool enableCamera)
        {
            //Complete Disable Crosshairs for now
            //DisableCrosshairs(!enableCamera);
        }

        protected override void OnAttachCharacter(GameObject character)
        {
            //Complete Disable Crosshairs for now
            //base.AttachCharacter(character);
            //DisableCrosshairs(true);
            ShowCrosshairs = false;
        }
    }
}