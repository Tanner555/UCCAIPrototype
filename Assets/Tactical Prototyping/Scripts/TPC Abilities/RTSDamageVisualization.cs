using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;
using Opsive.ThirdPersonController;
using Opsive.ThirdPersonController.Abilities;

namespace RTSPrototype
{
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
        #endregion

        #region Fields
        string damageReactLeft = "DamageReactLeft";
        string damageReactRight = "DamageReactRight";
        string damageReactGut = "DamageReactGut";

        float largeDamageAmount = 80f;
        float damageTime = 0.8f;

        enum RTSDamageType { DamageReactLeft = 0, DamageReactRight = 1, DamageReactGut = 2 }
        RTSDamageType m_DamageType = RTSDamageType.DamageReactLeft;

        bool damageIsLeft = true;
        #endregion

        #region UnityMessages
        /// <summary>
        /// Register for any events that the ability should be aware of.
        /// </summary>
        private void OnEnable()
        {
            myEventHandler.OnAllyTakeDamage += TookDamage;
        }

        /// <summary>
        /// Unregister for any events that the ability was registered for.
        /// </summary>
        private void OnDisable()
        {
            myEventHandler.OnAllyTakeDamage -= TookDamage;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Returns the destination state for the given layer.
        /// </summary>
        /// <param name="layer">The Animator layer index.</param>
        /// <returns>The state that the Animator should be in for the given layer. An empty string indicates no change.</returns>
        public override string GetDestinationState(int layer)
        {
            // Only the additive layer can play a damage animation.
            if (layer != m_AnimatorMonitor.AdditiveLayerIndex)
            {
                return string.Empty;
            }

            return GetAbilityStringFromDamageType();
        }

        /// <summary>
        /// Can this ability run at the same time as another ability?
        /// </summary>
        /// <returns>True if this ability can run with another ability.</returns>
        public override bool IsConcurrentAbility()
        {
            return true;
        }

        /// <summary>
        /// Should IK at the specified layer be used?
        /// </summary>
        /// <param name="layer">The IK layer in question.</param>
        /// <returns>True if the IK should be used.</returns>
        public override bool CanUseIK(int layer)
        {
            return false;
        }
        #endregion

        #region Handlers
        /// <summary>
        /// The character took some damage at the specified position. Apply the animation in direction of the damage.
        /// </summary>
        /// <param name="amount">The total amount of damage inflicted on the character.</param>
        /// <param name="position">The position that the character took the damage.</param>
        /// <param name="force">The amount of force applied to the object while taking the damage.</param>
        /// <param name="attacker">The GameObject that did the damage.</param>
        private void TookDamage(int amount, Vector3 position, Vector3 force, AllyMember _instigator, GameObject hitGameObject)
        {
            if (amount >= largeDamageAmount)
            {
                m_DamageType = RTSDamageType.DamageReactGut;
            }

            StartAbility();
            Invoke("DamageVisualizationComplete", damageTime);
        }

        /// <summary>
        /// Callback when the animation is complete. Stop the ability.
        /// </summary>
        private void DamageVisualizationComplete()
        {
            ToggleDamageIsLeft();
            m_DamageType = damageIsLeft ? RTSDamageType.DamageReactLeft : RTSDamageType.DamageReactRight;
            StopAbility();
        }
        #endregion

        #region GettersAndToggles
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