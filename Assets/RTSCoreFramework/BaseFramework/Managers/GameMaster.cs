using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public class GameMaster : BaseSingleton<GameMaster>
    {
        #region Properties
        //Components
        public GameInstance gameInstance
        {
            get { return GameInstance.thisInstance; }
        }

        public GameMode gamemode
        {
            get { return GameMode.thisInstance; }
        }

        protected UiMaster uiMaster
        {
            get { return UiMaster.thisInstance; }
        }

        //Access Properties
        public virtual bool bIsGamePaused
        {
            get { return _bIsGamePaused; }
            protected set { _bIsGamePaused = value; }
        }
        private bool _bIsGamePaused = false;

        public virtual bool bIsInPauseControlMode
        {
            get { return _bIsInPauseControlMode; }
            protected set { _bIsInPauseControlMode = value; }
        }
        private bool _bIsInPauseControlMode = false;

        public virtual bool isGameOver
        {
            get; protected set;
        }
        public virtual bool isMenuOn
        {
            get; protected set;
        }
        #endregion

        #region Fields
        protected float loadLevelDelay = 0.2f;
        protected float timePauseDelay = 0.05f;
        #endregion

        #region UnityMessages
        protected virtual void OnEnable()
        {

        }

        protected virtual void Start()
        {
            SubToEvents();
        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }

        protected virtual void OnDisable()
        {
            UnsubFromEvents();
        }
        #endregion

        #region Events and Delegates
        public delegate void GameManagerEventHandler();
        public delegate void OneBoolArgsHandler(bool enable);
        public delegate void TwoBoolArgsHandler(bool enable, bool isPositive);
        public delegate void OneIntParamHandler(int _number);
        //Levels, Scenarios, Progression
        public event GameManagerEventHandler RestartLevelEvent;
        public event GameManagerEventHandler GoToMenuSceneEvent;
        public event GameManagerEventHandler GoToNextLevelEvent;
        public event GameManagerEventHandler GoToNextScenarioEvent;
        public event GameManagerEventHandler GameOverEvent;
        public event GameManagerEventHandler EventAllObjectivesCompleted;
        //Input
        public event GameManagerEventHandler OnLeftClickNoSend;
        public event GameManagerEventHandler OnRightClickNoSend;
        public event TwoBoolArgsHandler EventEnableCameraZoom;
        public event OneIntParamHandler OnNumberKeyPress;
        //Camera and Pause
        public event OneBoolArgsHandler OnToggleIsGamePaused;
        public event OneBoolArgsHandler OnTogglebIsInPauseControlMode;
        public event OneBoolArgsHandler EventHoldingLeftMouseDown;
        public event OneBoolArgsHandler EventHoldingRightMouseDown;
        
        #endregion

        #region EventCalls
        public void CallEventRestartLevel()
        {
            CallOnToggleIsGamePaused(false);
            if (RestartLevelEvent != null) RestartLevelEvent();
            Invoke("WaitToRestartLevel", loadLevelDelay);
        }

        private void WaitToRestartLevel()
        {
            gameInstance.RestartCurrentLevel();
        }

        public void CallEventGoToMenuScene()
        {
            CallOnToggleIsGamePaused(false);
            if (GoToMenuSceneEvent != null) GoToMenuSceneEvent();
            Invoke("WaitToGoToMenuScene", loadLevelDelay);
        }

        private void WaitToGoToMenuScene()
        {
            gameInstance.GoToMainMenu();
        }

        public void CallEventGoToNextLevel()
        {
            CallOnToggleIsGamePaused(false);
            if (GoToNextLevelEvent != null) GoToNextLevelEvent();
            Invoke("WaitToGoToNextLevel", loadLevelDelay);
        }

        private void WaitToGoToNextLevel()
        {
            gameInstance.GoToNextLevel();
        }

        public void CallEventGoToNextScenario()
        {
            CallOnToggleIsGamePaused(false);
            if (GoToNextScenarioEvent != null) GoToNextScenarioEvent();
            Invoke("WaitToGoToNextScenario", loadLevelDelay);
        }

        private void WaitToGoToNextScenario()
        {
            gameInstance.GoToNextScenario();
        }

        public virtual void CallEventGameOver()
        {
            if (GameOverEvent != null)
            {
                if (!isGameOver)
                {
                    isGameOver = true;
                    GameOverEvent();
                }
            }
        }

        public virtual void CallEventAllObjectivesCompleted()
        {
            if (EventAllObjectivesCompleted != null) EventAllObjectivesCompleted();
        }

        public void CallOnToggleIsGamePaused()
        {
            CallOnToggleIsGamePaused(!bIsGamePaused);
        }

        public void CallOnToggleIsGamePaused(bool _enable)
        {
            StartCoroutine(PauseCheckerCoroutine(_enable, false));
        }

        //Used As a Wrapper For Pause Checker Coroutine Functionality
        private void Implement_CallOnToggleIsGamePaused(bool _enable)
        {
            bIsGamePaused = _enable;
            if (OnToggleIsGamePaused != null)
            {
                OnToggleIsGamePaused(bIsGamePaused);
            }
            Invoke("ToggleGamePauseTimeScale", timePauseDelay);
        }

        //Override this functionality in wrapper class
        protected virtual void ToggleGamePauseTimeScale()
        {
            Time.timeScale = bIsGamePaused ? 0f : 1f;
        }

        public void CallOnTogglebIsInPauseControlMode()
        {
            CallOnTogglebIsInPauseControlMode(!bIsInPauseControlMode);
        }

        public void CallOnTogglebIsInPauseControlMode(bool _enable)
        {
            StartCoroutine(PauseCheckerCoroutine(_enable, true));
        }

        //Used As a Wrapper For Pause Checker Coroutine Functionality
        private void Implement_CallOnTogglebIsInPauseControlMode(bool _enable)
        {
            bIsInPauseControlMode = _enable;
            if (OnTogglebIsInPauseControlMode != null)
            {
                OnTogglebIsInPauseControlMode(bIsInPauseControlMode);
            }
            Invoke("TogglePauseControlModeTimeScale", timePauseDelay);
        }

        //Override this functionality in wrapper class
        protected virtual void TogglePauseControlModeTimeScale()
        {
            Time.timeScale = bIsInPauseControlMode ? 0f : 1f;
        }

        public void CallEventHoldingRightMouseDown(bool _holding)
        {
            if (EventHoldingRightMouseDown != null)
            {
                EventHoldingRightMouseDown(_holding);
            }
        }

        public void CallEventHoldingLeftMouseDown(bool _holding)
        {
            if (EventHoldingLeftMouseDown != null)
            {
                EventHoldingLeftMouseDown(_holding);
            }
        }

        public virtual void CallEventOnLeftClick()
        {
            if (OnLeftClickNoSend != null) OnLeftClickNoSend();
        }

        public virtual void CallEventOnRightClick()
        {
            if (OnRightClickNoSend != null) OnRightClickNoSend();
        }

        public void CallEventEnableCameraZoom(bool enable, bool isPositive)
        {
            if (EventEnableCameraZoom != null) EventEnableCameraZoom(enable, isPositive);
        }

        public void CallOnNumberKeyPress(int _number)
        {
            if (OnNumberKeyPress != null) OnNumberKeyPress(_number);
        }
        #endregion

        #region Initialization
        protected virtual void SubToEvents()
        {
            
        }

        protected virtual void UnsubFromEvents()
        {
            
        }
        #endregion

        #region Helpers
        IEnumerator PauseCheckerCoroutine(bool _enable, bool _isInControl)
        {
            //Requesting Toggle InControl Pause While Game is Paused
            if (_isInControl && bIsGamePaused) yield break;
            else if (_isInControl)
            {
                //Implement IsInControlPause Functionality
                Implement_CallOnTogglebIsInPauseControlMode(_enable);
                yield break;
            }
            //Requesting Toggle Game Pause While InControl Pause is Active
            else if (_isInControl == false && bIsInPauseControlMode)
            {
                //Turn Off PauseControlMode
                Implement_CallOnTogglebIsInPauseControlMode(false);
                yield return new WaitForSecondsRealtime(timePauseDelay);
                //Implement GamePause Functionality
                Implement_CallOnToggleIsGamePaused(_enable);
                yield break;
            }
            else if(_isInControl == false)
            {
                //Implement GamePause Functionality
                Implement_CallOnToggleIsGamePaused(_enable);
                yield break;
            }
            yield break;
        }
        #endregion
    }
}