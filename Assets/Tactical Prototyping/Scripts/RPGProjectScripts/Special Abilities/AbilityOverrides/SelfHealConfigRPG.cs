using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;

namespace RPGPrototype
{
    [CreateAssetMenu(menuName = ("RTSPrototype/SpecialAbiltiesRPG/Self Heal RPG"))]
    public class SelfHealConfigRPG : AbilityConfigRPG
    {
        [Header("Self Heal Specific")]
        [SerializeField] float extraHealth = 50f;

        public override AbilityBehaviour AddBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<SelfHealBehaviourRPG>();
        }

        public float GetExtraHealth()
        {
            return extraHealth;
        }
    }
}