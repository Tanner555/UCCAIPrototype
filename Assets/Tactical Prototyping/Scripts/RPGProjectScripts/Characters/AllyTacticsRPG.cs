using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;

namespace RPGPrototype
{
    public class AllyTacticsRPG : AllyTacticsController
    {
        /*
        #region Handlers
        //protected override void HandleToggleTactics(bool _enable)
        //{
        //    //Do not want to disable tactics
        //    //when freemoving or on finish moving
        //    //Only when switching from allyincommand
        //}

        //void HandleAllySwitch(PartyManager _party, AllyMember _toSet, AllyMember _current)
        //{
        //    if (allyMember.partyManager != _party) return;
            
        //    if (allyMember == _toSet &&
        //        _toSet.bIsInGeneralCommanderParty)
        //    {
        //        RPGToggleEnableTactics(false);
        //    }
        //    else
        //    {
        //        RPGToggleEnableTactics(true);
        //    }
        //}
        #endregion

        #region UnityMessages
        //Will Stop Loading Tactics Automatically on Start
        protected override void OnEnable()
        {
            bEnableTactics = false;
            SetInitialReferences();
            SubToEvents();
        }

        protected override void Start()
        {
            
        }
        #endregion

        #region Initialization
        protected override void SubToEvents()
        {
            base.SubToEvents();
            gameMaster.OnAllySwitch += HandleAllySwitch;
        }

        protected override void UnsubFromEvents()
        {
            base.UnsubFromEvents();
            gameMaster.OnAllySwitch -= HandleAllySwitch;
        }
        #endregion

        void RPGToggleEnableTactics(bool _enable)
        {
            bPreviouslyEnabledTactics = bEnableTactics;
            bEnableTactics = _enable;
            if (bEnableTactics)
            {
                LoadAndExecuteAllyTactics();
            }
            else
            {
                UnLoadAndCancelTactics();
            }
        }
        */
    }
}