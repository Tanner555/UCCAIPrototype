﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSPrototype
{
    [CreateAssetMenu(menuName = ("RTSPrototype/NonUCCWeapon"))]
    public class WeaponConfig : ScriptableObject
    {
        public Transform gripTransform;

        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;
        [SerializeField] float timeBetweenAnimationCycles = .5f;
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] int additionalDamage = 10;
        [SerializeField] float damageDelay = .5f;

        public float GetTimeBetweenAnimationCycles()
        {
            return timeBetweenAnimationCycles;
        }

        public float GetMaxAttackRange()
        {
            return maxAttackRange;
        }

        public float GetDamageDelay()
        {
            return damageDelay;
        }

        public GameObject GetWeaponPrefab()
        {
            return weaponPrefab;
        }
        
        public AnimationClip GetAttackAnimClip()
        {
            RemoveAnimationEvents();
            return attackAnimation;
        }

        public int GetAdditionalDamage()
        {
            return additionalDamage;
        }

        // So that asset packs cannot cause crashes
        private void RemoveAnimationEvents()
        {
            // TODO look into this still happening on the Player
            attackAnimation.events = new AnimationEvent[0];
        }
    }
}