//using UnityEngine;
//using System.Collections;
//using RPG.CameraUI; // for mouse events
//using BaseFramework;
//using RTSCoreFramework;
//using RPG.Characters;
////Not using player control script anyways
//using RPGPrototype.OLDAbilities;

//namespace RPGPrototype
//{
//    public class RPGPlayerControl : MonoBehaviour
//    {
//        #region Fields
//        RPGCharacter character;
//        RPGSpecialAbilities abilities;
//        RPGWeaponSystem weaponSystem;

//        bool bIsDead = false;
//        #endregion

//        #region Properties
//        RPGGameMaster gamemaster
//        {
//            get { return RPGGameMaster.thisInstance; }
//        }

//        AllyEventHandlerRPG eventhandler
//        {
//            get
//            {
//                if (_eventhandler == null)
//                    _eventhandler = GetComponent<AllyEventHandlerRPG>();

//                return _eventhandler;
//            }
//        }
//        AllyEventHandlerRPG _eventhandler = null;
//        AllyMemberRPG allymember
//        {
//            get
//            {
//                if (_allymember == null)
//                    _allymember = GetComponent<AllyMemberRPG>();

//                return _allymember;
//            }
//        }
//        AllyMemberRPG _allymember = null;
//        #endregion

//        #region UnityMessages
//        void Start()
//        {
//            character = GetComponent<RPGCharacter>();
//            abilities = GetComponent<RPGSpecialAbilities>();
//            weaponSystem = GetComponent<RPGWeaponSystem>();
//        }

//        void Update()
//        {
//            //ScanForAbilityKeyDown();
//        }

//        /// <summary>
//        /// Temporary Method To Prevent Animation Event Errors
//        /// </summary>
//        public void Hit()
//        {
//            //TODO: RPGPrototype-Find another way to stop Hit Animation Event Errors
//        }

//        private void OnEnable()
//        {
//            gamemaster.OnNumberKeyPress += OnKeyPress;
//            eventhandler.EventCommandMove += OnCommandMove;
//            eventhandler.EventCommandAttackEnemy += OnCommandAttack;
//            eventhandler.EventAllyDied += Die;
//        }

//        private void OnDisable()
//        {
//            gamemaster.OnNumberKeyPress -= OnKeyPress;
//            eventhandler.EventCommandMove -= OnCommandMove;
//            eventhandler.EventCommandAttackEnemy -= OnCommandAttack;
//            eventhandler.EventAllyDied -= Die;
//        }
//        #endregion

//        #region MyHandlers
//        //Custom
//        void OnKeyPress(int _key)
//        {
//            if (bIsDead) return;

//            if (_key == 0 ||
//                _key >= abilities.GetNumberOfAbilities() ||
//                allymember.bIsCurrentPlayer == false) return;

//            abilities.AttemptSpecialAbility(_key);
//        }

//        void OnCommandMove(Vector3 destination)
//        {
//            weaponSystem.StopAttacking();
//            character.SetDestination(destination);
//        }

//        void OnCommandAttack(AllyMember _ally)
//        {
//            if (IsTargetInRange(_ally.gameObject))
//            {
//                weaponSystem.AttackTarget(_ally.gameObject);
//            }
//            else if (!IsTargetInRange(_ally.gameObject))
//            {
//                StartCoroutine(MoveAndAttack(_ally.gameObject));
//            }
//        }

//        void Die(Vector3 position, Vector3 force, GameObject attacker)
//        {
//            bIsDead = true;
//        }
//        #endregion

//        #region OldHandlers
//        //void OnMouseOverPotentiallyWalkable(Vector3 destination)
//        //{
//        //    if (Input.GetMouseButton(0))
//        //    {
//        //        weaponSystem.StopAttacking();
//        //        character.SetDestination(destination);
//        //    }
//        //}

//        //void OnMouseOverEnemy(EnemyAI enemy)
//        //{
//        //    if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
//        //    {
//        //        weaponSystem.AttackTarget(enemy.gameObject);
//        //    }
//        //    else if (Input.GetMouseButton(0) && !IsTargetInRange(enemy.gameObject))
//        //    {
//        //        StartCoroutine(MoveAndAttack(enemy));
//        //    }
//        //    else if (Input.GetMouseButtonDown(1) && IsTargetInRange(enemy.gameObject))
//        //    {
//        //        abilities.AttemptSpecialAbility(0, enemy.gameObject);
//        //    }
//        //    else if (Input.GetMouseButtonDown(1) && !IsTargetInRange(enemy.gameObject))
//        //    {
//        //        StartCoroutine(MoveAndPowerAttack(enemy));
//        //    }
//        //}
//        #endregion

//        #region OldCode
//        //void ScanForAbilityKeyDown()
//        //{
//        //    for (int keyIndex = 1; keyIndex < abilities.GetNumberOfAbilities(); keyIndex++)
//        //    {
//        //        if (Input.GetKeyDown(keyIndex.ToString()))
//        //        {
//        //            abilities.AttemptSpecialAbility(keyIndex);
//        //        }
//        //    }
//        //}

//        //private void RegisterForMouseEvents()
//        //{
//        //    var cameraRaycaster = FindObjectOfType<CameraRaycaster>();
//        //    if (cameraRaycaster != null)
//        //    {
//        //        cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
//        //        cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
//        //    }

//        //}

//        //private void DeregisterForMouseEvents()
//        //{
//        //    var cameraRaycaster = FindObjectOfType<CameraRaycaster>();
//        //    if (cameraRaycaster != null)
//        //    {
//        //        cameraRaycaster.onMouseOverEnemy -= OnMouseOverEnemy;
//        //        cameraRaycaster.onMouseOverPotentiallyWalkable -= OnMouseOverPotentiallyWalkable;
//        //    }
//        //    //Deregister Custom Events
//        //    gamemaster.OnNumberKeyPress -= OnKeyPress;
//        //}
//        #endregion

//        bool IsTargetInRange(GameObject target)
//        {
//            float distanceToTarget = (target.transform.position - transform.position).magnitude;
//            return distanceToTarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
//        }

//        IEnumerator MoveToTarget(GameObject target)
//        {
//            character.SetDestination(target.transform.position);
//            while (!IsTargetInRange(target))
//            {
//                yield return new WaitForEndOfFrame();
//            }
//            yield return new WaitForEndOfFrame();
//        }
//        /// <summary>
//        /// Originally used EnemyAI Parameter
//        /// </summary>
//        /// <param name="enemy"></param>
//        /// <returns></returns>
//        IEnumerator MoveAndAttack(GameObject enemy)
//        {
//            yield return StartCoroutine(MoveToTarget(enemy.gameObject));
//            weaponSystem.AttackTarget(enemy.gameObject);
//        }

//        IEnumerator MoveAndPowerAttack(EnemyAI enemy)
//        {
//            yield return StartCoroutine(MoveToTarget(enemy.gameObject));
//            abilities.AttemptSpecialAbility(0, enemy.gameObject);
//        }
//    }
//}