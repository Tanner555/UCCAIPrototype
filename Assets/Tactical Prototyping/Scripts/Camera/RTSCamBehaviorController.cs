using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.ThirdPersonController;
using RTSCoreFramework;

namespace RTSPrototype
{
    public class RTSCamBehaviorController : MonoBehaviour
    {
        RTSGameMaster myGameMaster
        {
            get { return RTSGameMaster.thisInstance; }
        }

        RTSCameraController myCamController
        {
            get { return GetComponent<RTSCameraController>(); }
        }

        RTSCameraHandler myCamHandler
        {
            get { return GetComponent<RTSCameraHandler>(); }
        }

        RTSCameraMonitor myCamMonitor
        {
            get { return GetComponent<RTSCameraMonitor>(); }
        }

        bool partyHasInitAlly = false;

        private void OnEnable()
        {
            ToggleCamBehaviors(false);
        }

        private void OnDisable()
        {
            myGameMaster.OnAllySwitch -= OnAllySwitch;
            myGameMaster.EventAllObjectivesCompleted -= DisableCamBehaviors;
            myGameMaster.GameOverEvent -= DisableCamBehaviors;
        }

        // Use this for initialization
        void Start()
        {
            myGameMaster.OnAllySwitch += OnAllySwitch;
            myGameMaster.EventAllObjectivesCompleted += DisableCamBehaviors;
            myGameMaster.GameOverEvent += DisableCamBehaviors;
        }

        void OnAllySwitch(PartyManager _party, AllyMember _toSet, AllyMember _current)
        {
            if(partyHasInitAlly == false)
            {
                partyHasInitAlly = true;
                ToggleCamBehaviors(true);
            }
        }

        void DisableCamBehaviors()
        {
            myCamController.enabled = false;
            myCamHandler.enabled = false;
            myCamMonitor.enabled = false;
        }

        void ToggleCamBehaviors(bool _enable)
        {
            myCamController.enabled = _enable;
            myCamHandler.enabled = _enable;
            myCamMonitor.enabled = _enable;
        }
    }
}