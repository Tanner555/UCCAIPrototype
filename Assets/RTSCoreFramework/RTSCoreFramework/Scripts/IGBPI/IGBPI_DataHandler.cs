using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BaseFramework;

namespace RTSCoreFramework
{
    public class IGBPI_DataHandler : BaseSingleton<IGBPI_DataHandler>
    {
        #region Enums
        public ConditionFilters conditionFilter { get; protected set; }
        public ActionFilters actionFilter { get; protected set; }

        public ConditionFilters NavForwardCondition()
        {
            var _filters = Enum.GetValues(typeof(ConditionFilters));
            if ((int)conditionFilter < _filters.Length - 1)
            {
                conditionFilter++;
            }
            else
            {
                conditionFilter = (ConditionFilters)0;
            }
            return conditionFilter;
        }

        public ConditionFilters NavPreviousCondition()
        {
            var _filters = Enum.GetValues(typeof(ConditionFilters));
            if ((int)conditionFilter > 0)
            {
                conditionFilter--;
            }
            else
            {
                conditionFilter = (ConditionFilters)_filters.Length - 1;
            }
            return conditionFilter;
        }

        public ActionFilters NavForwardAction()
        {
            var _filters = Enum.GetValues(typeof(ActionFilters));
            if ((int)actionFilter < _filters.Length - 1)
            {
                actionFilter++;
            }
            else
            {
                actionFilter = (ActionFilters)0;
            }
            return actionFilter;
        }

        public ActionFilters NavPreviousAction()
        {
            var _filters = Enum.GetValues(typeof(ActionFilters));
            if ((int)actionFilter > 0)
            {
                actionFilter--;
            }
            else
            {
                actionFilter = (ActionFilters)_filters.Length - 1;
            }
            return actionFilter;
        }

        #endregion

        #region Properties
        protected RTSGameMode gamemode { get { return RTSGameMode.thisInstance; } }
        #endregion

        #region ConditionDictionary
        public virtual Dictionary<string, IGBPI_Condition> IGBPI_Conditions
        {
            get { return _IGBPI_Conditions; }
        }

        protected Dictionary<string, IGBPI_Condition> _IGBPI_Conditions = new Dictionary<string, IGBPI_Condition>()
        {
            {"Self: Any", new IGBPI_Condition((_ally) => true, ConditionFilters.Standard) },
            {"Leader: Not Within Follow Distance", new IGBPI_Condition((_ally) =>
            { return !_ally.aiController.IsWithinFollowingDistance(); }, ConditionFilters.Standard) },
            {"Leader: Within Follow Distance", new IGBPI_Condition((_ally) =>
            { return _ally.aiController.IsWithinFollowingDistance(); }, ConditionFilters.Standard) },
            {"Self: Health < 100", new IGBPI_Condition((_ally) =>
            { return _ally.HealthAsPercentage < 1; }, ConditionFilters.AllyHealth) },
            {"Self: Health < 90", new IGBPI_Condition((_ally) =>
            { return _ally.HealthAsPercentage < 0.90; }, ConditionFilters.AllyHealth) },
            {"Self: Health < 75", new IGBPI_Condition((_ally) =>
            { return _ally.HealthAsPercentage < 0.75; }, ConditionFilters.AllyHealth) },
            {"Self: Health < 50", new IGBPI_Condition((_ally) =>
            { return _ally.HealthAsPercentage < 0.50; }, ConditionFilters.AllyHealth) },
            {"Self: Health < 25", new IGBPI_Condition((_ally) =>
            { return _ally.HealthAsPercentage < 0.25; }, ConditionFilters.AllyHealth) },
            {"Self: Health < 10", new IGBPI_Condition((_ally) =>
            { return _ally.HealthAsPercentage < 0.10; }, ConditionFilters.AllyHealth) },
            {"Self: CurAmmo < 10", new IGBPI_Condition((_ally) =>
            { return _ally.CurrentEquipedAmmo < 10; }, ConditionFilters.AllyGun) },
            {"Self: CurAmmo = 0", new IGBPI_Condition((_ally) =>
            { return _ally.CurrentEquipedAmmo == 0; }, ConditionFilters.AllyGun) },
            {"Self: CurAmmo > 0", new IGBPI_Condition((_ally) =>
            { return _ally.CurrentEquipedAmmo > 0; }, ConditionFilters.AllyGun) },
            {"Enemy: WithinSightRange", new IGBPI_Condition((_ally) =>
            { return _ally.aiController.Tactics_IsEnemyWithinSightRange(); }, ConditionFilters.TargetedEnemy)  },
        };
        #endregion

        #region ActionDictionary
        public virtual Dictionary<string, RTSActionItem> IGBPI_Actions
        {
            get { return _IGBPI_Actions; }
        }

        protected Dictionary<string, RTSActionItem> _IGBPI_Actions = new Dictionary<string, RTSActionItem>()
        {
            {"Self: Attack Targetted Enemy", new RTSActionItem((_ally) =>
                { _ally.aiController.AttackTargettedEnemy(); },
                (_ally) => { return _ally.bIsCarryingMeleeWeapon ||
                _ally.CurrentEquipedAmmo > 0; },
            ActionFilters.AI, false, false, true, false, _ally => true, _ally => _ally.bIsAttacking == false, _ally => _ally.allyEventHandler.CallEventStopTargettingEnemy()) },
            //{"Self: Attack Nearest Enemy", new IGBPI_Action((_ally) =>
            //{ _ally.aiController.Tactics_AttackClosestEnemy(); }, ActionFilters.AI) },
            {"Self: SwitchToNextWeapon", new RTSActionItem((_ally) =>
            { _ally.allyEventHandler.CallOnSwitchToNextItem(); },
                (_ally) => true,
                ActionFilters.Weapon, false, false, false, false, _ally => true, _ally => true, _ally => { }) },
            {"Self: SwitchToPrevWeapon", new RTSActionItem((_ally) =>
            { _ally.allyEventHandler.CallOnSwitchToPrevItem(); },
                (_ally) => true,
                ActionFilters.Weapon, false, false, false, false, _ally => true, _ally => true, _ally => { }) },
            {"Self: FollowLeader", new RTSActionItem((_ally) =>
            { _ally.aiController.Tactics_MoveToLeader(); },
                (_ally) => true,
                ActionFilters.Movement, false, false, false, true, _ally => true, _ally => false, _ally => _ally.allyEventHandler.CallEventFinishedMoving()) },
            {"Debug: Log True Message", new RTSActionItem((_ally) =>
            Debug.Log("Condition is true, called from: " + _ally.CharacterName),
                (_ally) => true,
                ActionFilters.Debugging, false, false, false, false, _ally => true, _ally => true, _ally => { }) }
        };
        #endregion

        #region UnityMessages
        protected virtual void OnEnable()
        {

        }
        // Use this for initialization
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

        #region ConditionHelpers

        #endregion

        #region ActionHelpers

        #endregion

        #region Structs
        public struct IGBPI_Condition
        {
            public Func<AllyMember, bool> action;
            public ConditionFilters filter;

            public IGBPI_Condition(Func<AllyMember, bool> action, ConditionFilters filter)
            {
                this.action = action;
                this.filter = filter;
            }
        }

        //public struct IGBPI_Action
        //{
        //    public Action<AllyMember> action;
        //    public Func<AllyMember, bool> canPerformAction;
        //    public ActionFilters filter;

        //    public IGBPI_Action(Action<AllyMember> action, 
        //        Func<AllyMember, bool> canPerformAction, 
        //        ActionFilters filter)
        //    {
        //        this.action = action;
        //        this.canPerformAction = canPerformAction;
        //        this.filter = filter;
        //    }
        //}
        #endregion
    }

    #region OutsideClassEnums
    public enum ConditionFilters
    {
        Standard = 0, AllyHealth = 1, AllyGun = 2,
        TargetedEnemy = 3, AllyStamina = 4,
        AllyAbilities = 5
    }
    public enum ActionFilters
    {
        Movement = 0, Weapon = 1, AI = 2,
        Debugging = 3, Abilities = 4
    }
    #endregion
}