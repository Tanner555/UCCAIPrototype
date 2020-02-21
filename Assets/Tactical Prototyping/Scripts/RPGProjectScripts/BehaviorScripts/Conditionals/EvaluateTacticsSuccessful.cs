using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RPGPrototype
{
    [TaskCategory("RPGPrototype/AllyMember")]
    [TaskDescription("Returns True if Evaluating Tactics Successfully Returns A Tactics Item.")]
    public class EvaluateTacticsSuccessful : Conditional
	{
        #region Shared
        public SharedTacticsItem CurrentExecutionItem;
        public SharedAllyMember CurrentExecutionTarget;
        #endregion

        #region FieldsAndProperties
        protected Dictionary<AllyTacticsItem, AllyMember> evalTactics = new Dictionary<AllyTacticsItem, AllyMember>();
        protected List<AllyTacticsItem> AllyTacticsList => aiController.AllyTacticsList;

        protected PartyManager myPartyManager { get { return allyMember ? allyMember.partyManager : null; } }
        protected AllyMember allyInCommand
        {
            get
            {
                return myPartyManager != null ? myPartyManager.AllyInCommand : null;
            }
        }

        AllyMember allyMember
        {
            get
            {
                if (_allymember == null)
                {
                    _allymember = GetComponent<AllyMember>();
                }
                return _allymember;
            }
        }
        AllyMember _allymember = null;

        AIControllerRPG aiController
        {
            get
            {
                if (_aiController == null)
                {
                    _aiController = (AIControllerRPG)allyMember.aiController;
                }
                return _aiController;
            }
        }
        AIControllerRPG _aiController = null;

        AllyEventHandler myEventHandler
        {
            get
            {
                if (_myEventhandler == null)
                {
                    _myEventhandler = GetComponent<AllyEventHandler>();
                }
                return _myEventhandler;
            }
        }
        AllyEventHandler _myEventhandler = null;
        #endregion

        #region Overrides
        public override TaskStatus OnUpdate()
        {
            //Temporary Fix for PartyManager Delaying Initial AllyInCommand Methods
            if (allyInCommand == null)
            {
                CurrentExecutionItem.Value = null;
                CurrentExecutionTarget.Value = null;
                return TaskStatus.Failure;
            }

            evalTactics.Clear();
            foreach (var _tactic in AllyTacticsList)
            {
                //If Condition is True and 
                //Can Perform The Given Action
                var _boolTargetTuple = _tactic.condition.action(allyMember, aiController);
                if (_boolTargetTuple._success &&
                    _tactic.action.canPerformAction(allyMember, aiController, _boolTargetTuple._target))
                {
                    evalTactics.Add(_tactic, _boolTargetTuple._target);
                }
            }

            if (evalTactics.Count > 0)
            {
                var _currentItem = EvaluateTacticalConditionOrders();                
                CurrentExecutionItem.Value = _currentItem._tacticItem;
                CurrentExecutionTarget.Value = _currentItem._target;
                return TaskStatus.Success;
            }
            else
            {
                CurrentExecutionItem.Value = null;
                CurrentExecutionTarget.Value = null;
                return TaskStatus.Failure;
            }                
        }
        #endregion

        #region HelperMethods
        protected (AllyTacticsItem _tacticItem, AllyMember _target) EvaluateTacticalConditionOrders()
        {
            int _order = int.MaxValue;
            AllyTacticsItem _exeTactic = null;
            AllyMember _exeTarget = null;
            foreach (var _tactic in evalTactics)
            {                
                if (_tactic.Key.order < _order)
                {
                    _order = _tactic.Key.order;
                    _exeTactic = _tactic.Key;
                    _exeTarget = _tactic.Value;
                }
            }
            return (_exeTactic, _exeTarget);
        }
        #endregion
    }
}