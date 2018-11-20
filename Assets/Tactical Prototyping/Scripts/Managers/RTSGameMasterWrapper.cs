using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;
using RTSCoreFramework;
using Chronos;

namespace RTSPrototype
{
    public class RTSGameMasterWrapper : RTSGameMaster
    {       
        #region Properties
        GlobalClock allyClocks
        {
            get
            {
                if (_allyClocks == null)
                    _allyClocks = Chronos.Timekeeper.instance.Clock(allyClocksName);

                return _allyClocks;
            }
        }
        GlobalClock _allyClocks = null;

        public string allyClocksName
        {
            get { return "Allies"; }
        }

        public static new RTSGameMasterWrapper thisInstance
        {
            get { return (RTSGameMasterWrapper)RTSGameMaster.thisInstance; }
        }
        #endregion

        #region EventCalls
        protected override void ToggleGamePauseTimeScale()
        {
            if (allyClocks != null)
            {
                allyClocks.localTimeScale = bIsGamePaused ?
                    0f : 1f;
            }
            else
            {
                Debug.Log("No Ally Global Clock Exists");
            }
        }

        protected override void TogglePauseControlModeTimeScale()
        {
            if (allyClocks != null)
            {
                allyClocks.localTimeScale = bIsInPauseControlMode ?
                    0f : 1f;
            }
            else
            {
                Debug.Log("No Ally Global Clock Exists");
            }
        }
        #endregion
    }
}