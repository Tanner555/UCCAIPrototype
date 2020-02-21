using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;

namespace RPGPrototype
{
    [CreateAssetMenu(menuName = ("RTSPrototype/SpecialAbiltiesRPG/Area Of Effect RPG"))]
    public class AreaOfEffectConfigRPG : AbilityConfigRPG
    {
        [Header("Area Effect Specific")]
        [SerializeField] float radius = 5f;
        [SerializeField] float damageToEachTarget = 15f;

        public override AbilityBehaviour AddBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<AreaOfEffectBehaviourRPG>();
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