﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

namespace RPGPrototype.OLDAbilities
{
    [CreateAssetMenu(menuName = ("RPG/Special Abiltiy/Area Effect"))]
    public class AreaEffectConfig : AbilityConfigOLD
    {
        [Header("Area Effect Specific")]
        [SerializeField] float radius = 5f;
        [SerializeField] float damageToEachTarget = 15f;

        public override AbilityBehaviourOLD AddBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<AreaEffectBehaviour>();
        }

        public float GetDamageToEachTarget()
        {
            return damageToEachTarget;
        }

        public float GetRadius()
        {
            return radius;
        }
    }
}