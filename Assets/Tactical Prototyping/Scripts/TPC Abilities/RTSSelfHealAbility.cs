﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;
using Opsive.UltimateCharacterController.Character.Abilities;

namespace RTSPrototype
{
    [DefaultStartType(AbilityStartType.Manual)]
    [DefaultStopType(AbilityStopType.Manual)]
    [DefaultAbilityIndex(202)]
    public class RTSSelfHealAbility : Ability
    {
        RTSGameMode gamemode
        {
            get { return RTSGameMode.thisInstance; }
        }

        public override bool CanStartAbility()
        {
            return base.CanStartAbility() && this.IsActive == false;
        }
    }
}