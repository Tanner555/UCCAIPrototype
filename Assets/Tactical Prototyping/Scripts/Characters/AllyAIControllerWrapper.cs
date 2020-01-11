using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RTSCoreFramework;
using Opsive.UltimateCharacterController;
using Opsive.UltimateCharacterController.Inventory;
using Opsive.UltimateCharacterController.Character;
using BehaviorDesigner.Runtime;

namespace RTSPrototype
{
    public class AllyAIControllerWrapper : AllyAIController
    {
        #region Fields
        //BTs
        private bool bUsingBehaviorTrees = true;
        #endregion

        #region Properties
        BehaviorTree AllyBehaviorTree
        {
            get
            {
                if (_AllyBehaviorTree == null)
                {
                    _AllyBehaviorTree = GetComponent<BehaviorTree>();
                }
                return _AllyBehaviorTree;
            }
        }
        BehaviorTree _AllyBehaviorTree = null;

        public string BBName_bIsAllyInCommand => "bIsAllyInCommand";
        public string BBName_bIsCurrentPlayer => "bIsCurrentPlayer";
        public string BBName_MyMoveDirection => "MyMoveDirection";
        //public string BBName_MyStationaryTurnSpeed => "MyStationaryTurnSpeed";
        //public string BBName_MyMovingTurnSpeed => "MyMovingTurnSpeed";
        //public string BBName_MyMoveThreshold => "MyMoveThreshold";
        //public string BBName_MyAnimatorForwardCap => "MyAnimatorForwardCap";
        //public string BBName_MyAnimationSpeedMultiplier => "MyAnimationSpeedMultiplier";
        public string BBName_MyNavDestination => "MyNavDestination";
        public string BBName_bHasSetDestination => "bHasSetDestination";
        public string BBName_bHasSetCommandMove => "bHasSetCommandMove";
        public string BBName_CurrentTargettedEnemy => "CurrentTargettedEnemy";
        public string BBName_bTargetEnemy => "bTargetEnemy";
        public string BBName_bIsFreeMoving => "bIsFreeMoving";
        public string BBName_bUpdateActiveTimeBar => "bUpdateActiveTimeBar";
        public string BBName_ActiveTimeBarRefillRate => "ActiveTimeBarRefillRate";
        public string BBName_EnergyRegenPointsPerSec => "EnergyRegenPointsPerSec";
        public string BBName_bTryUseAbility => "bTryUseAbility";
        public string BBName_AbilityToUse => "AbilityToUse";
        public string BBName_bIsPerformingAbility => "bIsPerformingAbility";
        public string BBName_bEnableTactics => "bEnableTactics";

        new AllyMemberWrapper allyMember
        {
            get
            {
                if (__allyMember == null)
                    __allyMember = GetComponent<AllyMemberWrapper>();

                return __allyMember;
            }
        }
        private AllyMemberWrapper __allyMember = null;

        new AllyEventHandlerWrapper myEventHandler
        {
            get
            {
                if (__myEventHandler == null)
                    __myEventHandler = GetComponent<AllyEventHandlerWrapper>();

                return __myEventHandler;
            }
        }
        private AllyEventHandlerWrapper __myEventHandler = null;

        UltimateCharacterLocomotion myRigidbodyTPC
        {
            get
            {
                if (_myRigidbodyTPC == null)
                    _myRigidbodyTPC = GetComponent<UltimateCharacterLocomotion>();

                return _myRigidbodyTPC;
            }
        }
        private UltimateCharacterLocomotion _myRigidbodyTPC = null;

        Inventory myInventory
        {
            get
            {
                if (_myInventory == null)
                    _myInventory = GetComponent<Inventory>();

                return _myInventory;
            }
        }
        private Inventory _myInventory = null;

        //ItemHandler itemHandler
        //{
        //    get
        //    {
        //        if (_itemHandler == null)
        //            _itemHandler = GetComponent<ItemHandler>();

        //        return _itemHandler;
        //    }
        //}
        //private ItemHandler _itemHandler = null;

        //RTSNavBridge myRTSNavBridge
        //{
        //    get
        //    {
        //        if (_myRTSNavBridge == null)
        //            _myRTSNavBridge = GetComponent<RTSNavBridge>();

        //        return _myRTSNavBridge;
        //    }
        //}
        //private RTSNavBridge _myRTSNavBridge = null;

        //protected override bool AllCompsAreValid
        //{
        //    get
        //    {
        //        return myRigidbodyTPC && myInventory /*&& itemHandler*/
        //            && myNavAgent /*&& myRTSNavBridge*/ && myEventHandler
        //            && allyMember;
        //    }
        //}

        //public AllyMemberWrapper currentTargettedEnemyWrapper
        //{
        //    get { return (AllyMemberWrapper)currentTargettedEnemy; }
        //}
        #endregion

        #region UnityMessages
        protected override void Start()
        {
            base.Start();

        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }
        #endregion

        #region Handlers
        protected override void OnAllyInitComps(RTSAllyComponentSpecificFields _specific, RTSAllyComponentsAllCharacterFields _allFields)
        {
            base.OnAllyInitComps(_specific, _allFields);
            var _RPGallAllyComps = (RTSAllyComponentsAllCharacterFieldsWrapper)_allFields;
            //var _rpgCharAttr = ((RTSAllyComponentSpecificFields)_specific).RPGCharacterAttributesObject;
            //this.bUseAStarPath = _RPGallAllyComps.bUseAStarPath;
            bUsingBehaviorTrees = _RPGallAllyComps.bUseBehaviourTrees;
            if (_RPGallAllyComps.bUseBehaviourTrees && _RPGallAllyComps.allAlliesDefaultBehaviourTree != null)
            {
                BehaviorTree _behaviourtree;
                if (GetComponent<BehaviorTree>() == null)
                {
                    //If BehaviorTree Can't Be Found, Add One And Initialize It Manually
                    _behaviourtree = gameObject.AddComponent<BehaviorTree>();
                    _behaviourtree.StartWhenEnabled = false;
                    _behaviourtree.ExternalBehavior = _RPGallAllyComps.allAlliesDefaultBehaviourTree;
                    _behaviourtree.BehaviorName = $"{_specific.CharacterType.ToString()} Behavior";
                }
                else
                {
                    //BehaviorTree Already Exists, Not Need To Manually Set it Up
                    _behaviourtree = AllyBehaviorTree;
                }
                //RPG Character Moving
                //_behaviourtree.SetVariableValue(BBName_MyStationaryTurnSpeed, _rpgCharAttr.stationaryTurnSpeed);
                //_behaviourtree.SetVariableValue(BBName_MyMovingTurnSpeed, _rpgCharAttr.movingTurnSpeed);
                //_behaviourtree.SetVariableValue(BBName_MyMoveThreshold, _rpgCharAttr.moveThreshold);
                //_behaviourtree.SetVariableValue(BBName_MyAnimatorForwardCap, _rpgCharAttr.animatorForwardCap);
                //_behaviourtree.SetVariableValue(BBName_MyAnimationSpeedMultiplier, _rpgCharAttr.animationSpeedMultiplier);
                //Active Time Bar
                _behaviourtree.SetVariableValue(BBName_bUpdateActiveTimeBar, false);
                _behaviourtree.SetVariableValue(BBName_ActiveTimeBarRefillRate, 5);
                //Abilities and Energy Bar
                _behaviourtree.SetVariableValue(BBName_EnergyRegenPointsPerSec, 10);
                _behaviourtree.SetVariableValue(BBName_bTryUseAbility, false);
                _behaviourtree.SetVariableValue(BBName_AbilityToUse, null);
                _behaviourtree.SetVariableValue(BBName_bIsPerformingAbility, false);

                if (_behaviourtree.StartWhenEnabled == false)
                {
                    //Only Manually Start Tree if It Doesn't Start On Enable
                    StartCoroutine(StartDefaultBehaviourTreeAfterDelay());
                }
            }
        }

        IEnumerator StartDefaultBehaviourTreeAfterDelay()
        {
            yield return new WaitForSeconds(0.2f);
            //Used To Mitigate Temporary BT Being Null When First AllySwitch Occurs
            AllySwitchHelper(allyMember.bIsAllyInCommand, allyMember.bIsCurrentPlayer, allyMember.bIsInGeneralCommanderParty);
            AllyBehaviorTree.EnableBehavior();            
        }

        protected override void HandleAllySwitch(PartyManager _party, AllyMember _toSet, AllyMember _current)
        {
            base.HandleAllySwitch(_party, _toSet, _current);
            if (bUsingBehaviorTrees && _party == allyMember.partyManager && AllyBehaviorTree != null)
            {
                bool _allyInCommand = _toSet == allyMember;
                bool _isCurrentPlayer = _toSet == allyMember && _toSet.bIsInGeneralCommanderParty;
                AllySwitchHelper(_allyInCommand, _isCurrentPlayer, _toSet.bIsInGeneralCommanderParty);
            }
        }

        private void AllySwitchHelper(bool _allyInCommand, bool _isCurrentPlayer, bool _inCommanderParty)
        {
            if (bUsingBehaviorTrees && AllyBehaviorTree != null)
            {
                AllyBehaviorTree.SetVariableValue(BBName_bIsAllyInCommand, _allyInCommand);
                AllyBehaviorTree.SetVariableValue(BBName_bIsCurrentPlayer, _isCurrentPlayer);
                //Don't Enable Tactics If Current Player
                //Otherwise, Load and Execute Tactics
                ToggleTactics(_isCurrentPlayer == false);
                if (_inCommanderParty)
                {
                    //NOT Command Moving If AllySwitch For All Members of Current Player's Party
                    AllyBehaviorTree.SetVariableValue(BBName_bHasSetCommandMove, false);
                }
            }
        }

        protected override void HandleOnMoveAlly(Vector3 _point, bool _isCommandMove)
        {
            //base.HandleOnMoveAlly(_point);
            if (bUsingBehaviorTrees)
            {
                AllyBehaviorTree.SetVariableValue(BBName_MyNavDestination, _point);
                AllyBehaviorTree.SetVariableValue(BBName_bHasSetDestination, true);
                AllyBehaviorTree.SetVariableValue(BBName_bHasSetCommandMove, _isCommandMove);
                if (_isCommandMove)
                {
                    ResetTargetting();
                }
            }
        }

        protected override void HandleCommandAttackEnemy(AllyMember enemy)
        {
            base.HandleCommandAttackEnemy(enemy);
            if (bUsingBehaviorTrees)
            {
                bool _isFreeMoving = (bool)AllyBehaviorTree.GetVariable(BBName_bIsFreeMoving).GetValue();
                bool _isPerformingAbility = IsPerformingSpecialAbility();
                //Shouldn't Be FreeMoving or Performing Special Ability
                if (_isFreeMoving == false && _isPerformingAbility == false)
                {
                    SetEnemyTarget(enemy);
                    AllyBehaviorTree.SetVariableValue(BBName_bHasSetCommandMove, false);
                    AllyBehaviorTree.SetVariableValue(BBName_bTryUseAbility, false);
                    AllyBehaviorTree.SetVariableValue(BBName_AbilityToUse, null);
                }
            }
        }

        protected void OnKeyPress(int _key)
        {
            int _index = _key - 1;

            if (allyMember == null || allyMember.IsAlive == false || allyMember.bIsCurrentPlayer == false ||
                _index < 0 || _index > allyMember.GetNumberOfAbilities() - 1) return;

            var _config = allyMember.GetAbilityConfig(_index);
            AllyBehaviorTree.SetVariableValue(BBName_bTryUseAbility, true);
            AllyBehaviorTree.SetVariableValue(BBName_AbilityToUse, _config);
        }

        protected override void HandleOnTogglePause(bool _paused)
        {
            base.HandleOnTogglePause(_paused);
            if (bUsingBehaviorTrees)
            {
                if (_paused)
                {
                    AllyBehaviorTree.DisableBehavior(true);
                }
                else
                {
                    AllyBehaviorTree.EnableBehavior();
                }
            }
        }

        void OnDeath(Vector3 position, Vector3 force, GameObject attacker)
        {
            //TODO: RTSPrototype Check For Melee HitBox When Character Dies
            //var _mHitbox = transform.GetComponentInChildren<MeleeWeaponHitbox>();
            //if (_mHitbox != null)
            //{
            //    SphereCollider _sphereCol;
            //    if ((_sphereCol = _mHitbox.GetComponent<SphereCollider>()) != null)
            //    {
            //        _sphereCol.enabled = false;
            //    }
            //    _mHitbox.SetActive(false);
            //}
            if (bUsingBehaviorTrees)
            {
                AllyBehaviorTree.DisableBehavior();
            }
            StopAllCoroutines();
            CancelInvoke();
            this.enabled = false;
        }

        #endregion

        #region TacticsMainMethods
        protected override void ToggleTactics(bool _enable)
        {
            base.ToggleTactics(_enable);
        }

        protected override void LoadAndExecuteAllyTactics()
        {
            //base function loads tactics from stat and data handlers
            base.LoadAndExecuteAllyTactics();
            if (AllyTacticsList.Count > 0)
            {
                AllyBehaviorTree.SetVariableValue(BBName_bEnableTactics, true);
            }
            else
            {
                AllyBehaviorTree.SetVariableValue(BBName_bEnableTactics, false);
            }
        }

        protected override void UnLoadAndCancelTactics()
        {
            //Cancel Tactics From Running
            AllyBehaviorTree.SetVariableValue(BBName_bEnableTactics, false);
            //Important For Clearing TacticsList
            base.UnLoadAndCancelTactics();
        }
        #endregion

        #region AITacticsCommands
        public override (bool _success, AllyMember _target) Tactics_IsEnemyWithinSightRange()
        {
            AllyMember _closestEnemy = FindClosestEnemy();
            bool _valid = _closestEnemy != null && _closestEnemy.IsAlive;
            if (_valid)
            {
                return (true, _closestEnemy);
            }
            else
            {
                return (false, null);
            }
        }

        public override void AttackTargettedEnemy(AllyMember _self, AllyAIController _ai, AllyMember _target)
        {
            if (_target != null && _target.IsAlive)
            {
                SetEnemyTarget(_target);
            }
            else
            {
                ResetTargetting();
            }
        }

        public override void Tactics_AttackClosestEnemy()
        {
            Transform _currentTarget;
            //Don't Attack Closest Enemy if Already Attacking One
            if (IsTargettingEnemy(out _currentTarget) == false)
            {
                AllyMember _closestEnemy = FindClosestEnemy();
                if (_closestEnemy != null)
                {
                    SetEnemyTarget(_closestEnemy);
                }
            }
        }
        #endregion

        #region Helpers
        public override bool IsPerformingSpecialAbility()
        {
            return (bool)AllyBehaviorTree.GetVariable(BBName_bIsPerformingAbility).GetValue();
        }

        public override bool IsTargettingEnemy(out Transform _currentTarget)
        {
            _currentTarget = null;
            bool _isTargetting = (bool)AllyBehaviorTree.GetVariable(BBName_bTargetEnemy).GetValue();
            if (_isTargetting)
            {
                _currentTarget = (Transform)AllyBehaviorTree.GetVariable(BBName_CurrentTargettedEnemy).GetValue();
            }
            return _isTargetting;
        }

        public override void TryStartSpecialAbility(System.Type _abilityType)
        {
            AbilityConfig _config;
            if (IsPerformingSpecialAbility() == false && allyMember.CanUseAbility(_abilityType, out _config))
            {
                AllyBehaviorTree.SetVariableValue(BBName_bTryUseAbility, true);
                AllyBehaviorTree.SetVariableValue(BBName_AbilityToUse, _config);
            }
        }

        public override void SetEnemyTarget(AllyMember _target)
        {
            AllyBehaviorTree.SetVariableValue(BBName_CurrentTargettedEnemy, _target.transform);
            AllyBehaviorTree.SetVariableValue(BBName_bTargetEnemy, true);
        }

        /// <summary>
        /// Finish Moving Helper For Tactics
        /// </summary>
        public override void FinishMoving()
        {
            //Override To Update BT
            AllyBehaviorTree.SetVariableValue(BBName_bHasSetDestination, false);
            AllyBehaviorTree.SetVariableValue(BBName_bHasSetCommandMove, false);
            AllyBehaviorTree.SetVariableValue(BBName_MyNavDestination, Vector3.zero);
            myNavAgent.SetDestination(transform.position);
            myNavAgent.velocity = Vector3.zero;
        }

        public override void ResetTargetting()
        {
            AllyBehaviorTree.SetVariableValue(BBName_bTargetEnemy, false);
            AllyBehaviorTree.SetVariableValue(BBName_CurrentTargettedEnemy, null);
        }

        public override void ResetSpecialAbilities()
        {
            if (IsPerformingSpecialAbility() == false)
            {
                AllyBehaviorTree.SetVariableValue(BBName_bTryUseAbility, false);
                AllyBehaviorTree.SetVariableValue(BBName_bIsPerformingAbility, false);
                AllyBehaviorTree.SetVariableValue(BBName_AbilityToUse, null);
            }
        }
        #endregion

        #region Initialization
        protected override void SubToEvents()
        {
            base.SubToEvents();
            gamemaster.OnNumberKeyPress += OnKeyPress;
            myEventHandler.EventAllyDied += OnDeath;
        }

        protected override void UnSubFromEvents()
        {
            base.UnSubFromEvents();
            gamemaster.OnNumberKeyPress -= OnKeyPress;
            myEventHandler.EventAllyDied -= OnDeath;
        }
        #endregion

    }
}