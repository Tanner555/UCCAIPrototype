using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;
using RTSCoreFramework;

namespace RTSPrototype
{
    public class MyFlowCanvasWrappers : MonoBehaviour
    {
        #region NoneAccessProperties
        private RTSGameMasterWrapper gamemaster
        {
            get { return RTSGameMasterWrapper.thisInstance; }
        }

        private RTSGameModeWrapper gamemode
        {
            get { return RTSGameModeWrapper.thisInstance; }
        }
        #endregion

        #region NoneAccessEventCalls
        private void CallOnLeftClickAlly(AllyMember _ally)
        {
            if (OnLeftClickAlly != null) OnLeftClickAlly(_ally);
        }

        private void CallOnAllySwitch(PartyManager _party, AllyMember _toSet, AllyMember _current)
        {
            if (OnAllySwitch != null) OnAllySwitch(_party, _toSet, _current);
        }
        #endregion

        #region EventWrappers
        public event RTSGameMasterWrapper.AllyMemberHandler OnLeftClickAlly;
        public event RTSGameMasterWrapper.AllySwitchHandler OnAllySwitch;
        #endregion

        #region UnityMessages
        private void OnEnable()
        {
            SubToEvents();
        }

        private void OnDisable()
        {
            UnsubFromEvents();
        }
        #endregion

        #region PublicMethodCalls
        public void SayHello(string _msg)
        {
            Debug.Log($"Hello {_msg}");
        }

        #endregion

        #region Init
        void SubToEvents()
        {
            gamemaster.OnLeftClickAlly += CallOnLeftClickAlly;
            gamemaster.OnAllySwitch += CallOnAllySwitch;
        }

        void UnsubFromEvents()
        {
            gamemaster.OnLeftClickAlly -= CallOnLeftClickAlly;
            gamemaster.OnAllySwitch -= CallOnAllySwitch;
        }
        #endregion
    }
}