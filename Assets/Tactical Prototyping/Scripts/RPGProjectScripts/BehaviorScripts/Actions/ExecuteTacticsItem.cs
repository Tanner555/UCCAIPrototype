using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;

namespace RPGPrototype
{
    [TaskCategory("RPGPrototype/AllyMember")]
    [TaskDescription("Executes The Given Tactics Item with Target. StopPerformingTask Determines If Stop Performing Task Should Be Executed Instead. Make Sure The CurrentTacticsItem and Target aren't null when executing an action. If Stopping, make sure previous tactics item isn't null either. Current will be reset if not stopping, and previous will be reset after stopping.")]
    public class ExecuteTacticsItem : Action
	{
        #region Shared
        public SharedBool StopPerformingTask;
        public SharedTacticsItem CurrentExecutionItem;
		public SharedAllyMember CurrentExecutionTarget;
        public SharedTacticsItem PreviousExecutionItem;
        public SharedAllyMember PreviousExecutionTarget;
        #endregion

        #region Properties
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
        #endregion

        #region Overrides
        public override TaskStatus OnUpdate()
		{
            if (StopPerformingTask.Value == false)
            {
                if (CurrentExecutionItem.Value != null && CurrentExecutionTarget.Value != null)
                {
                    //Not Stopping, Execute Current Item
                    CurrentExecutionItem.Value.action.actionToPerform(allyMember, aiController, CurrentExecutionTarget.Value);
                    ResetCurrentTacticsItem();
                }
                else
                {
                    //Resetting Current Tactics Item
                    CurrentExecutionItem.Value = null;
                    CurrentExecutionTarget.Value = null;
                    //Return Failure If Current Item is Null when trying to execute action.
                    return TaskStatus.Failure;
                }
            }
            else
            {
                if(PreviousExecutionItem.Value != null && PreviousExecutionTarget.Value != null)
                {
                    //Use Previous Execution Item To Stop Performing Task
                    PreviousExecutionItem.Value.action.stopPerformingTask(allyMember, aiController, PreviousExecutionTarget.Value);
                    ResetPreviousTacticsItem();
                }
                else
                {
                    ResetPreviousTacticsItem();
                    //Return Failure If Previous Item is Null when trying to stop execution.
                    return TaskStatus.Failure;
                }
            }
            return TaskStatus.Success;
		}
        #endregion

        #region Helpers
        void ResetPreviousTacticsItem()
        {
            //Do not reset current, only past tactics item.
            PreviousExecutionItem.Value = null;
            PreviousExecutionTarget.Value = null;
        }

        void ResetCurrentTacticsItem()
        {
            //Setting Previous Tactics Item To Current Item
            PreviousExecutionItem.Value = CurrentExecutionItem.Value;
            PreviousExecutionTarget.Value = CurrentExecutionTarget.Value;
            //Resetting Current Tactics Item
            CurrentExecutionItem.Value = null;
            CurrentExecutionTarget.Value = null;
        }
        #endregion
    }
}