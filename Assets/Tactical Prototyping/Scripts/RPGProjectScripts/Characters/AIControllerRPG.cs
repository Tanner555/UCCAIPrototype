using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;
using RPG.Characters;
using BehaviorDesigner.Runtime;
#if RTSAStarPathfinding
using Pathfinding;
#endif

namespace RPGPrototype
{
    public class AIControllerRPG : AllyAIController
    {
        #region Fields
        public RTSPrototype.WeaponConfig myRPGWeapon = null;
        //BTs
        private bool bUsingBehaviorTrees = true;
        //Extra
        bool bUseAStarPath = false;
        #if RTSAStarPathfinding
        ABPath myCurrentABPath = null;
        #endif
        LayerMask currWalkLayers;
        int currHitLayer;

        int baseDamage = 10;
        #endregion

        #region ComponentsAndSingletons
        new RPGGameMaster gamemaster
        {
            get { return RPGGameMaster.thisInstance; }
        }

        new RPGGameMode gamemode
        {
            get { return RPGGameMode.thisInstance; }
        }

        new AllyEventHandlerRPG myEventHandler
        {
            get
            {
                if (_eventhandler == null)
                    _eventhandler = GetComponent<AllyEventHandlerRPG>();

                return _eventhandler;
            }
        }
        AllyEventHandlerRPG _eventhandler = null;

        new AllyMemberRPG allyMember
        {
            get
            {
                if (_allymember == null)
                    _allymember = GetComponent<AllyMemberRPG>();

                return _allymember;
            }
        }
        AllyMemberRPG _allymember = null;        

        Animator myAnimator
        {
            get
            {
                if(_myAnimator == null)
                {
                    _myAnimator = GetComponent<Animator>();
                }
                return _myAnimator;
            }
        }
        Animator _myAnimator = null;

        //protected override bool AllCompsAreValid => myEventHandler && allyMember;
        #endregion

        #region Properties       
        BehaviorTree AllyBehaviorTree
        {
            get
            {
                if(_AllyBehaviorTree == null)
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
        public string BBName_MyStationaryTurnSpeed => "MyStationaryTurnSpeed";
        public string BBName_MyMovingTurnSpeed => "MyMovingTurnSpeed";
        public string BBName_MyMoveThreshold => "MyMoveThreshold";
        public string BBName_MyAnimatorForwardCap => "MyAnimatorForwardCap";
        public string BBName_MyAnimationSpeedMultiplier => "MyAnimationSpeedMultiplier";
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

        #if RTSAStarPathfinding
        Seeker mySeeker
        {
            get
            {
                if (_mySeeker == null)
                {
                    _mySeeker = GetComponent<Seeker>();
                }
                return _mySeeker;
            }
        }
        Seeker _mySeeker = null;

        AIPath myAIPath
        {
            get
            {
                if (_myAIPath == null)
                    _myAIPath = GetComponent<AIPath>();

                return _myAIPath;
            }
        }
        AIPath _myAIPath = null;
#endif
        #endregion

        #region UnityMessages
        protected override void Start()
        {
            //Overriding AIController To Remove Checks and StartServices
        }
        #endregion

        #region Getters
        public override bool isSurfaceWalkable(RaycastHit hit)
        {
            if (bUseAStarPath == false)
            {
                return base.isSurfaceWalkable(hit);
            }
            else
            {
                currWalkLayers = gamemode.WalkableLayers;
                currHitLayer = hit.transform.gameObject.layer;
                return currWalkLayers == (currWalkLayers | (1 << currHitLayer));
            }
        }

        public override bool isSurfaceWalkable(Vector3 _point)
        {
            if (bUseAStarPath == false)
            {
                return base.isSurfaceWalkable(_point);
            }
            else
            {
                return false;
            }
        }

        bool IsTargetInRange(GameObject target)
        {
            if (myRPGWeapon == null) return false;
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= myRPGWeapon.GetMaxAttackRange();
        }

        /// <summary>
        /// Doesn't Use AllyMember Values, A Quick Hack To Get Damage Without using other Comps
        /// </summary>
        /// <returns></returns>
        public int GetDamageRate()
        {
            if (myRPGWeapon == null) return baseDamage;

            return baseDamage + myRPGWeapon.GetAdditionalDamage();
        }

        public override float GetAttackRate()
        {
            if (myRPGWeapon != null)
            {
                var animationClip = myRPGWeapon.GetAttackAnimClip();
                float animationClipTime = animationClip.length / myAnimator.speed;/*character.GetAnimSpeedMultiplier()*/
                float _timeToWait = animationClipTime + myRPGWeapon.GetTimeBetweenAnimationCycles();
                return _timeToWait;
            }
            else
            {
                return 0.25f;
            }
        }
        #endregion

        #region Handlers
        protected override void OnAllyInitComps(RTSAllyComponentSpecificFields _specific, RTSAllyComponentsAllCharacterFields _allFields)
        {
            //Make Sure Base is Called Before Override
            base.OnAllyInitComps(_specific, _allFields);
            var _RPGallAllyComps = (AllyComponentsAllCharacterFieldsRPG)_allFields;
            var _rpgCharAttr = ((AllyComponentSpecificFieldsRPG)_specific).RPGCharacterAttributesObject;
            //Set Important NavMeshAgent Attributes
            myNavAgent.speed = _rpgCharAttr.navMeshAgentSteeringSpeed;
            myNavAgent.stoppingDistance = _rpgCharAttr.navMeshAgentStoppingDistance;
            myNavAgent.autoBraking = false;
            myNavAgent.updateRotation = false;
            myNavAgent.updatePosition = true;
            this.bUseAStarPath = _RPGallAllyComps.bUseAStarPath;
            //Behavior Tree Init
            bUsingBehaviorTrees = _RPGallAllyComps.bUseBehaviourTrees;
            if(_RPGallAllyComps.bUseBehaviourTrees && _RPGallAllyComps.allAlliesDefaultBehaviourTree != null)
            {
                BehaviorTree _behaviourtree;
                if(GetComponent<BehaviorTree>() == null)
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
                _behaviourtree.SetVariableValue(BBName_MyStationaryTurnSpeed, _rpgCharAttr.stationaryTurnSpeed);
                _behaviourtree.SetVariableValue(BBName_MyMovingTurnSpeed, _rpgCharAttr.movingTurnSpeed);
                _behaviourtree.SetVariableValue(BBName_MyMoveThreshold, _rpgCharAttr.moveThreshold);
                _behaviourtree.SetVariableValue(BBName_MyAnimatorForwardCap, _rpgCharAttr.animatorForwardCap);
                _behaviourtree.SetVariableValue(BBName_MyAnimationSpeedMultiplier, _rpgCharAttr.animationSpeedMultiplier);
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

        void PutWeaponInHand(RTSPrototype.WeaponConfig _config)
        {
            myRPGWeapon = _config;
        }

        protected override void HandleAllySwitch(PartyManager _party, AllyMember _toSet, AllyMember _current)
        {
            base.HandleAllySwitch(_party, _toSet, _current);
            if (bUsingBehaviorTrees && _party == allyMember.partyManager)
            {
                bool _isCurrentPlayer = _toSet == allyMember && _toSet.bIsInGeneralCommanderParty;
                AllyBehaviorTree.SetVariableValue(BBName_bIsAllyInCommand, _toSet == allyMember);
                AllyBehaviorTree.SetVariableValue(BBName_bIsCurrentPlayer, _isCurrentPlayer);
                //Don't Enable Tactics If Current Player
                //Otherwise, Load and Execute Tactics
                ToggleTactics(_isCurrentPlayer == false);
                if (_toSet.bIsInGeneralCommanderParty)
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

        //protected override void HandleStopTargetting()
        //{
        //    base.HandleStopTargetting();
        //    if (bUsingBehaviorTrees)
        //    {
        //        AllyBehaviorTree.SetVariableValue(BBName_bTargetEnemy, false);
        //        AllyBehaviorTree.SetVariableValue(BBName_CurrentTargettedEnemy, null);
        //    }
        //}

        protected override void OnAllyDeath(Vector3 position, Vector3 force, GameObject attacker)
        {
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
            if(IsPerformingSpecialAbility() == false && allyMember.CanUseAbility(_abilityType, out _config))
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
            if (IsNavMeshAgentEnabled())
            {
                myNavAgent.SetDestination(transform.position);
                myNavAgent.velocity = Vector3.zero;
            }
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

        #region ShootingAndBattleBehavior
        //protected override void UpdateBattleBehavior()
        //{
        //    RPGUpdateBattleBehaviorOLD();
        //}

        //private void RPGUpdateBattleBehaviorOLD()
        //{
        //    // Pause Ally Tactics If Ally Is Paused
        //    // Due to the Game Pausing Or Control Pause Mode
        //    // Is Active
        //    if (myEventHandler.bAllyIsPaused) return;

        //    if (currentTargettedEnemy == null ||
        //        currentTargettedEnemy.IsAlive == false ||
        //        myEventHandler.bIsFreeMoving)
        //    {
        //        myEventHandler.CallEventStopTargettingEnemy();
        //        myEventHandler.CallEventFinishedMoving();
        //        return;
        //    }

        //    if (IsTargetInRange(currentTargettedEnemy.gameObject))
        //    {
        //        if (bIsMeleeing == false)
        //        {
        //            //myEventHandler.CallAttackRPGTarget(currentTargettedEnemy.gameObject);
        //            myEventHandler.CallEventFinishedMoving();
        //            StartMeleeAttackBehavior();
        //        }
        //    }
        //    else
        //    {
        //        if (bIsMeleeing == true)
        //        {
        //            //myEventHandler.CallStopAttackingRPGTarget();
        //            StopMeleeAttackBehavior();
        //        }

        //        myEventHandler.CallEventAIMove(currentTargettedEnemy.transform.position);
        //    }
        //}

        ////Probably Won't Use, From RTSCoreFramework AIController
        //private void RPGUpdateBattleBehaviorNEW()
        //{
        //    // Pause Ally Tactics If Ally Is Paused
        //    // Due to the Game Pausing Or Control Pause Mode
        //    // Is Active
        //    if (myEventHandler.bAllyIsPaused) return;

        //    if (bStopUpdatingBattleBehavior)
        //    {
        //        myEventHandler.CallEventStopTargettingEnemy();
        //        myEventHandler.CallEventFinishedMoving();
        //        return;
        //    }

        //    //Melee Behavior
        //    if (IsTargetInMeleeRange(currentTargettedEnemy.gameObject))
        //    {
        //        if (bIsMeleeing == false)
        //        {
        //            StartMeleeAttackBehavior();
        //            myEventHandler.CallEventFinishedMoving();
        //        }
        //    }
        //    else
        //    {
        //        if (bIsMeleeing == true)
        //        {
        //            StopMeleeAttackBehavior();
        //        }

        //        myEventHandler.CallEventAIMove(currentTargettedEnemy.transform.position);
        //    }
        //}
        #endregion

        #region Initialization
        protected override void SubToEvents()
        {
            base.SubToEvents();
            //myEventHandler.PutRPGWeaponInHand += PutWeaponInHand;
            gamemaster.OnNumberKeyPress += OnKeyPress;
        }

        protected override void UnSubFromEvents()
        {
            base.UnSubFromEvents();
            //myEventHandler.PutRPGWeaponInHand -= PutWeaponInHand;
            gamemaster.OnNumberKeyPress -= OnKeyPress;
        }
        #endregion

        /// <summary>
        /// Temporary Method To Prevent Animation Event Errors
        /// </summary>
        public void Hit()
        {
            //TODO: RPGPrototype-Find another way to stop Hit Animation Event Errors
        }

    }
}