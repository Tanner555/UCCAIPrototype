using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.ThirdPersonController.Abilities;
using RTSCoreFramework;

namespace RTSPrototype
{
    public abstract class AbilityConfigTPC : AbilityConfig
    {
        public abstract override AbilityBehaviour AddBehaviourComponent(GameObject objectToAttachTo);


    }
}
