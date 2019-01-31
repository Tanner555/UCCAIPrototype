using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Character.Abilities;
using RTSCoreFramework;

namespace RTSPrototype
{
    public abstract class AbilityBehaviourTPC : AbilityBehaviour
    {
        public override abstract void Use(GameObject target = null);

        public Opsive.UltimateCharacterController.Character.Abilities.Ability TPCAbility
        {
            get
            {
                if (_TPCAbility == null)
                    _TPCAbility = GetTPCAbility();

                return _TPCAbility;
            }
        }
        private Opsive.UltimateCharacterController.Character.Abilities.Ability _TPCAbility = null;

        protected override void PlayAbilityAnimation()
        {
            if(CanUseAbility())
            {
                TPCAbility.StartAbility();
                allyEventHandler.CallEventToggleIsUsingAbility(true);
                Invoke("StopAbilityAnimation", config.GetAbilityAnimationTime());
            }
        }

        protected override void StopAbilityAnimation()
        {
            TPCAbility.StopAbility();
            allyEventHandler.CallEventToggleIsUsingAbility(false);
        }

        protected virtual Opsive.UltimateCharacterController.Character.Abilities.Ability GetTPCAbility()
        {
            //Override To Get the Actual TPC Ability
            return null;
        }

        public override bool CanUseAbility()
        {
            return TPCAbility != null && TPCAbility.CanStartAbility();
        }

        protected override void OnAllyDeath(Vector3 position, Vector3 force, GameObject attacker)
        {
            base.OnAllyDeath(position, force, attacker);
            //Stop The Ability When Ally Dies
            if (TPCAbility != null && TPCAbility.IsActive)
            {
                StopAbilityAnimation();
            }
        }
    }
}