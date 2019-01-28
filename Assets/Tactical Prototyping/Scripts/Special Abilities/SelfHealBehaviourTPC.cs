using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;
using Opsive.UltimateCharacterController.Character;

namespace RTSPrototype
{
    public class SelfHealBehaviourTPC : AbilityBehaviourTPC
    {
        #region Properties
        RTSGameMaster gamemaster
        {
            get { return RTSGameMaster.thisInstance; }
        }

        AllyEventHandler eventhandler
        {
            get
            {
                if (_eventhandler == null)
                    _eventhandler = GetComponent<AllyEventHandler>();

                return _eventhandler;
            }
        }
        AllyEventHandler _eventhandler = null;
        AllyMember allymember
        {
            get
            {
                if (_allymember == null)
                    _allymember = GetComponent<AllyMember>();

                return _allymember;
            }
        }
        AllyMember _allymember = null;

        UltimateCharacterLocomotion myController
        {
            get
            {
                if (_myController == null)
                    _myController = GetComponent<UltimateCharacterLocomotion>();

                return _myController;
            }
        }
        UltimateCharacterLocomotion _myController = null;
        #endregion

        public override void Use(GameObject target = null)
        {
            float _extraHealth = (config as SelfHealConfigTPC).GetExtraHealth();
            PlayAbilitySound();
            allymember.AllyHeal((int)_extraHealth);
            PlayParticleEffect();
            PlayAbilityAnimation();
        }

        protected override Opsive.UltimateCharacterController.Character.Abilities.Ability GetTPCAbility()
        {
            return myController.GetAbility<RTSSelfHealAbility>();
        }
    }
}