using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;

namespace RPGPrototype
{
    public abstract class AbilityConfigRPG : AbilityConfig
    {
        [SerializeField] AnimationClip abilityAnimation;

        public abstract override AbilityBehaviour AddBehaviourComponent(GameObject objectToAttachTo);

        public AnimationClip GetAbilityAnimation()
        {
            return abilityAnimation;
        }
    }
}