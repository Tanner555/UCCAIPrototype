using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;
using Opsive.UltimateCharacterController.Game;

namespace RTSPrototype
{
    public class AllyVisualsWrapper : AllyVisuals
    {
        #region Overrides
        protected override void OnHealthAfterTakeDamage(int amount, Vector3 position, Vector3 force, AllyMember _instigator, Collider hitCollider)
        {
            base.OnHealthAfterTakeDamage(amount, position, force, _instigator, hitCollider);
            if (BloodParticles == null) return;
            ObjectPool.Instantiate(BloodParticles, position, Quaternion.identity);
        }
        #endregion
    }
}