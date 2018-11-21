using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;
using Opsive.UltimateCharacterController.Character.Abilities;

namespace RTSPrototype
{
    /// <summary>
    /// TODO: RTSPrototype Fix RTSSelfHealAbility Ability
    /// </summary>
    public class RTSSelfHealAbility : Ability
    {
        RTSGameMode gamemode
        {
            get { return RTSGameMode.thisInstance; }
        }

        //public override string GetDestinationState(int layer)
        //{
        //    if (layer != m_AnimatorMonitor.BaseLayerIndex && layer != m_AnimatorMonitor.UpperLayerIndex &&
        //        !m_AnimatorMonitor.ItemUsesAbilityLayer(this, layer))
        //    {
        //        return string.Empty;
        //    }

        //    return "SelfHeal.Movement";
        //}

        public override bool CanStartAbility()
        {
            return base.CanStartAbility() && this.IsActive == false;
        }
    }
}