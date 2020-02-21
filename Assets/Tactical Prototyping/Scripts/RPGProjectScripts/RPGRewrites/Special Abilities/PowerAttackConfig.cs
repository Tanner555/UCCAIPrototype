﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

namespace RPGPrototype.OLDAbilities
{
    [CreateAssetMenu(menuName = ("RPG/Special Abiltiy/Power Attack"))]
    public class PowerAttackConfig : AbilityConfigOLD
    {
        [Header("Power Attack Specific")]
        [SerializeField] float extraDamage = 10f;

        public override AbilityBehaviourOLD AddBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<PowerAttackBehaviour>();
        }
        public float GetExtraDamage()
        {
            return extraDamage;
        }
    }
}