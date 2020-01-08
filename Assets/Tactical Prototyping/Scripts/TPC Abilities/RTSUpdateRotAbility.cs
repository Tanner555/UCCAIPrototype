using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseFramework;
using RTSCoreFramework;
using Opsive.UltimateCharacterController.Utility;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using uccEventHelper = UtilitiesAndHelpersForUCC.UCCEventsControllerUtility;

namespace RTSPrototype
{
    /// <summary>
    /// TODO: RTSPrototype Start RTSRotate Ability Manually Inside Behavior Tree
    /// </summary>
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

        //local stored vars
        private Vector3 myLookDirection;
        private Vector3 myLocalLookDirection;
        private Vector3 myDeltaRotation;
        #endregion

        #region Overrides
        public override void UpdateRotation()
        {
            //Only Update Character's Rotation If FreeMoving Is Enabled And Aim Ability Isn't Active
            //if (bAllyDied == false && bUpdateMyRotation && myAimAbility != null && myAimAbility.IsActive == false)
            //{
            //    // If the character can look indepdently then the character does not need to rotate to face the look direction.
            //    if (m_CharacterLocomotion.ActiveMovementType.UseIndependentLook(true))
            //    {
            //        return;
            //    }

            //    // Determine the direction that the character should be facing.
            //    myLookDirection = m_LookSource.LookDirection(m_LookSource.LookPosition(), true, m_CharacterLayerManager.IgnoreInvisibleCharacterLayers, false);
            //    myLocalLookDirection = m_Transform.InverseTransformDirection(myLookDirection);
            //    myLocalLookDirection.y = 0;
            //    myDeltaRotation = m_CharacterLocomotion.DeltaRotation;
            //    myDeltaRotation.y = MathUtility.ClampInnerAngle(Quaternion.LookRotation(myLocalLookDirection.normalized, m_CharacterLocomotion.Up).eulerAngles.y);
            //    m_CharacterLocomotion.DeltaRotation = myDeltaRotation;
            //}
            base.UpdateRotation();
        }

        public override void Awake()
        {
            base.Awake();
            // The look source may have already been assigned if the ability was added to the character after the look source was assigned.
            m_LookSource = m_CharacterLocomotion.LookSource;

            uccEventHelper.RegisterOnCharacterAttachLookSource(m_GameObject, OnAttachLookSource);
        }

        protected override void AbilityStarted()
        {
            base.AbilityStarted();
            m_CharacterLocomotion.StartCoroutine(AbilityStartedDelayCoroutine());
        }

        IEnumerator AbilityStartedDelayCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            //myEventHandler.EventTogglebIsFreeMoving += OnFreeMoving;
            myEventHandler.EventAllyDied += OnAllyDeath;
            if (myAimAbility == null)
            {
                Debug.LogError("Aim Ability is Not Found On RTSUpdateRotAbility");
            }
        }

        protected override void AbilityStopped(bool force)
        {
            myEventHandler.EventAllyDied -= OnAllyDeath;
            //myEventHandler.EventTogglebIsFreeMoving -= OnFreeMoving;
            base.AbilityStopped(force);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            uccEventHelper.UnregisterOnCharacterAttachLookSource(m_GameObject, OnAttachLookSource);
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