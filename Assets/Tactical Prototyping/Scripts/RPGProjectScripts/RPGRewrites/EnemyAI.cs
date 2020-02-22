using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;
using RTSCoreFramework;

namespace RPGPrototype
{
    //[RequireComponent(typeof(HealthSystem))]
    //[RequireComponent(typeof(Character))]
    //[RequireComponent(typeof(WeaponSystem))]

    // TODO consider specialising to NPCMovement
    public class EnemyAI : MonoBehaviour
    {
        //#region Fields
        ////Used For Services
        //float aiRepeatRate = 0.05f;

        //[SerializeField] float chaseRadius = 6f;
        //[SerializeField] WaypointContainer patrolPath;
        //[SerializeField] float waypointTolerance = 2.0f;
        //[SerializeField] float waypointDwellTime = 2.0f;

        //GameObject player = null;
        //Character character;
        //int nextWaypointIndex;
        //float currentWeaponRange;
        //float distanceToPlayer;

        //enum State { idle, patrolling, attacking, chasing }
        //State state = State.idle;
        //#endregion

        //#region Properties
        //RTSGameMode gamemode
        //{
        //    get { return RTSGameMode.thisInstance; }
        //}

        //RTSGameMaster gamemaster
        //{
        //    get { return RTSGameMaster.thisInstance; }
        //}
        //#endregion

        //private void OnEnable()
        //{
        //    gamemaster.OnAllySwitch += OnAllySwitch;
        //}

        //private void OnDisable()
        //{
        //    gamemaster.OnAllySwitch -= OnAllySwitch;
        //}

        //void Start()
        //{
        //    character = GetComponent<Character>();
        //    InvokeRepeating("SE_UpdateEnemyAI", 1f, aiRepeatRate);
        //}

        //void SE_UpdateEnemyAI()
        //{
        //    distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        //    WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
        //    currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();

        //    bool inWeaponCircle = distanceToPlayer <= currentWeaponRange;
        //    bool inChaseRing = distanceToPlayer > currentWeaponRange
        //                         &&
        //                         distanceToPlayer <= chaseRadius;
        //    bool outsideChaseRing = distanceToPlayer > chaseRadius;

        //    if (outsideChaseRing)
        //    {
        //        StopAllCoroutines();
        //        weaponSystem.StopAttacking();
        //        StartCoroutine(Patrol());
        //    }
        //    if (inChaseRing)
        //    {
        //        StopAllCoroutines();
        //        weaponSystem.StopAttacking();
        //        StartCoroutine(ChasePlayer());
        //    }
        //    if (inWeaponCircle)
        //    {
        //        StopAllCoroutines();
        //        state = State.attacking;
        //        weaponSystem.AttackTarget(player.gameObject);
        //    }
        //}

        ////void Update()
        ////{
            
        ////}

        //IEnumerator Patrol()
        //{
        //    state = State.patrolling;

        //    while (patrolPath != null)
        //    {
        //        Vector3 nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
        //        character.SetDestination(nextWaypointPos);
        //        CycleWaypointWhenClose(nextWaypointPos);
        //        yield return new WaitForSeconds(waypointDwellTime);
        //    }
        //}

        //private void CycleWaypointWhenClose(Vector3 nextWaypointPos)
        //{
        //    if (Vector3.Distance(transform.position, nextWaypointPos) <= waypointTolerance)
        //    {
        //        nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
        //    }
        //}

        //IEnumerator ChasePlayer()
        //{
        //    state = State.chasing;
        //    while (distanceToPlayer >= currentWeaponRange)
        //    {
        //        character.SetDestination(player.transform.position);
        //        yield return new WaitForEndOfFrame();
        //    }
        //}

        //void OnAllySwitch(PartyManager _party, AllyMember _toSet, AllyMember _current)
        //{
        //    if (_party.bIsCurrentPlayerCommander)
        //    {
        //        player = _toSet.gameObject;
        //    }
        //}

        //void OnDrawGizmos()
        //{
        //    // Draw attack sphere 
        //    Gizmos.color = new Color(255f, 0, 0, .5f);
        //    Gizmos.DrawWireSphere(transform.position, currentWeaponRange);

        //    // Draw chase sphere 
        //    Gizmos.color = new Color(0, 0, 255, .5f);
        //    Gizmos.DrawWireSphere(transform.position, chaseRadius);
        //}
    }
}