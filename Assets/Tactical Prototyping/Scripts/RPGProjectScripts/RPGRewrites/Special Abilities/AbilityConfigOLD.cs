﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using RPG.Core;
using RPG.Characters;

namespace RPGPrototype.OLDAbilities
{
    public abstract class AbilityConfigOLD : ScriptableObject
    {
        [Header("Spcial Ability General")]
        [SerializeField] float energyCost = 10f;
        [SerializeField] GameObject particlePrefab;
        [SerializeField] AnimationClip abilityAnimation;
        [SerializeField] AudioClip[] audioClips;

        /// <summary>
        /// Determines Behavior To Be Added
        /// </summary>
        /// <param name="objectToAttachTo"></param>
        /// <returns></returns>
        public abstract AbilityBehaviourOLD AddBehaviourComponent(GameObject objectToAttachTo);

        public float GetEnergyCost()
        {
            return energyCost;
        }

        public GameObject GetParticlePrefab()
        {
            return particlePrefab;
        }

        public AnimationClip GetAbilityAnimation()
        {
            return abilityAnimation;
        }

        public AudioClip GetRandomAbilitySound()
        {
            return audioClips[Random.Range(0, audioClips.Length)];
        }
    }
}