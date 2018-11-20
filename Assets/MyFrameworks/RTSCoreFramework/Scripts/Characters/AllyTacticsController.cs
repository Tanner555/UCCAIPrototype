using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCoreFramework
{
    public class AllyTacticsController : MonoBehaviour
    {
        #region Properties
        protected AllyEventHandler myEventHandler
        {
            get
            {
                if (_myEventHandler == null)
                    _myEventHandler = GetComponent<AllyEventHandler>();

                return _myEventHandler;
            }
        }
        private AllyEventHandler _myEventHandler = null;
        protected AllyMember allyMember
        {
            get
            {
                if (_allyMember == null)
                    _allyMember = GetComponent<AllyMember>();

                return _allyMember;
            }
        }
        private AllyMember _allyMember = null;
        protected AllyAIController aiController
        {
            get
            {
                if (_aiController == null)
                    _aiController = GetComponent<AllyAIController>();

                return _aiController;
            }
        }
        private AllyAIController _aiController = null;
        protected RTSStatHandler statHandler
        {
            get
            {
                return RTSStatHandler.thisInstance;
            }
        }
        protected RTSGameMaster gameMaster { get { return RTSGameMaster.thisInstance; } }
        protected RTSGameMode gamemode { get { return RTSGameMode.thisInstance; } }
        protected RTSUiMaster uiMaster { get { return RTSUiMaster.thisInstance; } }
        protected RTSUiManager uiManager { get { return RTSUiManager.thisInstance; } }
        protected RTSSaveManager saveManager { get { return RTSSaveManager.thisInstance; } }
        protected IGBPI_DataHandler dataHandler { get { return IGBPI_DataHandler.thisInstance; } }
        protected PartyManager myPartyManager { get { return allyMember ? allyMember.partyManager : null; } }
        protected AllyMember allyInCommand
        {
            get
            {
                return myPartyManager != null ? myPartyManager.AllyInCommand : null;
            }
        }

        protected bool AllyComponentsAreReady
        {
            get
            {
                return allyMember && myEventHandler && aiController &&
                  gamemode && gameMaster && uiManager && saveManager;
            }
        }

        /// <summary>
        /// Previous Item Is Either Null Or Doesn't Equal Current Item
        /// </summary>
        protected bool bCanAddActionItemToQueue
        {
            get
            {
                return previousExecutionItem == null ||
                    previousExecutionItem != currentExecutionItem;
            }
        }
        #endregion

        #region Fields
        protected bool bEnableTactics = false;
        protected bool bPreviouslyEnabledTactics = false;
        protected List<AllyTacticsItem> evalTactics = new List<AllyTacticsItem>();
        public List<AllyTacticsItem> AllyTacticsList = new List<AllyTacticsItem>();
        public int executionsPerSec = 5;
        protected AllyTacticsItem currentExecutionItem = null;
        protected AllyTacticsItem previousExecutionItem = null;
        #endregion

        #region UnityMessages
        protected virtual void OnEnable()
        {
            SetInitialReferences();
            SubToEvents();
        }

        protected virtual void OnDisable()
        {
            UnsubFromEvents();
            UnLoadAndCancelTactics();
        }

        // Use this for initialization
        protected virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }
        #endregion

        #region Handlers
        protected virtual void HandleInitAllyComps(RTSAllyComponentSpecificFields _specific, RTSAllyComponentsAllCharacterFields _allFields)
        {
            executionsPerSec = _allFields.tacticsExecutionsPerSecond;
        }

        protected virtual void HandleAllySwitch(PartyManager _party, AllyMember _toSet, AllyMember _current)
        {
            if (allyMember.partyManager != _party) return;

            if (allyMember == _toSet &&
                _toSet.bIsInGeneralCommanderParty)
            {
                CallToggleTactics(false);
            }
            else
            {
                CallToggleTactics(true);
            }
        }

        protected virtual void OnSaveTactics()
        {
            if (bEnableTactics)
            {
                LoadAndExecuteAllyTactics();
            }
        }

        protected virtual void HandleAllyDeath()
        {
            UnsubFromEvents();
            UnLoadAndCancelTactics();
            Destroy(this);
        }

        #endregion

        #region OldHandlers
        //Used For Reference Only
        protected virtual void HandleEventTogglebIsFreeMoving(bool _enable)
        {
            bool _canEnableTactics = (myEventHandler.bIsCommandMoving ||
                  myEventHandler.bIsFreeMoving) == false &&
                  myEventHandler.bIsCommandAttacking == false;
            bool _enableTactics = !_enable && _canEnableTactics;
            CallToggleTactics(_enable);
        }

        protected virtual void HandleFinishMoving()
        {
            bool _canEnableTactics = (myEventHandler.bIsCommandMoving ||
                 myEventHandler.bIsFreeMoving) == false &&
                 myEventHandler.bIsCommandAttacking == false;
            if (_canEnableTactics)
            {
                CallToggleTactics(true);
            }
        }

        #endregion

        #region TacticsMethods
        protected virtual void LoadAndExecuteAllyTactics()
        {
            UnLoadAndCancelTactics();
            var _tactics = statHandler.RetrieveCharacterTactics(
                    allyMember, allyMember.CharacterType);
            foreach (var _data in _tactics.Tactics)
            {
                bool _hasCondition = dataHandler.IGBPI_Conditions.ContainsKey(_data.condition);
                bool _hasAction = dataHandler.IGBPI_Actions.ContainsKey(_data.action);
                int _order = -1;
                bool _hasOrder = int.TryParse(_data.order, out _order) && _order != -1;
                if (_hasCondition && _hasAction && _hasOrder)
                {
                    AllyTacticsList.Add(new AllyTacticsItem(_order,
                        dataHandler.IGBPI_Conditions[_data.condition],
                        dataHandler.IGBPI_Actions[_data.action]));
                }
            }

            if (AllyTacticsList.Count > 0)
                InvokeRepeating("ExecuteAllyTacticsList", 0.05f, 1f / executionsPerSec);
        }

        protected virtual void UnLoadAndCancelTactics()
        {
            if (IsInvoking("ExecuteAllyTacticsList"))
            {
                CancelInvoke("ExecuteAllyTacticsList");
            }
            AllyTacticsList.Clear();
        }

        protected virtual void ExecuteAllyTacticsList()
        {
            if (!AllyComponentsAreReady)
            {
                Debug.LogError("Not All Components are Available, cannot execute Tactics.");
                UnLoadAndCancelTactics();
                UnsubFromEvents();
                Destroy(this);
            }

            // Pause Ally Tactics If Ally Is Paused
            // Due to the Game Pausing Or Control Pause Mode
            // Is Active
            if (myEventHandler.bAllyIsPaused) return;
            //Don't Want to Execute Tactics While in the 
            //Middle of an Ability Use
            if (myEventHandler.bIsUsingAbility) return;

            //Temporary Fix for PartyManager Delaying Initial AllyInCommand Methods
            if (allyInCommand == null) return;

            evalTactics.Clear();
            foreach (var _tactic in AllyTacticsList)
            {
                //If Condition is True and 
                //Can Perform The Given Action
                if (_tactic.condition.action(allyMember) &&
                    _tactic.action.canPerformAction(allyMember))
                {
                    evalTactics.Add(_tactic);
                }
            }
            if (evalTactics.Count > 0)
            {
                previousExecutionItem = currentExecutionItem;
                currentExecutionItem = EvaluateTacticalConditionOrders(evalTactics);
                //Execution Item isn't null and Previous Entry Doesn't Equal Current One
                //Prevents AddActionItem Event Being Called Constantly
                if (currentExecutionItem != null &&
                    currentExecutionItem.action != null &&
                    currentExecutionItem.action.actionToPerform != null &&
                    bCanAddActionItemToQueue
                    )
                {
                    myEventHandler.CallOnAddActionItemToQueue(currentExecutionItem.action);
                }
            }
            else
            {
                if(currentExecutionItem != null)
                {
                    myEventHandler.CallOnRemoveAIActionFromQueue();
                }
                //Setting Previous To Null Is Equivalent To
                //A Boolean That Gets Set To True Once an 
                //Action Gets Added And False When Removing
                previousExecutionItem = null;
                currentExecutionItem = null;
            }
        }

        protected virtual AllyTacticsItem EvaluateTacticalConditionOrders(List<AllyTacticsItem> _tactics)
        {
            int _order = int.MaxValue;
            AllyTacticsItem _exeTactic = null;
            foreach (var _tactic in _tactics)
            {
                if (_tactic.order < _order)
                {
                    _order = _tactic.order;
                    _exeTactic = _tactic;
                }
            }
            return _exeTactic;
        }

        protected virtual void CallToggleTactics(bool _enable)
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
            myEventHandler.CallEventToggleAllyTactics(_enable);
        }

        #endregion

        #region Initialization
        protected virtual void SetInitialReferences()
        {

        }

        protected virtual void SubToEvents()
        {
            uiMaster.EventOnSaveIGBPIComplete += OnSaveTactics;
            gameMaster.OnAllySwitch += HandleAllySwitch;
            myEventHandler.EventAllyDied += HandleAllyDeath;
            myEventHandler.InitializeAllyComponents += HandleInitAllyComps;
            //Reference Only
            //myEventHandler.EventTogglebIsFreeMoving += HandleEventTogglebIsFreeMoving;
            //myEventHandler.EventFinishedMoving += HandleFinishMoving;
            //No Longer Handling Toggle Event, But Sending it Instead
            //myEventHandler.EventToggleAllyTactics += CallToggleTactics;
        }

        protected virtual void UnsubFromEvents()
        {
            uiMaster.EventOnSaveIGBPIComplete -= OnSaveTactics;
            gameMaster.OnAllySwitch -= HandleAllySwitch;
            myEventHandler.EventAllyDied -= HandleAllyDeath;
            myEventHandler.InitializeAllyComponents -= HandleInitAllyComps;
            //Reference Only
            //myEventHandler.EventTogglebIsFreeMoving -= HandleEventTogglebIsFreeMoving;
            //myEventHandler.EventFinishedMoving -= HandleFinishMoving;
            //No Longer Handling Toggle Event, But Sending it Instead
            //myEventHandler.EventToggleAllyTactics -= CallToggleTactics;
        }
        #endregion

        #region Structs
        [System.Serializable]
        public class AllyTacticsItem
        {
            public int order;
            public IGBPI_DataHandler.IGBPI_Condition condition;
            public RTSActionItem action;

            public AllyTacticsItem(int order,
                IGBPI_DataHandler.IGBPI_Condition condition,
                RTSActionItem action)
            {
                this.order = order;
                this.condition = condition;
                this.action = action;
            }
        }
        #endregion

    }
}