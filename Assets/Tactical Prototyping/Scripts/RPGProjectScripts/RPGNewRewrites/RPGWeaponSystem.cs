using System.Collections;
using UnityEngine.Assertions;
using UnityEngine;
using RTSCoreFramework;
using RPG.Characters;

namespace RPGPrototype
{
    public class RPGWeaponSystem : MonoBehaviour
    {
        #region Properties
        RPGGameMaster gamemaster
        {
            get { return RPGGameMaster.thisInstance; }
        }

        AllyEventHandlerRPG eventhandler
        {
            get
            {
                if (_eventhandler == null)
                    _eventhandler = GetComponent<AllyEventHandlerRPG>();

                return _eventhandler;
            }
        }
        AllyEventHandlerRPG _eventhandler = null;

        AllyMemberRPG allymember
        {
            get
            {
                if (_allymember == null)
                    _allymember = GetComponent<AllyMemberRPG>();

                return _allymember;
            }
        }
        AllyMemberRPG _allymember = null;
        #endregion

        #region Fields
        float checkForAttackRate = 0.05f;

        [SerializeField] float baseDamage = 10f;
        [SerializeField] RTSPrototype.WeaponConfig currentWeaponConfig = null;

        //GameObject target;
        GameObject weaponObject;
        Animator animator;
        RPGCharacter character;
        float lastHitTime;

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";
        #endregion

        #region UnityMessages
        //void Start()
        //{
     
        //}

        private void OnEnable()
        {
            //eventhandler.StopAttackingRPGTarget += OnStopAttacking;
            eventhandler.OnTryUseWeapon += CheckTargetAndAttackEnemy;
            //eventhandler.AttackRPGTarget += AttackTarget;
            eventhandler.InitializeAllyComponents += OnInitializeAllyComponents;
        }

        private void OnDisable()
        {
            //eventhandler.StopAttackingRPGTarget -= OnStopAttacking;
            eventhandler.OnTryUseWeapon -= CheckTargetAndAttackEnemy;
            //eventhandler.AttackRPGTarget -= AttackTarget;
            eventhandler.InitializeAllyComponents -= OnInitializeAllyComponents;
        }

        //void Update()
        //{
        //    //SE_CheckForAttack();
        //}
        #endregion

        #region Handlers
        private void OnInitializeAllyComponents(RTSAllyComponentSpecificFields _specificComps, RTSAllyComponentsAllCharacterFields _allAllyComps)
        {
            var _RPGallAllyComps = (AllyComponentsAllCharacterFieldsRPG)_allAllyComps;
            if (_specificComps.bBuildCharacterCompletely)
            {
                var _rpgCharAttr = _RPGallAllyComps.bUseAStarPath == false ?
                    ((AllyComponentSpecificFieldsRPG)_specificComps).RPGCharacterAttributesObject :
                    ((AllyComponentSpecificFieldsRPG)_specificComps).ASTAR_RPGCharacterAttributesObject;
                
                this.baseDamage = _rpgCharAttr.baseDamage;
                //This needs to be fixed.
                //if(_rpgCharAttr.currentWeaponConfig != null)
                //{
                //    this.currentWeaponConfig = _rpgCharAttr.currentWeaponConfig;
                //}
            }
            animator = GetComponent<Animator>();
            character = GetComponent<RPGCharacter>();

            PutWeaponInHand(currentWeaponConfig);
            SetAttackAnimation();

            //InvokeRepeating("SE_CheckForAttack", 1f, checkForAttackRate);
        }

        void CheckTargetAndAttackEnemy(Transform target)
        {
            //eventhandler.CallOnActiveTimeBarDepletion();

            if (target == null) return;

            AttackTargetOnce(target);
        }

        //void OnStopAttacking()
        //{
        //    StopAttacking();
        //}
        #endregion

        #region Services
        ////void SE_CheckForAttack()
        ////{
        ////    bool targetIsOutOfRange;

        ////    if (target == null)
        ////    {
        ////        //targetIsDead = false;
        ////        targetIsOutOfRange = false;
        ////    }
        ////    else
        ////    {
        ////        // test if target is dead
        ////        //var targethealth = allymember.healthAsPercentage;
        ////        //targetIsDead = targethealth <= Mathf.Epsilon;

        ////        // test if target is out of range
        ////        var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        ////        targetIsOutOfRange = distanceToTarget > currentWeaponConfig.GetMaxAttackRange();
        ////    }
        ////    ///float characterHealth
        ////    bool characterIsDead = allymember == null ||
        ////        allymember.IsAlive == false;
        ////    //bool characterIsDead = (characterHealth <= Mathf.Epsilon);

        ////    if (characterIsDead || targetIsOutOfRange || targetIsDead(target))
        ////    {
        ////        StopAllCoroutines();
        ////    }
        ////}
        #endregion

        #region Helpers
        bool targetIsDead(Transform target)
        {
            if (target == null) return false;
            var _ally = target.GetComponent<AllyMemberRPG>();
            if (_ally != null)
            {
                return target.GetComponent<AllyMemberRPG>().IsAlive == false;
            }
            return false;
        }
        #endregion

        public void PutWeaponInHand(RTSPrototype.WeaponConfig weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            Destroy(weaponObject); // empty hands
            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
            //Needs to be fixed
            //eventhandler.CallPutRPGWeaponInHand(currentWeaponConfig);
        }

        //public void AttackTarget(GameObject targetToAttack)
        //{
        //    StartCoroutine(AttackTargetRepeatedly());
        //}

        //public void StopAttacking()
        //{
        //    animator.StopPlayback();
        //    StopAllCoroutines();
        //}

        IEnumerator AttackTargetRepeatedly(Transform target)
        {
            // determine if alive (attacker and defender)
            var _targetAlly = target.GetComponent<AllyMemberRPG>();
            while(allymember != null &&
                allymember.IsAlive && 
                _targetAlly != null &&
                _targetAlly.IsAlive)
            {
                var animationClip = currentWeaponConfig.GetAttackAnimClip();                
                float animationClipTime = animationClip.length / character.GetAnimSpeedMultiplier();
                float timeToWait = animationClipTime + currentWeaponConfig.GetTimeBetweenAnimationCycles();

                bool isTimeToHitAgain = Time.time - lastHitTime > timeToWait;

                if (isTimeToHitAgain)
                {
                    AttackTargetOnce(target);
                    lastHitTime = Time.time;
                }
                yield return new WaitForSeconds(timeToWait);
            }
        }

        void AttackTargetOnce(Transform target)
        {
            transform.LookAt(target);
            animator.SetTrigger(ATTACK_TRIGGER);
            //float damageDelay = currentWeaponConfig.GetDamageDelay();
            SetAttackAnimation();
            //StartCoroutine(DamageAfterDelay(damageDelay, target));
        }

        //IEnumerator DamageAfterDelay(float delay, Transform target)
        //{
        //    yield return new WaitForSecondsRealtime(delay);
        //    DamageAlly(target.gameObject, (int)CalculateDamage());
        //}

        public RTSPrototype.WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig;
        }

        void SetAttackAnimation()
        {
            if (!character.GetOverrideController())
            {
                Debug.Break();
                Debug.LogAssertion("Please provide " + gameObject + " with an animator override controller.");
            }
            else
            {
                var animatorOverrideController = character.GetOverrideController();
                animator.runtimeAnimatorController = animatorOverrideController;
                animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip();
            }
        }

        GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            Assert.IsFalse(numberOfDominantHands <= 0, "No DominantHand found on " + gameObject.name + ", please add one");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple DominantHand scripts on " + gameObject.name + ", please remove one");
            return dominantHands[0].gameObject;
        }

        float CalculateDamage()
        {
            return baseDamage + currentWeaponConfig.GetAdditionalDamage();
        }

        //Custom Methods
        void DamageAlly(GameObject _allyObject, int _damage)
        {
            AllyMemberRPG _ally = _allyObject.GetComponent<AllyMemberRPG>();
            _ally.AllyTakeDamage(_damage, allymember);
        }

        float GetAllyHealthAsPercent(GameObject _allyObject)
        {
            AllyMemberRPG _ally = _allyObject.GetComponent<AllyMemberRPG>();
            return _ally.healthAsPercentage;
        }
    }
}