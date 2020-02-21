using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

namespace RPGPrototype.OLDAbilities
{
    [CreateAssetMenu(menuName = ("RPG/Special Abiltiy/Self Heal"))]
    public class SelfHealConfig : AbilityConfigOLD
	{
		[Header("Self Heal Specific")]
		[SerializeField] float extraHealth = 50f;

        public override AbilityBehaviourOLD AddBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<SelfHealBehaviour>();
        }

		public float GetExtraHealth()
		{
			return extraHealth;
		}
	}
}