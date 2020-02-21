﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;
//using RPG.Core;
using System;

namespace RPGPrototype.OLDAbilities
{
    public class AreaEffectBehaviour : AbilityBehaviourOLD
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
            DealRadialDamage();
            PlayParticleEffect();
            PlayAbilityAnimation();
        }

        private void DealRadialDamage()
        {
            // Static sphere cast for targets
            RaycastHit[] hits = Physics.SphereCastAll(
                transform.position,
                (config as AreaEffectConfig).GetRadius(),
                Vector3.up,
                (config as AreaEffectConfig).GetRadius()
            );

            foreach (RaycastHit hit in hits)
            {
                var damageable = hit.collider.gameObject.GetComponent<AllyMemberRPG>();
                //Cannot Hurt Self Or Allies in Same Party (Friends)
                if(damageable != null && 
                    damageable != allymember &&
                    damageable.bIsCurrentPlayer == false &&
                    damageable.IsEnemyFor(allymember))
                {
                    float damageToDeal = (config as AreaEffectConfig).GetDamageToEachTarget();
                    damageable.AllyTakeDamage((int)damageToDeal, allymember);
                }
            }
        }
    }
}