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

        protected override bool AllCompsAreValid
        {
            get
            {
                return myRigidbodyTPC && myInventory /*&& itemHandler*/
                    && myNavAgent /*&& myRTSNavBridge*/ && myEventHandler
                    && allyMember;
            }
        }

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
            AllyBehaviorTree.EnableBehavior();
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

        #region Initialization
        protected override void SubToEvents()
        {
            base.SubToEvents();
            myEventHandler.EventAllyDied += OnDeath;
        }

        protected override void UnSubFromEvents()
        {
            base.UnSubFromEvents();
            myEventHandler.EventAllyDied -= OnDeath;
        }
        #endregion

    }
}