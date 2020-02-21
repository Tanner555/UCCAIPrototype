using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using RTSCoreFramework;
using RPG.Characters;
#if RTSAStarPathfinding
using Pathfinding;
#endif
using Sirenix.OdinInspector;
using BehaviorDesigner.Runtime;

namespace RPGPrototype
{
    #region RTSAllyComponentSpecificFields
    [System.Serializable]
    public class AllyComponentSpecificFieldsRPG : RTSAllyComponentSpecificFields
    {
        [Header("RPG Character Attributes")]
        [SerializeField]
        public RPGAllySpecificCharacterAttributesObject RPGCharacterAttributesObject;
        [SerializeField]
        public RPGAllySpecificCharacterAttributesObject ASTAR_RPGCharacterAttributesObject;
    }
    #endregion

    #region RTSAllyComponentsAllCharacterFields
    [System.Serializable]
    public class AllyComponentsAllCharacterFieldsRPG : RTSAllyComponentsAllCharacterFields
    {
        [Header("Behaviour Designer Settings")]
        public bool bUseBehaviourTrees = true;
        public ExternalBehaviorTree allAlliesDefaultBehaviourTree;
        [FoldoutGroup("AStar PathFinding Settings")]
        [Header("AStar PathFinding Settings")]
        public bool bUseAStarPath = false;
        [FoldoutGroup("AStar PathFinding Settings")]
        public string aStar_traversableGraphs = "RTS Graph";
        [FoldoutGroup("AStar PathFinding Settings")]
        public float aStar_Radius = 0.5f;
        [FoldoutGroup("AStar PathFinding Settings")] public float aStar_Height = 1.8f;
        [FoldoutGroup("AStar PathFinding Settings")] public bool aStar_CanSearch = true;
        [FoldoutGroup("AStar PathFinding Settings")] public float aStar_RepathRate = 0.5f;
        [FoldoutGroup("AStar PathFinding Settings")] public bool aStar_CanMove = false;
        [FoldoutGroup("AStar PathFinding Settings")] public float aStar_MaxSpeed = 1;
#if RTSAStarPathfinding
        [FoldoutGroup("AStar PathFinding Settings")] public OrientationMode aStar_Orientation;
#endif
        [FoldoutGroup("AStar PathFinding Settings")] public bool aStar_EnableRotation = false;
        [FoldoutGroup("AStar PathFinding Settings")] public float aStar_PickNextWaypointDistance = 0.75f;
        [FoldoutGroup("AStar PathFinding Settings")] public float aStar_SlowdownDistance = 0.6f;
        [FoldoutGroup("AStar PathFinding Settings")] public float aStar_EndReachedDistance = 1;
        [FoldoutGroup("AStar PathFinding Settings")] public bool aStar_AlwaysDrawGizmos = false;
#if RTSAStarPathfinding
        [FoldoutGroup("AStar PathFinding Settings")] public CloseToDestinationMode aStar_CloseToDestination;
#endif
        [FoldoutGroup("AStar PathFinding Settings")] public bool aStar_ConstrainInsideGraph = false;
    }
    #endregion

    public class AllyEventHandlerRPG : AllyEventHandler
    {
        #region Fields
        public bool bIsMeleeingEnemy = false;
        #endregion

        #region Delegates
        //public delegate void OneGameObjectParamHandler(GameObject _target);
        //public event OneGameObjectParamHandler AttackRPGTarget;
        //public event GeneralEventHandler StopAttackingRPGTarget;

        public delegate void WeaponConfigParamHandler(WeaponConfig weaponToUse);
        public event WeaponConfigParamHandler PutRPGWeaponInHand;
        #endregion

        #region Calls
        //public void CallAttackRPGTarget(GameObject _target)
        //{
        //    bIsMeleeingEnemy = true;
        //    if (AttackRPGTarget != null) AttackRPGTarget(_target);
        //}

        //public void CallStopAttackingRPGTarget()
        //{
        //    bIsMeleeingEnemy = false;
        //    if (StopAttackingRPGTarget != null) StopAttackingRPGTarget();
        //}

        public void CallPutRPGWeaponInHand(WeaponConfig _weapon)
        {
            if (PutRPGWeaponInHand != null) PutRPGWeaponInHand(_weapon);
        }
        #endregion

        #region OverrideCalls
        protected override void CallEventCommandMove(Vector3 _point, bool _isCommandMove)
        {
            //CallStopAttackingRPGTarget();
            base.CallEventCommandMove(_point, _isCommandMove);
        }

        //public override void CallEventStopTargettingEnemy()
        //{
        //    CallStopAttackingRPGTarget();
        //    base.CallEventStopTargettingEnemy();
        //}
        #endregion
    }
}