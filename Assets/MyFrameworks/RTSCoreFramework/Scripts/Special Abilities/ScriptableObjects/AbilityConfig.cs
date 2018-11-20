﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCoreFramework
{
    public abstract class AbilityConfig : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] protected float energyCost = 10f;
        [SerializeField] protected GameObject particlePrefab;
        [SerializeField] protected AudioClip[] audioClips;
        [SerializeField] protected float AbilityAnimationTime = 1f;

        /// <summary>
        /// Determines Behavior To Be Added
        /// </summary>
        /// <param name="objectToAttachTo"></param>
        /// <returns></returns>
        public abstract AbilityBehaviour AddBehaviourComponent(GameObject objectToAttachTo);

        public virtual float GetEnergyCost()
        {
            return energyCost;
        }

        public virtual GameObject GetParticlePrefab()
        {
            return particlePrefab;
        }

        public virtual AudioClip GetRandomAbilitySound()
        {
            if (audioClips.Length <= 0) return null;
            return audioClips[Random.Range(0, audioClips.Length)];
        }

        public virtual float GetAbilityAnimationTime()
        {
            return AbilityAnimationTime;
        }
    }
}