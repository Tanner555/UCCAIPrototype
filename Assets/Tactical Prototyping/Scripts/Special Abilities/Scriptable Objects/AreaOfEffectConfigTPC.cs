using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;

namespace RTSPrototype
{
    [CreateAssetMenu(menuName = ("RTSPrototype/SpecialAbiltiesTPC/Area Of Effect TPC"))]
    public class AreaOfEffectConfigTPC : AbilityConfigTPC
    {
        [Header("Area Effect Specific")]
        [SerializeField] float radius = 5f;
        [SerializeField] float damageToEachTarget = 15f;

        public override AbilityBehaviour AddBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<AreaOfEffectBehaviourTPC>();
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