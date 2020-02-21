using UnityEngine;
using UnityEngine.AI;
using RTSCoreFramework;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
#if RTSAStarPathfinding
using Pathfinding;
#endif

namespace RPGPrototype
{
    [SelectionBase]
    public class RPGCharacter : MonoBehaviour
    {
        #region Fields
        //Init Field
        bool bInitializedAlly = false;
        //Used For Character Death
        [Header("Character Death")]
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;
        [SerializeField] float deathVanishSeconds = 2.0f;

        const string DEATH_TRIGGER = "Death";

        AudioSource audioSource;
        // TODO consider a CharacterConfig SO
        [Header("Animator")] [SerializeField] RuntimeAnimatorController animatorController;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] Avatar characterAvatar;
        [SerializeField] [Range(.1f, 1f)] float animatorForwardCap = 1f;

        [Header("Audio")]
        [SerializeField] float audioSourceSpatialBlend = 0.5f;

        [Header("Capsule Collider")]
        [SerializeField] Vector3 colliderCenter = new Vector3(0, 1.03f, 0);
        [SerializeField] float colliderRadius = 0.2f;
        [SerializeField] float colliderHeight = 2.03f;

        [Header("Movement")]
        [SerializeField] float moveSpeedMultiplier = .7f;

        // private instance variables for state
        float turnAmount;
        float forwardAmount;
        bool isAlive = true;

        // cached references for readability
        Animator animator;

        private Vector3 myAnimMoveVelocity = Vector3.zero;
        #endregion

        #region Properties
        AllyEventHandlerRPG eventHandler
        {
            get
            {
                if (_eventHandler == null)
                {
                    _eventHandler = GetComponent<AllyEventHandlerRPG>();
                }
                return _eventHandler;
            }
        }
        AllyEventHandlerRPG _eventHandler = null;

        CapsuleCollider capsuleCollider
        {
            get
            {
                if (_capsuleCollider == null)
                    _capsuleCollider = GetComponent<CapsuleCollider>();

                if (_capsuleCollider == null)
                {
                    //CapsuleCollider hasn't been added yet.
                    _capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
                }

                return _capsuleCollider;
            }
        }
        CapsuleCollider _capsuleCollider = null;

        Rigidbody ridigBody
        {
            get
            {
                if (_ridigBody == null)
                    _ridigBody = GetComponent<Rigidbody>();

                if (_ridigBody == null)
                {
                    //Rigidbody hasn't been added yet.
                    _ridigBody = gameObject.AddComponent<Rigidbody>();
                }

                return _ridigBody;
            }
        }
        Rigidbody _ridigBody = null;
        #endregion

        #region UnityMessages
        private void OnEnable()
        {
            eventHandler.EventAllyDied += Kill;
            eventHandler.InitializeAllyComponents += OnInitializeAllyComponents;
        }

        private void OnDisable()
        {
            eventHandler.EventAllyDied -= Kill;
            eventHandler.InitializeAllyComponents -= OnInitializeAllyComponents;
        }
        
        void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (Time.deltaTime > 0)
            {
                myAnimMoveVelocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                myAnimMoveVelocity.y = ridigBody.velocity.y;
                ridigBody.velocity = myAnimMoveVelocity;
            }
        }
        #endregion

        #region Initialization
        private void AddRequiredComponents()
        {
            capsuleCollider.center = colliderCenter;
            capsuleCollider.radius = colliderRadius;
            capsuleCollider.height = colliderHeight;

            ridigBody.constraints = RigidbodyConstraints.FreezeRotation;

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = audioSourceSpatialBlend;

            animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            animator.avatar = characterAvatar;
        }
        #endregion

        #region Handlers
        private void OnInitializeAllyComponents(RTSAllyComponentSpecificFields _specificComps, RTSAllyComponentsAllCharacterFields _allAllyComps)
        {
            var _RPGallAllyComps = (AllyComponentsAllCharacterFieldsRPG)_allAllyComps;

            if (_specificComps.bBuildCharacterCompletely)
            {                
                var _rpgCharAttr = ((AllyComponentSpecificFieldsRPG)_specificComps).RPGCharacterAttributesObject;
                this.damageSounds = _rpgCharAttr.damageSounds;
                this.deathSounds = _rpgCharAttr.deathSounds;
                this.deathVanishSeconds = _rpgCharAttr.deathVanishSeconds;
                this.animatorController = _rpgCharAttr.animatorController;
                this.animatorOverrideController = _rpgCharAttr.animatorOverrideController;
                this.characterAvatar = _rpgCharAttr.characterAvatar;
                this.animatorForwardCap = _rpgCharAttr.animatorForwardCap;
                this.audioSourceSpatialBlend = _rpgCharAttr.audioSourceSpatialBlend;
                this.colliderCenter = _rpgCharAttr.colliderCenter;
                this.colliderRadius = _rpgCharAttr.colliderRadius;
                this.colliderHeight = _rpgCharAttr.colliderHeight;
                this.moveSpeedMultiplier = _rpgCharAttr.moveSpeedMultiplier;
            }

            AddRequiredComponents();
            bInitializedAlly = true;
        }

        public void Kill(Vector3 position, Vector3 force, GameObject attacker)
        {
            isAlive = false;
            StartCoroutine(KillCharacter());
        }

        IEnumerator KillCharacter()
        {
            animator.SetTrigger(DEATH_TRIGGER);

            audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
            audioSource.Play(); // overrind any existing sounds
            yield return new WaitForSecondsRealtime(audioSource.clip.length);

            UnityEngine.Object.Destroy(gameObject, deathVanishSeconds);
        }
        #endregion

        #region Getters
        public float GetAnimSpeedMultiplier()
        {
            return animator.speed;
        }

        public AnimatorOverrideController GetOverrideController()
        {
            return animatorOverrideController;
        }
        #endregion
    }
}
