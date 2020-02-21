using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

namespace RPGPrototype.OLDAbilities
{
    public class SelfHealBehaviour : AbilityBehaviourOLD
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

        void Start()
        {
            
        }

		public override void Use(GameObject target)
		{
            float _extraHealth = (config as SelfHealConfig).GetExtraHealth();
            PlayAbilitySound();
            allymember.AllyHeal((int)_extraHealth);
            PlayParticleEffect();
            PlayAbilityAnimation();
		}
    }
}