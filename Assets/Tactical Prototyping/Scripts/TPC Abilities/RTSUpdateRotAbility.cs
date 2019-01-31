using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;
using RTSCoreFramework;
using Opsive.UltimateCharacterController.Utility;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Events;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character.Abilities.Items;

namespace RTSPrototype
{
    public class RTSUpdateRotAbility : Opsive.UltimateCharacterController.Character.Abilities.Ability
    {
        #region Properties
        AllyEventHandlerWrapper myEventHandler
        {
            get
            {
                if(_myEventHandler == null)
                {
                    _myEventHandler = m_CharacterLocomotion.GetComponent<AllyEventHandlerWrapper>();
                }
                return _myEventHandler;
            }
        }
        AllyEventHandlerWrapper _myEventHandler = null;

        Ability myAimAbility
        {
            get
            {
                if(_myAimAbility == null)
                {
                    _myAimAbility = m_CharacterLocomotion.GetAbility<Aim>();
                }
                return _myAimAbility;
            }
        }
        Ability _myAimAbility = null;
        #endregion

        #region Fields
        /// <summary>
        /// From Aim Ability
        /// </summary>
        private ILookSource m_LookSource;

        bool bAllyDied = false;
        bool bUpdateMyRotation = false;
        #endregion

        #region Overrides
        public override void UpdateRotation()
        {
            //Only Update Character's Rotation If FreeMoving Is Enabled And Aim Ability Isn't Active
            if (bAllyDied == false && bUpdateMyRotation && myAimAbility != null && myAimAbility.IsActive == false)
            {
                // If the character can look indepdently then the character does not need to rotate to face the look direction.
                if (m_CharacterLocomotion.ActiveMovementType.UseIndependentLook(true))
                {
                    return;
                }

                // Determine the direction that the character should be facing.
                var lookDirection = m_LookSource.LookDirection(m_LookSource.LookPosition(), true, m_LayerManager.IgnoreInvisibleCharacterLayers, false);
                var localLookDirection = m_Transform.InverseTransformDirection(lookDirection);
                localLookDirection.y = 0;
                m_CharacterLocomotion.DeltaYawRotation = MathUtility.ClampInnerAngle(Quaternion.LookRotation(localLookDirection.normalized, m_CharacterLocomotion.Up).eulerAngles.y);
            }
            base.UpdateRotation();
        }

        public override void Awake()
        {
            base.Awake();
            // The look source may have already been assigned if the ability was added to the character after the look source was assigned.
            m_LookSource = m_CharacterLocomotion.LookSource;
            EventHandler.RegisterEvent<ILookSource>(m_GameObject, "OnCharacterAttachLookSource", OnAttachLookSource);
        }

        protected override void AbilityStarted()
        {
            base.AbilityStarted();
            AbilityStartedDelay();
        }

        async void AbilityStartedDelay()
        {
            await System.Threading.Tasks.Task.Delay(500);
            myEventHandler.EventTogglebIsFreeMoving += OnFreeMoving;
            myEventHandler.EventAllyDied += OnAllyDeath;
            if (myAimAbility == null)
            {
                Debug.LogError("Aim Ability is Not Found On RTSUpdateRotAbility");
            }
        }

        protected override void AbilityStopped(bool force)
        {
            myEventHandler.EventAllyDied -= OnAllyDeath;
            myEventHandler.EventTogglebIsFreeMoving -= OnFreeMoving;
            base.AbilityStopped(force);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            EventHandler.UnregisterEvent<ILookSource>(m_GameObject, "OnCharacterAttachLookSource", OnAttachLookSource);
        }
        #endregion

        #region Handlers
        /// <summary>
        /// A new ILookSource object has been attached to the character.
        /// </summary>
        /// <param name="lookSource">The ILookSource object attached to the character.</param>
        private void OnAttachLookSource(ILookSource lookSource)
        {
            m_LookSource = lookSource;
        }

        void OnFreeMoving(bool _isFreeMoving)
        {
            bUpdateMyRotation = _isFreeMoving;
        }

        protected virtual void OnAllyDeath(Vector3 position, Vector3 force, GameObject attacker)
        {
            bAllyDied = true;
            bUpdateMyRotation = false;
        }
        #endregion
    }
}