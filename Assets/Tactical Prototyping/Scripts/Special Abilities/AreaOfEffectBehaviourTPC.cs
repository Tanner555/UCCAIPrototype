﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;
using Opsive.UltimateCharacterController.Character;

namespace RTSPrototype
{
    public class AreaOfEffectBehaviourTPC : AbilityBehaviourTPC
    {
        #region Properties
        RTSGameMaster gamemaster
        {
            get { return RTSGameMaster.thisInstance; }
        }

        RTSGameMode gamemode
        {
            get { return RTSGameMode.thisInstance; }
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
                (config as AreaOfEffectConfigTPC).GetRadius(),
                Vector3.up,
                (config as AreaOfEffectConfigTPC).GetRadius(), 
                gamemode.AllyAndCharacterLayers
            );

            Dictionary<AllyMember, RaycastHit> _hitEnemies = new Dictionary<AllyMember, RaycastHit>();
            string _allyTag = gamemode.AllyTag;
            //Obtain All Enemies To Damage From Hits
            foreach (RaycastHit hit in hits)
            {
                Transform _enemyRoot = hit.collider.transform.root;
                AllyMember _enemyMember = null;
                if(_enemyRoot.tag == _allyTag &&
                    (_enemyMember = _enemyRoot.GetComponent<AllyMember>()) != null)
                {
                    //Cannot Hurt Self Or Allies in Same Party (Friends)
                    if (_enemyMember != null &&
                        _enemyMember != allymember &&
                        _enemyMember.bIsCurrentPlayer == false &&
                        _enemyMember.IsEnemyFor(allymember) &&
                        _hitEnemies.ContainsKey(_enemyMember) == false)
                    {
                        _hitEnemies.Add(_enemyMember, hit);
                    }
                }
            }
            
            foreach (var _hitEnemy in _hitEnemies)
            {
                AllyMember damageable = _hitEnemy.Key;
                RaycastHit hit = _hitEnemy.Value;
                float damageToDeal = (config as AreaOfEffectConfigTPC).GetDamageToEachTarget();
                damageable.allyEventHandler.CallOnAllyTakeDamage(
                    (int)damageToDeal, hit.point, Vector3.zero,
                    allymember, hit.transform.gameObject, hit.collider
                    );
            }
        }

        protected override Opsive.UltimateCharacterController.Character.Abilities.Ability GetTPCAbility()
        {
            return myController.GetAbility<RTSAreaEffectAbility>();
        }
    }
}
