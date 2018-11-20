using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;

namespace RTSPrototype
{
    [CreateAssetMenu(menuName = ("RTSPrototype/SpecialAbiltiesTPC/Self Heal TPC"))]
    public class SelfHealConfigTPC : AbilityConfigTPC
    {
        [Header("Self Heal Specific")]
        [SerializeField] float extraHealth = 50f;

        public override AbilityBehaviour AddBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<SelfHealBehaviourTPC>();
        }

        public float GetExtraHealth()
        {
            return extraHealth;
        }
    }
}
