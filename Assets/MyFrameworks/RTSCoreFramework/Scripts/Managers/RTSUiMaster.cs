using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;

namespace RTSCoreFramework
{
    public class RTSUiMaster : UiMaster
    {
        #region DelegatesAndEvents     
        //public event MenuToggleHandler EventInventoryUIToggle;
        public event MenuToggleHandler EventIGBPIToggle;
        //IGBPI Events
        public delegate void UI_PanelHandler(IGBPI_UI_Panel _info);
        public delegate void UI_MovePanelHandler(IGBPI_UI_Panel _info, int _order);
        public event UI_PanelHandler EventRemoveDropdownInstance;
        public event UI_PanelHandler EventUIPanelSelectionChanged;
        public event UI_PanelHandler EventResetPanelUIMenu;
        public event UI_MovePanelHandler EventMovePanelUI;
        public event GeneralEventHandler EventAddDropdownInstance;
        public event GeneralEventHandler EventResetAllPaneUIMenus;
        public event GeneralEventHandler EventReorderIGBPIPanels;
        public event GeneralEventHandler EventOnSaveIGBPIComplete;
        //Character Stat Events
        public delegate void RegisterAllyToStatHandler(PartyManager _party, AllyMember _ally);
        public event RegisterAllyToStatHandler RegisterAllyToCharacterStatMonitor;
        #endregion

        #region Properties
        public RTSCamRaycaster rayCaster { get { return RTSCamRaycaster.thisInstance; } }

        //For Ui Conflict Checking
        public override bool isUiAlreadyInUse
        {
            get { return isPauseMenuOn || isIGBPIOn; }
        }
        //Override Inside Wrapper Class
        public override bool isPauseMenuOn
        {
            get { return uiManager.MenuUiPanel.activeSelf; }
        }
        public virtual bool isIGBPIOn
        {
            get { return uiManager.IGBPIUi.activeSelf; }
        }
        #endregion

        #region OverrideAndHideProperties
        new RTSGameMaster gamemaster
        {
            get { return RTSGameMaster.thisInstance; }
        }

        new public RTSUiManager uiManager
        {
            get { return RTSUiManager.thisInstance; }
        }

        new public static RTSUiMaster thisInstance
        {
            get { return UiMaster.thisInstance as RTSUiMaster; }
        }
        #endregion

        #region Fields
        public bool isDraggingIGBPI = false;
        #endregion

        #region UnityMessages
        // Use this for initialization
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        #endregion

        #region EventCalls-General/Toggles
        protected override void WaitToCallEventMenuToggle()
        {
            base.WaitToCallEventMenuToggle();
            EnableRayCaster(!isPauseMenuOn);
        }

        public void CallEventIGBPIToggle()
        {
            //If Ui Item isn't being used or IGBPI Menu is turned on
            if (isUiAlreadyInUse == false || isIGBPIOn)
            {
                CallEventAnyUIToggle(!isIGBPIOn);
                if (EventIGBPIToggle != null) EventIGBPIToggle(!isIGBPIOn);
                EnableRayCaster(!isIGBPIOn);
            }
        }

        #endregion

        #region EventCalls-IGBPI
        public void CallEventAddDropdownInstance()
        {
            if (EventAddDropdownInstance != null)
            {
                EventAddDropdownInstance();
            }
        }

        public void CallEventRemoveDropdownInstance(IGBPI_UI_Panel _info)
        {
            if (EventRemoveDropdownInstance != null)
            {
                EventRemoveDropdownInstance(_info);
            }
        }

        public void CallEventUIPanelSelectionChanged(IGBPI_UI_Panel _info)
        {
            if (EventUIPanelSelectionChanged != null)
            {
                EventUIPanelSelectionChanged(_info);
            }
        }

        public void CallEventResetPanelUIMenu(IGBPI_UI_Panel _info)
        {
            if (EventResetPanelUIMenu != null)
            {
                EventResetPanelUIMenu(_info);
            }
        }

        public void CallEventMovePanelUI(IGBPI_UI_Panel _info, int _order)
        {
            if (EventMovePanelUI != null)
                EventMovePanelUI(_info, _order);
        }

        public void CallEventResetAllPanelUIMenus()
        {
            if (EventResetAllPaneUIMenus != null)
            {
                EventResetAllPaneUIMenus();
            }
        }

        public void CallEventReorderIGBPIPanels()
        {
            if (EventReorderIGBPIPanels != null)
                EventReorderIGBPIPanels();
        }

        public void CallEventOnSaveIGBPIComplete()
        {
            if (EventOnSaveIGBPIComplete != null)
                EventOnSaveIGBPIComplete();
        }
        #endregion

        #region EventCalls-CharacterStats
        public void CallRegisterAllyToCharacterStatMonitor(PartyManager _party, AllyMember _ally)
        {
            //Only Call if PartyManager is the Current Player's General
            if (RegisterAllyToCharacterStatMonitor != null &&
                _party && _party.bIsCurrentPlayerCommander)
            {
                RegisterAllyToCharacterStatMonitor(_party, _ally);
            }
        }
        #endregion

        #region Helpers
        void EnableRayCaster(bool _enable)
        {
            if (rayCaster != null) rayCaster.enabled = _enable;
        }
        #endregion
    }
}