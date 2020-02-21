﻿﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

namespace RPGPrototype.OLDAbilities
{
    public class PowerAttackBehaviour : AbilityBehaviourOLD
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

        public override void Use(GameObject target)
        {
            PlayAbilitySound();
            DealDamage(target);
            PlayParticleEffect();
            PlayAbilityAnimation();
        }

        private void DealDamage(GameObject target)
        {
            float damageToDeal = (config as PowerAttackConfig).GetExtraDamage();
            target.GetComponent<AllyMemberRPG>().AllyTakeDamage((int)damageToDeal, allymember);
        }
    }
}