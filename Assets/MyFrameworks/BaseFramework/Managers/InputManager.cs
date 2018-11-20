using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public class InputManager : MonoBehaviour
    {
        #region Properties
        //Time Properties
        protected float CurrentGameTime
        {
            get { return Time.unscaledTime; }
        }

        //Mouse Setup - Scrolling
        protected bool bScrollAxisIsPositive
        {
            get { return scrollInputAxisValue >= 0.0f; }
        }

        protected UiMaster uiMaster { get { return UiMaster.thisInstance; } }

        protected UiManager uiManager
        {
            get { return UiManager.thisInstance; }
        }

        protected GameMode gamemode
        {
            get { return GameMode.thisInstance; }
        }

        protected GameMaster gamemaster
        {
            get { return GameMaster.thisInstance; }
        }

        public static InputManager thisInstance
        {
            get; protected set;
        }
        #endregion

        #region Fields
        //Handles Right Mouse Down Input
        [Header("Right Mouse Down Config")]
        public float RMHeldThreshold = 0.15f;
        protected bool isRMHeldDown = false;
        protected bool isRMHeldPastThreshold = false;
        protected float RMCurrentTimer = 5f;
        //Handles Left Mouse Down Input
        [Header("Left Mouse Down Config")]
        public float LMHeldThreshold = 0.15f;
        protected bool isLMHeldDown = false;
        protected bool isLMHeldPastThreshold = false;
        protected float LMCurrentTimer = 5f;
        //Handles Mouse ScrollWheel Input
        //Scroll Input
        protected string scrollInputName = "Mouse ScrollWheel";
        protected float scrollInputAxisValue = 0.0f;
        protected bool bScrollWasPreviouslyPositive = false;
        protected bool bScrollIsCurrentlyPositive = false;
        //Scroll Timer Handling
        protected bool isScrolling = false;
        //Used to Fix First Scroll Not Working Issue
        protected bool bBeganScrolling = false;
        //Stop Scroll Functionality
        [Header("Mouse ScrollWheel Config")]
        public float scrollStoppedThreshold = 0.15f;
        protected bool isNotScrollingPastThreshold = false;
        protected float noScrollCurrentTimer = 5f;
        //UI is enabled
        protected bool UiIsEnabled = false;
        //Number Key Input
        protected List<int> NumberKeys = new List<int>
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9
        };
        protected List<string> NumberKeyNames = new List<string>
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
        };
        #endregion

        #region UnityMessages
        protected virtual void OnEnable()
        {
            if (thisInstance != null)
                Debug.LogWarning("More than one instance of InputManager in scene.");
            else
                thisInstance = this;
        }

        protected virtual void Start()
        {
            SubToEvents();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            InputSetup();
            LeftMouseDownSetup();
            RightMouseDownSetup();
            StopMouseScrollWheelSetup();
        }

        protected virtual void OnDisable()
        {
            UnsubFromEvents();
        }
        #endregion

        #region InputSetup
        protected virtual void InputSetup()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CallMenuToggle();
            }
            foreach (int _key in NumberKeys)
            {
                if (Input.GetKeyDown(_key.ToString()))
                {
                    CallOnNumberKeyPress(_key);
                }
            }
        }

        #endregion

        #region MouseSetup
        void LeftMouseDownSetup()
        {
            if (UiIsEnabled) return;
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (isRMHeldDown) return;
                if (isLMHeldDown == false)
                {
                    isLMHeldDown = true;
                    LMCurrentTimer = CurrentGameTime + LMHeldThreshold;
                }

                if (CurrentGameTime > LMCurrentTimer)
                {
                    //Calls Every Update
                    //CreateSelectionSquare();
                    if (isLMHeldPastThreshold == false)
                    {
                        //OnMouseDown Code Goes Here
                        isLMHeldPastThreshold = true;
                        gamemaster.CallEventHoldingLeftMouseDown(true);
                    }
                }
            }
            else
            {
                if (isLMHeldDown == true)
                {
                    isLMHeldDown = false;
                    if (isLMHeldPastThreshold == true)
                    {
                        //When MouseDown Code Exits
                        isLMHeldPastThreshold = false;
                        gamemaster.CallEventHoldingLeftMouseDown(false);
                    }
                    else
                    {
                        //Mouse Button Was Let Go Before the Threshold
                        //Call the Click Event
                        gamemaster.CallEventOnLeftClick();
                    }
                }
            }
        }

        void RightMouseDownSetup()
        {
            if (UiIsEnabled) return;
            if (Input.GetKey(KeyCode.Mouse1))
            {
                if (isLMHeldDown) return;
                if (isRMHeldDown == false)
                {
                    isRMHeldDown = true;
                    RMCurrentTimer = CurrentGameTime + RMHeldThreshold;
                }

                if (CurrentGameTime > RMCurrentTimer)
                {
                    if (isRMHeldPastThreshold == false)
                    {
                        //OnMouseDown Code Goes Here
                        isRMHeldPastThreshold = true;
                        gamemaster.CallEventHoldingRightMouseDown(true);
                    }
                }
            }
            else
            {
                if (isRMHeldDown == true)
                {
                    isRMHeldDown = false;
                    if (isRMHeldPastThreshold == true)
                    {
                        //When MouseDown Code Exits
                        isRMHeldPastThreshold = false;
                        gamemaster.CallEventHoldingRightMouseDown(false);
                    }
                    else
                    {
                        //Mouse Button Was Let Go Before the Threshold
                        //Call the Click Event
                        gamemaster.CallEventOnRightClick();
                    }
                }
            }

        }

        void StopMouseScrollWheelSetup()
        {
            if (UiIsEnabled) return;
            scrollInputAxisValue = Input.GetAxis(scrollInputName);
            if (Mathf.Abs(scrollInputAxisValue) > 0.0f)
            {
                if (isLMHeldDown) return;
                bScrollIsCurrentlyPositive = bScrollAxisIsPositive;

                //Fixes First Scroll Not Working Issue
                if (bBeganScrolling == false)
                {
                    bBeganScrolling = true;
                    gamemaster.CallEventEnableCameraZoom(true, bScrollAxisIsPositive);
                }

                if (bScrollWasPreviouslyPositive != bScrollIsCurrentlyPositive)
                {
                    gamemaster.CallEventEnableCameraZoom(true, bScrollAxisIsPositive);
                    bScrollWasPreviouslyPositive = bScrollAxisIsPositive;
                }

                if (isScrolling == false)
                {
                    isScrolling = true;
                    if (isNotScrollingPastThreshold == true)
                    {
                        //When ScrollWheel Code Starts
                        isNotScrollingPastThreshold = false;
                        gamemaster.CallEventEnableCameraZoom(true, bScrollAxisIsPositive);
                        bScrollWasPreviouslyPositive = bScrollAxisIsPositive;
                    }
                    else
                    {
                        //Scroll Wheel Started Before the Stop Threshold
                        //Do nothing for now
                    }
                }
            }
            else
            {
                if (isScrolling == true)
                {
                    isScrolling = false;
                    noScrollCurrentTimer = CurrentGameTime + scrollStoppedThreshold;
                }

                if (CurrentGameTime > noScrollCurrentTimer)
                {
                    if (isNotScrollingPastThreshold == false)
                    {
                        //OnScrollWheel Stopping Code Goes Here
                        isNotScrollingPastThreshold = true;
                        gamemaster.CallEventEnableCameraZoom(false, bScrollAxisIsPositive);
                    }
                }
            }
        }

        #endregion

        #region Handlers
        protected virtual void HandleGamePaused(bool _isPaused)
        {
            if (_isPaused)
            {
                ResetMouseSetup();
            }
        }

        protected virtual void HandleUiActiveSelf(bool _state)
        {
            UiIsEnabled = _state;
            if (_state == true)
            {
                ResetMouseSetup();
            }
        }

        protected virtual void HandleUiActiveSelf()
        {
            UiIsEnabled = uiMaster.isUiAlreadyInUse;
            if (uiMaster.isUiAlreadyInUse)
            {
                ResetMouseSetup();
            }
        }
        #endregion

        #region InputCalls
        protected void CallMenuToggle() { uiMaster.CallEventMenuToggle(); }
        protected void CallToggleIsGamePaused() { gamemaster.CallOnToggleIsGamePaused(); }
        protected void CallOnNumberKeyPress(int _index) { gamemaster.CallOnNumberKeyPress(_index); }
        #endregion

        #region Initialization
        void SubToEvents()
        {
            gamemaster.OnToggleIsGamePaused += HandleGamePaused;
            uiMaster.EventAnyUIToggle += HandleUiActiveSelf;
        }

        void UnsubFromEvents()
        {
            gamemaster.OnToggleIsGamePaused -= HandleGamePaused;
            uiMaster.EventAnyUIToggle -= HandleUiActiveSelf;
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Used Whenever Mouse Setup Needs to be disabled,
        /// Such as when a UI Menu (Pause Menu) is Active
        /// </summary>
        void ResetMouseSetup()
        {
            if (isRMHeldPastThreshold)
            {
                isRMHeldPastThreshold = false;
                gamemaster.CallEventHoldingRightMouseDown(false);
            }
            if (isLMHeldPastThreshold)
            {
                isLMHeldPastThreshold = false;
                gamemaster.CallEventHoldingLeftMouseDown(false);
            }

            isLMHeldDown = false;
            isRMHeldDown = false;
            //Reset Scrolling
            isScrolling = false;
            isNotScrollingPastThreshold = true;
            bBeganScrolling = false;
            noScrollCurrentTimer = 0.0f;
            gamemaster.CallEventEnableCameraZoom(false, bScrollAxisIsPositive);
        }
        #endregion
    }
}