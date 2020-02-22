using UnityEngine;
using UnityEngine.AI;
using RTSCoreFramework;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
#if RTSAStarPathfinding
using Pathfinding;
#endif

namespace RTSPrototype
{
    //[SelectionBase]
    public class RPGCharacter : MonoBehaviour
    {
        #region Fields
        //Init Field
        bool bInitializedAlly = false;
        //Used For Character Death
        [Header("Character Death")]
        //[SerializeField] AudioClip[] damageSounds;
        //[SerializeField] AudioClip[] deathSounds;
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

        //Set by Init Handler, Should Be False by default
        bool bChangeAnimAvatar = false;
        #endregion

        #region Properties
        //Non-UCC RPG Accessor Properties
        public float MyStationaryTurnSpeed { get; protected set; }
        public float MyMovingTurnSpeed { get; protected set; }
        public float MyMoveThreshold { get; protected set; }
        public float MyAnimatorForwardCap { get; protected set; }
        public float MyAnimationSpeedMultiplier { get; protected set; }

        AllyEventHandlerWrapper eventHandler
        {
            get
            {
                if (_eventHandler == null)
                {
                    _eventHandler = GetComponent<AllyEventHandlerWrapper>();
                }
                return _eventHandler;
            }
        }
        AllyEventHandlerWrapper _eventHandler = null;

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

            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.spatialBlend = audioSourceSpatialBlend;

            animator = gameObject.GetComponent<Animator>();
            if (animator == null)
            {
                animator = gameObject.AddComponent<Animator>();
            }
            animator.runtimeAnimatorController = animatorController;
            if (bChangeAnimAvatar)
            {
                //most time, don't want to change anim avatar
                animator.avatar = characterAvatar;
            }
        }
        #endregion

        #region Handlers
        private void OnInitializeAllyComponents(RTSAllyComponentSpecificFields _specificComps, RTSAllyComponentsAllCharacterFields _allAllyComps)
        {
            var _RPGallAllyComps = (RTSAllyComponentsAllCharacterFieldsWrapper)_allAllyComps;
            var _RPGspecificAllyComps = (RTSAllyComponentSpecificFieldsWrapper)_specificComps;

            if (_RPGspecificAllyComps.bBuildNonUCCCharacterCompletely)
            {
                //var _rpgCharAttr = ((AllyComponentSpecificFieldsRPG)_specificComps).RPGCharacterAttributesObject;
                var _rpgCharAttr = _RPGspecificAllyComps.RPGCharacterAttributesObject;                
                //this.damageSounds = _RPGallAllyComps.damageSounds.AudioClips;
                //this.deathSounds = _RPGallAllyComps.deathSounds.AudioClips;
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
                this.bChangeAnimAvatar = _RPGspecificAllyComps.bChangeNonUCCCharacterAnimAvatar;
                //RPG Character Moving
                this.MyStationaryTurnSpeed = _rpgCharAttr.stationaryTurnSpeed;
                this.MyMovingTurnSpeed = _rpgCharAttr.movingTurnSpeed;
                this.MyMoveThreshold = _rpgCharAttr.moveThreshold;
                this.MyAnimatorForwardCap = _rpgCharAttr.animatorForwardCap;
                this.MyAnimationSpeedMultiplier = _rpgCharAttr.animationSpeedMultiplier;
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

            //audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
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
