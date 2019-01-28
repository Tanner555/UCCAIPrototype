using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public class UiMaster : BaseSingleton<UiMaster>
    {
        #region DelegatesAndEvents
        public delegate void GeneralEventHandler();
        public delegate void MenuToggleHandler(bool enable);
        public event MenuToggleHandler EventMenuToggle;
        public event MenuToggleHandler EventAnyUIToggle;
        #endregion

        #region EventCalls-General/Toggles
        public virtual void CallEventMenuToggle()
        {
            //If Ui Item isn't being used or Pause Menu is turned on
            if (isUiAlreadyInUse == false || isPauseMenuOn)
                WaitToCallEventMenuToggle();
        }

        protected virtual void WaitToCallEventMenuToggle()
        {
            CallEventAnyUIToggle(!isPauseMenuOn);
            if (EventMenuToggle != null) EventMenuToggle(!isPauseMenuOn);
        }

        protected virtual void CallEventAnyUIToggle(bool _enabled)
        {
            if (EventAnyUIToggle != null) EventAnyUIToggle(_enabled);
        }
        #endregion

        #region Properties
        //For Ui Conflict Checking
        public virtual bool isUiAlreadyInUse
        {
            get { return isPauseMenuOn; }
        }
        //Override Inside Wrapper Class
        public virtual bool isPauseMenuOn
        {
            get { return uiManager.MenuUiPanel.activeSelf; }
        }

        public UiManager uiManager
        {
            get { return UiManager.thisInstance; }
        }

        protected GameMaster gamemaster
        {
            get { return GameMaster.thisInstance; }
        }
        #endregion

        #region UnityMessages
        protected virtual void OnEnable()
        {

        }

        protected virtual void Start()
        {
            
        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }

        protected virtual void OnDisable()
        {
            
        }
        #endregion
    }
}