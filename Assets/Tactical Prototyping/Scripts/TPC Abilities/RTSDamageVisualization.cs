using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;
using Opsive.UltimateCharacterController.Character.Abilities;
using Chronos;
using Opsive.UltimateCharacterController.Audio;

namespace RTSPrototype
{
    [DefaultStartType(AbilityStartType.Manual)]
    [DefaultStopType(AbilityStopType.Manual)]
    [DefaultAbilityIndex(203)]
    public class RTSDamageVisualization : Ability
    {
        #region Properties
        AllyEventHandler myEventHandler
        {
            get
            {
                if (_myEventHandler == null)
                    _myEventHandler = GetComponent<AllyEventHandler>();

                return _myEventHandler;
            }
        }
        AllyEventHandler _myEventHandler = null;

        Timeline myTimeline
        {
            get
            {
                if (_myTimeline == null)
                    _myTimeline = GetComponent<Timeline>();

                return _myTimeline;
            }
        }
        Timeline _myTimeline = null;
        #endregion

        #region Fields
        [SerializeField]
        AudioClipSet damageSounds = new AudioClipSet();
        AudioSource damageSoundSource;

        string damageReactLeft = "DamageReactLeft";
        string damageReactRight = "DamageReactRight";
        string damageReactGut = "DamageReactGut";

        int m_TakeDamageIndex = -1;

        float largeDamageAmount = 80f;
        float damageTime = 1.5f;

        enum RTSDamageType { DamageReactLeft = 0, DamageReactRight = 1, DamageReactGut = 2 }
        RTSDamageType m_DamageType = RTSDamageType.DamageReactLeft;

        bool damageIsLeft = true;
        #endregion

        #region UnityMessages
        /// <summary>
        /// Register for any events that the ability should be aware of.
        /// </summary>
        public override void Awake()
        {
            m_CharacterLocomotion.StartCoroutine(AbilityStartedDelayCoroutine());
        }

        IEnumerator AbilityStartedDelayCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            myEventHandler.OnAllyAfterTakeDamage += TookDamage;
        }

        /// <summary>
        /// Unregister for any events that the ability was registered for.
        /// </summary> 
        public override void OnDestroy()
        {
            if (myEventHandler != null)
            {
                myEventHandler.OnAllyAfterTakeDamage -= TookDamage;
            }
        }
        #endregion

        #region Overrides
        public override bool IsConcurrent => true;
        public override int AbilityIntData { get { return m_TakeDamageIndex; } }

        public override bool CanStartAbility()
        {
            return base.CanStartAbility() && this.IsActive == false;
        }
        #endregion

        #region Handlers
        /// <summary>
        /// The character took some damage at the specified position. Apply the animation in direction of the damage.
        /// </summary>
        private void TookDamage(int amount, Vector3 position, Vector3 force, AllyMember _instigator, Collider hitCollider)
        {
            if(damageSoundSource != null && damageSoundSource.isPlaying)
            {
                damageSoundSource.Stop();
            }
            damageSoundSource = damageSounds.PlayAudioClip(m_GameObject);

            if (CanStartAbility() == false) return;

            if (amount >= largeDamageAmount)
            {
                m_DamageType = RTSDamageType.DamageReactGut;
            }
            else
            {
                m_DamageType = damageIsLeft ? RTSDamageType.DamageReactLeft : RTSDamageType.DamageReactRight;
            }

            m_TakeDamageIndex = GetDamageIndexFromType(m_DamageType);

            StartAbility();
            m_CharacterLocomotion.StartCoroutine(DamageVisualizationComplete());            
        }

        /// <summary>
        /// Callback when the animation is complete. Stop the ability.
        /// </summary>
        IEnumerator DamageVisualizationComplete()
        {
            if (myTimeline != null)
            {
                yield return myTimeline.WaitForSeconds(damageTime);
            }
            else
            {
                Debug.LogWarning("Timeline Not Found From RTSDamageVisualization.");
                yield return new WaitForSeconds(damageTime);
            }
            ToggleDamageIsLeft();
            if (damageSoundSource != null && damageSoundSource.isPlaying)
            {
                damageSoundSource.Stop();
            }
            StopAbility();
        }
        #endregion

        #region GettersAndToggles
        int GetDamageIndexFromType(RTSDamageType _type)
        {
            switch (_type)
            {
                case RTSDamageType.DamageReactLeft:
                    return 0;
                case RTSDamageType.DamageReactRight:
                    return 1;
                case RTSDamageType.DamageReactGut:
                    return 2;
                default:
                    return -1;
            }
        }

        string GetAbilityStringFromDamageType()
        {
            switch (m_DamageType)
            {
                case RTSDamageType.DamageReactLeft:
                    return damageReactLeft;
                case RTSDamageType.DamageReactRight:
                    return damageReactRight;
                case RTSDamageType.DamageReactGut:
                    return damageReactGut;
                default:
                    return "";
            }
        }

        void ToggleDamageIsLeft()
        {
            damageIsLeft = !damageIsLeft;
        }
        #endregion

    }
}